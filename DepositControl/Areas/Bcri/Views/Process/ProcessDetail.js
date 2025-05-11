$(document).ready(function () {
    $('[href="#inputs"]').on("shown.bs.tab", function () {
        var grid = $("#inputs").find(".active table[Id].ui-jqgrid-btable");
        if (typeof grid.resizeGrid === "function") {
            grid.resizeGrid();
        }
    });
    $('#tabInputs').on('click', function() {
        var grids = $('#inputs').find("table[data-entityname]");
        if (grids.length === 0) return;
        InitGrid(grids[0]);
    });

    //Los output aun usan los tabs
    $('a[data-toggle=tab]').on('click', function() {
        var href = $(this).attr('href');
        if (href.endsWith('-output')) {

            var griid = $(this).attr('href').replace('-output', '');

            InitGrid(griid);
        }        
    });

    $('#selRepositoryInputs').select2();
    $('#selRepositoryOutput').select2();
    $('#selRepositoryInputs, #selRepositoryOutput').on('change', function () {               
        var gid = $(this).find("option:selected").attr("grid-attach");        

        var realJqGridId = gid.replace('-input-', '').replace('-output-', '');
        InitGrid('#' + realJqGridId);

        

        if (gid.includes('-input-')) {
            $("#contentInputsGrid .panel-body.tab-pane.fade.in.active").removeClass("in active");
        } else {
            $("#contentOutputGrid .panel-body.tab-pane.fade.in.active").removeClass("in active");
        }

        $("#" + gid).addClass("in active");

        if (gid.includes('-input-')) {
            $("#" + realJqGridId).setGridWidth($("#contentInputsGrid").width());
        } else {
            $("#" + realJqGridId).setGridWidth($("#contentOutputGrid").width());            
        }


    });
});