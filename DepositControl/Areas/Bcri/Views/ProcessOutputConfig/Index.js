


$(document)
    .ready(function () {
        var Common = $("#ProcessOutputConfigJsData").data();
        var gridTable = "#jqGridProcessOutputConfig";
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
        var visibilityConfigOptionsData = OptionsData.visibilityConfig;

        var EditOptions = {
            editCaption: "Edit",
            recreateForm: true,
            checkOnUpdate: false,
            checkOnSubmit: false,
            closeAfterEdit: true,
            //beforeSubmit: function(postdata) { //validacion de duplicado (solo anda con data local)
            //    grid = $(this);
            //    var id = postdata[grid.attr("Id") + "_id"];
            //    return [true];
            //},
            serializeEditData: function (postdata) {
                postdata.ProcessConfigId = postdata.ProcessConfig || Common.processconfigid;
                return postdata;
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

        }
        $(gridTable).jqGrid({
            editurl: Common.editurl,
            url: Common.dataurl,
            postData: {
                ProcessConfigId: Common.processconfigid
            },
            colModel: [{
                name: "Id",
                hidden: true,
                key: true

            }, {

                name: "ProcessConfig",
                hidden: true,
                key: false

            }, {
                
                editable: true,
                editoptions: { "value": respositoryConfigOptionsData },
                edittype: "select",
                width: 260,
                formatter: "select",
                label: "Repository",
                name: "RepositoryConfig",
                resizable: true,
                search: true,
                searchoptions: { sopt: ["cn"], value: respositoryConfigOptionsData},
                stype: "select"
            }, {
                label: "Code",
                name: "Code",
                width: 260,
                editable: false,
                formatter: function (val, options, rowData) {
                    return "<a href='" + baseUrl + "Entity?Name=" + val + "'>" + val + "</a>";
                }
            },  {
                editable: true,
                editoptions: { "value": visibilityConfigOptionsData },
                edittype: "select",
                formatter: "select",
                label: "Visibility",
                name: "Visibility",
                width: 160,
                resizable: true,
                search: true,
                searchoptions: { sopt: ["cn"], value: ": ;" + visibilityConfigOptionsData },
                stype: "select"
            }, {
                label: "RelatedProcess",
                name: "RelatedProcess",
                editable: false,
                formatter: function (val, options, rowData) {
                    if (val.length === 0)
                        return "";
                    var resp = res.inputFor;
                    $.each(val, function (i, o) {
                        resp += "\n" + "<a href='" + baseUrl + "Config/" + o.Code + "'>" + o.Name + "</a>";
                    });
                    return resp;
                }
            }
            ],
            ajaxGridOptions: { cache: false },
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
            height: 'auto',
            maxHeight: 250,
            rowNum: 20,
            gridview: false
        });
        $(gridTable).jqGrid('filterToolbar');
        $(gridTable).navGrid(gridPager,
        // the buttons to appear on the toolbar of the grid
        { edit: true, add: true, del: true, search: false, refresh: false, view: false, position: "left", cloneToTop: false },
        // options for the Edit Dialog
        EditOptions,
        // options for the Add Dialog
        {
            closeAfterAdd: true,
            recreateForm: true,
            //beforeSubmit: EditOptions.beforeSubmit,
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