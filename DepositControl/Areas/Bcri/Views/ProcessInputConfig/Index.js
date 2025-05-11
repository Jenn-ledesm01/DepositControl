


$(document)
    .ready(function () {
        var Common = $("#ProcessInputConfigJsData").data();
        var gridTable = "#jqGridProcessInputConfig";
        var gridPager = gridTable + "Pager";


        var OptionsData = '';
        $.ajax({
            url: Common.optionsdataurl,
            global: false,
            type: "GET",
            dataType: "json",
            async: false,
            success: function (dataJson) {
                OptionsData = dataJson;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status + " " + thrownError);
            }
        });
        var respositoryConfigOptionsData = OptionsData.respositoryConfig;
        var frecuencySimpleInput = OptionsData.frecuencySimpleInput;
        var visibilityConfigOptionsData = OptionsData.visibilityConfig;
        
        
		var EditOptions = {
			editCaption: "Edit",
			recreateForm: true,
			checkOnUpdate: false,
			checkOnSubmit: false,
			closeAfterEdit: true,
			onInitializeForm: function (formid) { //validacion de duplicado (solo anda con data local)
				$('#RepositoryConfig', formid).on('change', function () {
					$('input#Code', formid).val($(this.selectedOptions[0]).val());
				});

			},
			//,serializeEditData: function (postdata) {
			//    postdata.repositoryConfigId = gridRepoData.customRepositoryconfigid;
			//    postdata.repositoryId = gridRepoData.customRepositoryid;
			//    return postdata;
			//}
			serializeEditData: function (postdata) {
				postdata.ProcessConfigId = postdata.ProcessConfig || Common.processconfigid;
				return postdata;
			},
			beforeShowForm: function (formId) {
				var start = "<a href='" + baseUrl + "Entity?Name=";
				Code = formId.find("#Code").val();
				formId.find("#Code").val(Code.substr(start.length, Code.substr(start.length).indexOf('"')));
			},
			afterShowForm: function (formId) {
				formId.find("#FrecuencySimpleInput").on("change", function () {

					if ($(this).find("option:selected").text().toLowerCase() === "custom")
						$(this).parent().parent().parent().append($(this).parent().parent().clone().empty().prop("id", "downloadBtnContainer").append("<td colspan='2'><a href='" + baseUrl + "/ProcessInputConfig/DownloadCustomInputConfig?ProcessInputConfigCode=" + $("#Code").val() + "' class='fm-button btn btn-default fm-button-icon-left'>Descargar Template</a></td>"));
					else
						$(this).parent().parent().parent().find("#downloadBtnContainer").remove();
				});
				$("#FrecuencySimpleInput").trigger("change");
			},
			afterSubmit: function () {
				$(gridTable).jqGrid("setGridParam", {
					datatype: "json",
					postData: {
						ProcessConfigId: Common.processconfigid
					}
				});
				$(gridTable).trigger("reloadGrid");
				return [true];
			},
			viewPagerButtons: false,
			errorTextFormat: function (data) {
				return "Error: " + data.responseText;
			}
		};
        $(gridTable).jqGrid({
            editurl: Common.editurl,
            url: Common.dataurl,
            postData: {
                ProcessConfigId: Common.processconfigid
            },
            colModel: [

                {
                    hidden: true,
                    key: true,
                    name: "Id"
                }, {
             
                    name: "ProcessConfig",
                    hidden: true,
                    key: false
                }, {
                    editable: true,
                    editoptions: { "value": respositoryConfigOptionsData },
                    edittype: "select",
                    formatter: "select",
                    label: res.repository,
                    name: "RepositoryConfig",
                    width: 260,
                    resizable: true,
                    search: true,
                    searchoptions: { "value": respositoryConfigOptionsData },
                    stype: "select"

                }, {
                    label: res.code,
                    name: "Code",
                    width: 260,
                    editable: true,
                    formatter: function (val, options, rowData) {
                        return "<a href='" + baseUrl + "Entity?Name=" + val + "'>" + val + "</a>";
                    }
                }, {
                    editable: true,
                    editoptions: { "value": frecuencySimpleInput , requiered : true},
                    edittype: "select",
                    formatter: "select",
                    label: res.relativeDate ,
                    width: 150,
                    name: "FrecuencySimpleInput",
                    resizable: true,
                    search: true,
                    searchoptions: { "value": ": ;" + frecuencySimpleInput },
                    stype: "select"
                },

                    //}, {
                //    align: "right",
                //    editable: true,
                //    editoptions: { "value": frecuencyConfigOptionsData },
                //    edittype: "select",
                //    formatter: "select",
                //    label: "FrecuencyConfig",
                //    name: "FrecuencyConfig",
                //    resizable: true,
                //    search: true,
                //    searchoptions: { "value": frecuencyConfigOptionsData },
                //    stype: "select"
                //}, {
                //    label: "FrecuencyIncrease",
                //    name: "FrecuencyIncrease",
                //    width: 100,
                //    editable: true}
                 { 
                      editable: true,
                      editoptions: { "value": visibilityConfigOptionsData },
                      edittype: "select",
                      formatter: "select",
                      label: res.visibility,
                      name: "Visibility",
                      width: 150,
                      resizable: true,
                      search: true,
                      searchoptions: { "value": ": ;" + visibilityConfigOptionsData },
                      stype: "select"
                }, {
                     label: "RelatedProcess",
                     name: "RelatedProcess",
                     editable: false,
                     formatter: function (val, options, rowData) {
                         if (val.length === 0)
                             return "";
                         var resp = res.outputFrom;
                         $.each(val, function (i, o) {
                             resp += "\n" + "<a href='" + baseUrl + "Config/" + o.Code + "'>" + o.Name + "</a>";
                         });
                         return resp;
                     }
                }
            ],
            ajaxGridOptions: {cache: false},
            pager: gridPager,
            datatype: "json",
            sortname: "Code",
            sortorder: "ASC",
            loadonce: true,
            sortable: true,
            viewrecords: true,
            rownumbers: false,
            shrinkToFit: false,
            forceFit: false,
            ignoreCase: true,
            width: 'auto',
            height:'auto',
            maxHeight: 250,
            rowNum: 20,
            gridview: false
        });


        $(gridTable).jqGrid('filterToolbar');
        $(gridTable).navGrid(gridPager,
            // the buttons to appear on the toolbar of the grid
            { edit: true, add: true, del: true, search: false, refresh: false, view: true, position: "left", cloneToTop: false },
            // options for the Edit Dialog
            EditOptions,
            // options for the Add Dialog
            {
                closeAfterAdd: true,
                recreateForm: true,
                beforeSubmit: EditOptions.beforeSubmit,
                onInitializeForm: EditOptions.onInitializeForm,
                serializeEditData: EditOptions.serializeEditData,
                afterSubmit: EditOptions.afterSubmit,
                errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        // options for the Delete Dailog
        {
            afterSubmit: EditOptions.afterSubmit,
            onclickSubmit: function () {
                return {
                     ProcessConfigId: Common.processconfigid
                };
            },
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        });
    });