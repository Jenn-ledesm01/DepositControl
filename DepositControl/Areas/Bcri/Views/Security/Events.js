$(document).ready(function () {
    var Common = $("#EventsJsData").data();

    var gridTable = "#jqGridEvents";
    var gridPager = gridTable + "Pager";

    var gridData = [];
    var colModelData = {};

    $.ajax({
        url: Common.dataurl,
                global: false,
                type: "GET",
                //data: "data=" + JSON.stringify(listData),
                dataType: "json",
                async: false,
                success: function (dataJson) {
                    colModelData = dataJson;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status + " " + thrownError);
                }
    });

    $("#jqGridEvents").dxDataGrid({
        dataSource: colModelData,
        columns: [            
             {   
                dataField: 'Name',
                 caption: 'Nombre Accion',
             },
             {
                dataField: 'Code',
                 caption: 'Codigo',
                // width: 110,
                            
        }],

        paging: {
            pageSize: 10
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        headerFilter: {
            visible: false
        },
        groupPanel: {
            visible: true
        },
        allowColumnReordering: true,
        allowColumnResizing: true,
        export: {
            enabled: true,
            fileName: "Eventos auditables"
        },
        showBorders: true
    }).dxDataGrid("instance");
    $.extend($.jgrid, $.jgrid.regional["es"]);
    
    //$(gridTable).jqGrid({
    //    url: Common.dataurl,
    //    datatype: "json",
    //    autowidth: true,
    //    colModel: [
    //        {
    //            label: "Id",
    //            name: "Id",
    //            key: true,
    //            hidden: true
    //        },
    //        {
    //            label: "Name",
    //            name: "Name",
    //            width: 100,
    //            edittype: "text",
    //            editable: false,
    //            editoptions: { maxlength: 100 },
    //            editrules: {
    //                //custom rules
    //                required: true
    //            }
    //        },
    //        {
    //            label: "Code",
    //            name: "Code",
    //            width: 220,
    //            edittype: "text",
    //            editable: false,
    //            editoptions: { maxlength: 250 },
    //            editrules: { required: true }
    //        }
    //    ],
    //    sortname: "Name",
    //    sortorder: "ASC",
    //    loadonce: true,
    //    viewrecords: true,
    //    pager: gridPager
    //});

    //$(gridTable).navGrid(gridPager,
    //    // the buttons to appear on the toolbar of the grid
    //    { edit: false, add: false, del: false, search: true, refresh: true, view: false, position: "left", cloneToTop: false },
    //    // options for the Edit Dialog
    //    //EditOptions,
    //    //// options for the Add Dialog
    //    //{
    //    //    closeAfterAdd: true,
    //    //    recreateForm: true,
    //    //    beforeSubmit: EditOptions.beforeSubmit,
    //    //    afterSubmit: EditOptions.afterSubmit,
    //    //    errorTextFormat: function (data) {
    //    //        return "Error: " + data.responseText;
    //    //    }
    //    //},
    //    // options for the Delete Dailog
    //    {
    //        errorTextFormat: function (data) {
    //            return "Error: " + data.responseText;
    //        }
    //    });
});
