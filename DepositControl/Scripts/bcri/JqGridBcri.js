$(document).ready(function () {

    InitFirstGrid();
});
 function InitFirstGrid() {
    var grids = $("table[data-entityname]");
    if (grids.length === 0) return;
    InitGrid(grids[0]);
}

function InitAllGrids() {
    var grids = $("table[data-entityname]");
    if (grids.length === 0) return;
    $.each(grids, function (i) {
        InitGrid(grids[i]);
    });
}

function GetJqGrid(grid) {
    var resul;
    $.ajax({
        url: baseUrl + "Grid/JqGrid",
        global: false,
        type: "GET",
        data: grid.data(),
        dataType: "json",
        async: false,
        success: function (dataJson) {
            resul = dataJson;
        },
        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status + " " + thrownError);
        }
    });
    return resul;
}

function InitGrid(grid) {

    grid = $(grid);


    if (grid.hasClass('ui-jqgrid-btable')) return; //ya esta inicializada tonses me voy 

    var pagerId = "#" + grid.attr("id") + "-Pager";

    $("option[data-toggle=\"tab\"]").on("change", function(e) {
        grid.trigger("resize");
    });

    var gridConfig = grid.data();

    var editable = gridConfig.editable.toLowerCase() == "true";
    var exportable = gridConfig.exportable.toLowerCase() == "true";

    var gridModel = GetJqGrid(grid);

    var prevDate;
    //    

	gridModel["gridComplete"] = function () {
		//ajuste de filtros para el datepicker
		$("div[role=grid] input[date=true]").datepicker({
			beforeShow: function (el, inst) {
				prevDate = $("#" + el.id, "#gbox_" + grid.prop("id")).datepicker("getDate");
			},
			onSelect: function (val, inst) {
				if ($(".active .active #" + inst.input[0].id).length > 0 && $(".active .active #" + inst.input[0].id)[0] != inst.input[0]) {
					$(".active .active #" + inst.input[0].id).datepicker("setDate", inst.input.datepicker("getDate"));
					inst.input.datepicker("setDate", prevDate);
				}
				var actualGrid = $.grep($(".active .active table"), function (e, i) { return e.id ? true : false; });
				actualGrid[actualGrid.length - 1].triggerToolbar();
			}
		});
	};
    grid.jqGrid(
        gridModel
    );


	var editOptions = {
		editCaption: res.edit,
		recreateForm: true,
		checkOnUpdate: false,
		checkOnSubmit: false,
		closeAfterEdit: true,
		beforeShowForm: function ($form) {
			var rowData = $(this).jqGrid('getRowData', $form.find("#id_g").val());
			var dateInputs = $form.find("input[type=date]"); //argega los campos de fecha al postdata
			for (var i = 0, length = dateInputs.length; i < length; i++) {
				var input = dateInputs[i];
				var dateStringLocaleFormat = rowData[$(input).attr("name")];
				input.valueAsDate = moment(dateStringLocaleFormat, 'L').toDate();
			}
		},
		beforeSubmit: function (postdata, $form) {
			var getColDef = function (nameCol) {
				var colDef = grid.jqGrid('getGridParam', 'colModel');
				var def = {};
				$(colDef).each(function (i, e) {
					if (e.name == nameCol) {
						def = e;
					}
				});

				return def;
			};


			var dateInputs = $form.find("input[type=date]"); //argega los valores de fecha al formulario
			if (dateInputs.length > 0) {
				for (var i = 0, length = dateInputs.length; i < length; i++) {
					var input = $(dateInputs[i]);
					var inputName = input.attr("name");

					var cd = getColDef(inputName);
					if (cd.editrules.required && !moment(input.val()).isValid()) {
						return [false, cd.label + ' ' + res.msgisrequired];
					}

					postdata[inputName] = input.val() || '_empty'; //ver si hace falta agregar formateo

				}
			}

			var tmpField;
			for (key in postdata) {
				var cD = getColDef(key);
				if (cD.formatter != "text") {
					tmpField = Number(postdata[key]);
					if (!Number.isInteger(tmpField)) {
						tmpField = Globalize.formatNumber(tmpField);
					}
					postdata[key] = tmpField === "NeuN" || tmpField === "NaN" ? postdata[key] : tmpField;
				}
			}

			return [true, ""];
		},
		serializeEditData: function (postdata) {
			postdata.repositoryConfigId = gridConfig.repositoryconfigid;
			postdata.EntityName = gridConfig.entityname;
			postdata.repository_Id = gridConfig.repositoryid;
			return postdata;
		},
		afterSubmit: function () {
			grid.jqGrid("setGridParam", {
				datatype: "json",
				postData: {
					repositoryConfigId: gridConfig.repositoryconfigid,
					EntityName: gridConfig.entityname,
					repository_Id: gridConfig.repositoryid
				}
			});
			grid.trigger("reloadGrid");
			return [true];
		},
		viewPagerButtons: false,
		errorTextFormat: function (data) {
			return "Error: " + data.responseText;
		},
		formatter: {
			integer: { thousandsSeparator: "", defaultValue: '0' }
		}
	};

    grid.jqGrid('filterToolbar');
    //$.jgrid.loadState(grid.attr('id'));
    grid.navGrid(pagerId,
        // the buttons to appear on the toolbar of the grid
        { edit: editable, add: editable, del: editable, search: true, refresh: true, view: true, position: "left", cloneToTop: false },
        // options for the Edit Dialog
        editOptions,
        // options for the Add Dialog
        {
            closeAfterAdd: true,
            recreateForm: true,
            serializeEditData: editOptions.serializeEditData,
            beforeSubmit: editOptions.beforeSubmit,
            afterSubmit: editOptions.afterSubmit,
            errorTextFormat: function(data) {
                return "Error: " + data.responseText;
            }
        },
        // options for the Delete Dailog
        {
            serializeDelData: editOptions.serializeEditData,
            afterSubmit: editOptions.afterSubmit,
            errorTextFormat: function(data) {
                return "Error: " + data.responseText;
            }
        });

    if (exportable) {
        var tdDownload = $('<td class="ui-pg-button" title="Download csv" id="download_' + grid.attr("id") + '"><div class="ui-pg-div"><span class="fa fa-file-excel-o"></span></div></td>');
        tdDownload.click(function() {
            window.location = baseUrl + 'Grid/GetCsv?entityName=' + gridConfig.entityname + '&repository_Id=' + gridConfig.repositoryid;
        });
        $(pagerId).find(".ui-pg-table .ui-common-table tbody tr:first").append(tdDownload);
    }
    $(window).bind("beforeunload", function() {
        $.jgrid.saveState(grid.attr("id"));
    });
}