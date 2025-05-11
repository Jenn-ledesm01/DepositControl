$(function () {
    Common = $('RepositoryEditJsData').data();
    var grids = $("table[data-RepositoryConfigId][data-RepositoryId]");
    var listData = new Array();
    var colModelData = {};

    if (grids.length === 0) return;


    $.each(grids, function (i) {
        debugger;
        var grid = $(grids[i]);
        var data = grid.data();
        listData.push(data);
    });
    
    $.ajax({
        url: Common.colmodeldataurl,
        global: false,
        type: "GET",
        data: "data=" + JSON.stringify(listData),
        dataType: "json",
        async: false,
        success: function (dataJson) {
            colModelData = dataJson;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status + " " + thrownError);
        }
    });

    $.each(grids, function (i) {
        var grid = $(grids[i]);
        var editable = grid.data().repositoryeditable == "true";

        var pagerId = "#" + grid.attr("id") + "-Pager";

        $("a[data-toggle=\"tab\"]").on("click", function (e) {
            grid.trigger("resize");
        });

        var dataColModel = colModelData[i];

        var colMod = dataColModel["ColModels"];
        var loadData = dataColModel["LoadData"];

        grid.jqGrid({
            //url: '',
            //postData: grid.data(),
            editurl: Common.editurl,
            datatype: "json",
            data: loadData,
            colModel: colMod,
            sortname: "Id",
            sortorder: "ASC",
            loadonce: true,
            sortable: true,
            viewrecords: true,
            width: 450,
            height: 350,
            rowNum: 20,
            rownumbers: false,
            shrinkToFit: false,
            forceFit: false,
            ignoreCase: true,
            pager: pagerId
        });

        //$.jgrid.loadState(grid.attr('id'));
        grid.navGrid(pagerId,
            // the buttons to appear on the toolbar of the grid
            { edit: editable, add: editable, del: editable, search: true, refresh: true, view: true, position: "left", cloneToTop: false },
            // options for the Edit Dialog
            {
                editCaption: res.edit,
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
                newCaption: res.new,
                closeAfterAdd: true,
                recreateForm: true,
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
});