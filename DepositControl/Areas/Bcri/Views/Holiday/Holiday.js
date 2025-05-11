$(document).ready(function () {
    var Common = $("#HolidayJsData").data();
    var gridTable = "#jqGridHoliday";
    var gridPager = gridTable + "Pager";

    var EditOptions = {
        editCaption: res.editUser,
        recreateForm: true,
        checkOnUpdate: false,
        checkOnSubmit: false,
        closeAfterEdit: true,
        beforeSubmit: function (postdata) {//validacion de duplicado (solo anda con data local)
            grid = $(this);
            var id = postdata[grid.attr("Id") + "_id"];
            if (isDateDuplicateInGrid(postdata, "Date", this)) {
                return [false, res.dateDuplicateError];
            }
            else
            {
                return [true];
            }
        },
        afterSubmit: function () {
            $(gridTable).jqGrid("setGridParam", { datatype: "json" });
            $(gridTable).trigger("reloadGrid");
            return [true];
        },
        viewPagerButtons: false,
        errorTextFormat: function (data) {
            return "Error: " + data.responseText;
        }
    }

    $(gridTable).jqGrid({
        url: Common.dataurl,
        editurl: Common.editurl,
        datatype: "json",
        autowidth: true,
        colModel: [
            {
                label: "Id",
                name: "Id",
                key: true,
                hidden: true
            },
            {
                    label: res.date,
                    name: "Date",
                    width: 110,                    
                    editable: true,
                    sorttype: 'date',
                    editrules: { required: true },
                    editoptions: {
                        size: 10, maxlengh: 10,
                        dataInit: function(element) {
                            $(element).datepicker();
                        },
                    },
                    formatter: function (cellValue) {
                            return moment(cellValue).format("L");
                }
            },
            {
                label: res.name,
                name: "Name",
                width: 250,
                edittype: "text",
                editable: true,
                editoptions: { maxlength: 250 },
                editrules: {
                    //custom rules
                    required: true
                }
            }
        ],
        sortname: "Id",
        sortorder: "DESC",
        loadonce: true,
        viewrecords: true,
        pager: gridPager
    });

    $(gridTable).navGrid(gridPager,
        // the buttons to appear on the toolbar of the grid
        { edit: Common.edit, add: Common.edit, del: Common.edit, del: true, search: true, refresh: true, view: false, position: "left", cloneToTop: false },
        // options for the Edit Dialog
        EditOptions,
        // options for the Add Dialog
        {
            closeAfterAdd: true,
            recreateForm: true,
            beforeSubmit: EditOptions.beforeSubmit,
            afterSubmit: EditOptions.afterSubmit,
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        // options for the Delete Dailog
        {
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        });
});
function isDateDuplicateInGrid(postData, column, grid) {
    grid = $(grid);
    var id = postData[grid.attr("Id") + "_id"];
    var value = postData[column].toLowerCase().trim();
    var allData = grid.jqGrid("getGridParam", "data");
   
    for (var i = 0; i < allData.length; i++) {

        var a = allData[i][column];
        var date1 = new Date(value);
        var date2 = a.substring(6, a.length - 5);
        var date = new Date(date2 * 1000);

        if (moment(date1).diff(moment(date), 'days') === 0 && id != allData[i].Id)
            return true;
    }
    return false;
}
