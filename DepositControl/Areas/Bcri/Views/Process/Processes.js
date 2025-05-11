var Common = null;

$(document).ready(function () {
    Common = $("#ProcessesJsData").data();

    function actions(cellvalue, options, rowObject) {
        var actionsRow = $("#processActionGridTemplate").clone();
        actionsRow.children()
            .attr("data-processId", rowObject.Id)
            .css("display", "inline-table");
        //recorrer el cellvalue y ocultar botones que no corresponden

        return actionsRow.html().replace(/\n/g, "").replace(/\r/g, "").replace(/\t/g, ""); // el enter activa un stylo en la jqgrid que queda feo
    }
    var dicData = {};

    LoadGrid();

    RecargarGrilla();
	InitAllGrids();
	$("#parametrics").select2({ width: '50%' });

	$("#parametrics").on("change", function (obj) {
		$("#processParam .in.active").removeClass("in active");
		$("#" + $(this).val()).addClass("in active");
		//Inicializo el anchor correcto
		$("#" + $(this).val() + "-grid").setGridWidth($("#" + $(this).val() + ">.row>.col-lg-12").width());
	});
	$("#parametrics").trigger("change");
	// Para que sea responsive
	$(window).bind('resize', function () {
		$.each($("#parametrics")[0], function (i,e) {
			$("#" + $(e).val() + "-grid").setGridWidth($("#" + $(e).val() + ">.row>.col-lg-12").width());
		});
		
	});

    function LoadGrid() {
        var myGrid = $("#jqGridProcess"), currentPage = 1;
        $("#jqGridProcess").jqGrid({
            url: Common.dataurl,
            editurl: Common.editurl,
            datatype: "json",
            colModel: [
                {
                    label: "Id",
                    name: "Id",
                    key: true,
                    hidden: true
                },
                {
                    label: res.period,
                    name: "Period",
                    width: 110,
                    fixed: true,
                    edittype: "date",
                    editable: true,
                    sorttype: 'date',
                    editrules: { required: true, edithidden: true, date: true },
                    formatter: function (cellValue) {
                        return moment(cellValue).format("YYYY-MM-DD");
                    },
                    searchoptions: {
                        dataInit: function (element) {
                            $(element).datepicker({
                                dateFormat: "yy-mm-dd",
                                onSelect: function () {
                                    myGrid[0].triggerToolbar();
                                }
                            });
                        },
                        sopt: ["eq"]
                    }
                },
                {
                    label: " ",
                    name: "Version",
                    width: 35,
                    sorttype: "text",
                    summaryType: "max",
                    search: false,
                    fixed: true
                },
                {
                    label: res.state,
                    name: "State",
                    fixed: true,
                    width: 130,
                    formatter: function (cellvalue, options, rowObject) {
                        return cellvalue != "Error"
                            ? cellvalue
                            : "Error <i class='fa fa-eye' title='View Detail' data-errorprocessid='" + rowObject.Id + "'> " +
                            "<span style='display: none'>" + rowObject.Error + "</span></i>";
                    }
                },
                {
                    label: res.error,
                    name: "Error",
                    hidden: true,
                    editable: false
                },
                {
                    label: res.executed,
                    name: "Date",
                    width: 150,
                    fixed: true,
                    editable: false,
                    sorttype: 'date',
                    formatter: function (cellValue) {
                        return moment(cellValue).format('YYYY-MM-DD h:mm').toString();
                    },
                    searchoptions: {
                        sopt: ["cn"]
                    }
                },
                {
                    label: res.action,
                    name: "Action",
                    fixed: true,
                    width: 280,
                    search: false,
                    title: "",
                    formatter: actions,
                    sortable: false
                }
            ],
            loadComplete: function() {
                dicData = {},
                    data = $("#jqGridProcess").jqGrid('getGridParam', 'data');
                $.each(data, function (i, e) {
                    dicData[e.Id] = e;
                });

            },
            gridComplete: function () {


                $('i[data-errorprocessid]').unbind("click");
                $('i[data-errorprocessid]').on('click', function () {

                    BootstrapDialog.show({
                        title: res.errorDescription,
                        message: $(this).text()
                    });
                });

                $(".processActionGrid a[data-action]").unbind("click"); // activo las acciones
                $(".processActionGrid a[data-action]").on("click", function (e) {
                    e.preventDefault();
                    var processId = $(this).parent("div[data-processId]").attr("data-processId");
                    var action = $(this).attr("data-action");
                    if (action === "Details")
						window.location.href = processController + "/ProcessDetail/" + processId;
					else if (action === "Export")
						window.location.href = processController + "/Export/" + processId;
                    else if (action === "Delete")
                        $("#jqGridProcess").jqGrid('delGridRow', processId,
                        {
                            reloadAfterSubmit: true
                            //,url: "",
                            //    onclickSubmit: function(params) {
                            //        $.ajax({
                            //                method: "post",
                            //                url: processController + "/" + action + "/" + processId,
                            //                success: function (data) {},
                            //                error: function () {}
                            //            });
                            //    }
                        });
                    else {

                        $.ajax({
                            method: "post",
                            url: processController + "/" + action + "/" + processId,
                            success: function (data) {
                                if (action === "Delete") 
                                    $("#jqGridProcess").jqGrid('delGridRow', processId);
                                else
                                    $("#jqGridProcess").setRowData(data[0].Id, data[0], null);
                            },
                            error: function () {
                            }
                        });
                    }
                });
            },
            sortname: "Period",
            sortorder: "desc",
            loadonce: true,
            viewrecords: false,
            height: 310,
            width: 720,
            autowidth: false,
            onSelectRow: function (id) {
                $("#ProcessDetail").load(Common.detailurl + "/" + id, function () {
                    initRepositoryGrid("#ProcessDetail");
                    initCommets();
                });

                return true;
            },
            //width: 550,//el width esta definido por el div que contiene la tabla, esto no funciona
            rowNum: 10,
            pager: "#jqGridProcessPager"
        });
        $("#jqGridProcess").jqGrid("filterToolbar");
        $("#jqGridProcess").navGrid("#jqGridProcessPager",
            // the buttons to appear on the toolbar of the grid
            { edit: false, add: false, del: false, search: false, refresh: true, view: false, position: "left", cloneToTop: false },
            // options for the Edit Dialog
            {
                editCaption: "The Edit Dialog",
                recreateForm: true,
                checkOnUpdate: true,
                checkOnSubmit: true,
                closeAfterEdit: true,
                errorTextFormat: function (data) {
                    return "Error: " + data.responseText;
                }
            },
            // options for the Add Dialog
            {
                closeAfterAdd: true,
                recreateForm: true,
                errorTextFormat: function (data) {
                    return "Error: " + data.responseText;
                },
                beforeSubmit: function (postdata, $form) {
                    var dateInputs = $form.find("input[type=date]");
                    for (var i = 0, length = dateInputs.length; i < length; i++) {
                        var input = $(dateInputs[i]);
                        postdata[input.attr("name")] = input.val();
                    }
                    var saveButton = $('#editmodjqGridProcess').find("#sData");
                    saveButton.attr("disabled", true);
                    return [true, ""];
                },
                afterSubmit: function (response, postdata) {
                    $("#jqGridProcess")
                        .setGridParam({ datatype: "json" })
                        .setGridParam({ page: currentPage })
                        .setGridParam({ url: Common.dataurl })
                        .trigger("reloadGrid");

                    //document.location.reload(true);
                    return [true, ""];
                }
            },
            // options for the Delete Dailog
            {
                errorTextFormat: function (data) {
                    return "Error: " + data.responseText;
                }
            }); 
    }

    function RecargarGrilla() {

        setInterval(function () {
            if (Object.keys(dicData).length === 0) return;

            $.ajax({
                url: Common.dataurl,
                success: function(newData) { //trago la data de la grilla actualzada
                    $.each(newData, function (i, e) {
                        //compara cada elemento con el que esta actualmente en la grilla 
                        //dicData es un diccionario por id, se carga en el evento loadComplete de la grilla
                        if (!(dicData.hasOwnProperty(e.Id) && dicData[e.Id].State === e.State && dicData[e.Id].Date === e.Date)) {
                            $("#jqGridProcess").setRowData(e.Id, e, null); //Cargo el nuevos dato en la grilla
                            dicData[e.Id] = e;
                        }
                    });
                }
            });
        }, 20000);
    }
});

$('#sData').click(function () {
    $('#editmodjqGridProcess').hide();
});