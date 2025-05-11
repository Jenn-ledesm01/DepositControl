/// <reference path="Audit.cshtml" />
var Common = null;
$.ajaxSetup({ cache: false, async: false });

$.datepicker.setDefaults($.datepicker.regional["es"]);
 $.fn.bootstrapBtn = $.fn.button.noConflict();


$("#filterFromFilters").datepicker("setDate", new Date());
$("#filterToFilters").datepicker("setDate", new Date());

$("#filterFromFilters").datepicker({
    onSelect: function (selected) {
        $("#filterToFilters").datepicker("option", "minDate", selected)
    }
});

$("#filterToFilters").datepicker({
    onSelect: function (selected) {
        $("#filterFromFilters").datepicker("option", "maxDate", selected)
    }
});

Common = $("#AuditJsData").data();

var CallView = function (nLogAccion) {
    $.ajax({
        type: 'GET',
        url: Common.getdetallelog + '?idLogAccion=' + nLogAccion,
        success: function (resultPartial) {
            $('#dialog-confirm-body').html(resultPartial);
           
            $("#dialog-confirm").dialog({
                resizable: false,
                height: "auto",
                width: 850,
                modal: true,
                position: { my: "center center", at: "center+120 center", of: window }
            });
        }
    });
}
 function loadAccions()
        {
            var typeAccionId = $("#TypeAccion").val();
            $.ajax({
                type: 'GET',
                url: Common.geturl + '?id=' + typeAccionId,
                success: function (result) {
                    console.log(result);
                    if (result.length > 0)
                    {
                        $('#ddAccion').html('');
                        $('#ddAccion').multiselect('destroy');

                        $.each(result, function (i, accion) {
                            $('#ddAccion').append('<option value="' + accion.Value + '">' +
                                accion.Text + '</option>');
                        });
                        $("#ddAccion").multiselect({
                            selectAllValue: 'multiselect-all',
                            includeSelectAllOption: true,
                            enableCaseInsensitiveFiltering: false,
                            enableFiltering: false,
                            maxHeight: '300',
                            buttonWidth: '235',
                            numberDisplayed: 1,
                            allSelectedText: res.allSelectedText,
                            selectAllText: res.selectAllText,
                            buttonText: function (options) {
                                if (options.length == 0) {
                                    totalSelected = 0;
                                    return res.nonSelectedText;
                                }
                                if (options.length == result.length) {
                                    totalSelected = 1;
                                    return res.allSelectedText;
                                } else {
                                    totalSelected = 0;
                                    return options.length + " " + res.selected;
                                }
                            },
                            onSelectAll: function () {
                                totalSelected = 1;
                            },
                            onDeselectAll: function () {
                                totalSelected = 0;
                            }
                        });

                        $("#ddAccion").multiselect('refresh');
                    }else
                    {
                        $('#ddAccion').html('');
                        $('#ddAccion').multiselect('destroy')
                        $("#ddAccion").multiselect({
                            selectAllValue: 'multiselect-all',
                            includeSelectAllOption: true,
                            enableCaseInsensitiveFiltering: false,
                            enableFiltering: false,
                            maxHeight: '300',
                            buttonWidth: '235',
                            numberDisplayed: 1,
                            allSelectedText: res.allSelectedText,
                            selectAllText: res.selectAllText,
                            buttonText: function (options) {
                                if (options.length == 0) {
                                    totalSelected = 0;
                                    return res.nonSelectedText;
                                }
                                if (options.length == result.length) {
                                    totalSelected = 1;
                                    return res.allSelectedText;
                                } else {
                                    totalSelected = 0;
                                    return options.length + " " + res.selected;
                                }
                            },
                            onSelectAll: function () {
                                totalSelected = 1;
                            },
                            onDeselectAll: function () {
                                totalSelected = 0;
                            }
                        });
                        $("#ddAccion").multiselect('refresh');
                        $("#ddAccion").multiselect('disable');
                    }
                    
                }, error: function (ex) { alert(ex.responseText) }
            });
        
        }
$(document).ready(function () {
    InitForm("#filterDiv");
    $.datepicker.setDefaults($.datepicker.regional["es"]);
    $("#filterFromFilters").datepicker("setDate", new Date());
    $("#filterToFilters").datepicker("setDate", new Date());
    $("#dashBoardContent").hide();
    $("#buttonExport").hide();
    var totalSelected = 0;
    loadAccions();
  
    $("#ddAccion").multiselect({
        selectAllValue: 'multiselect-all',
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: false,
        enableFiltering: false,
        maxHeight: '300',
        buttonWidth: '235',
        numberDisplayed: 1,
        allSelectedText: res.allSelectedText,
        selectAllText: res.selectAllText,
        buttonText: function (options) {
            if (options.length == 0) {
                totalSelected = 0;
                return res.nonSelectedText;
            }
            if (options.length == result.length) {
                totalSelected = 1;
                return res.allSelectedText;
            } else {
                totalSelected = 0;
                return options.length + " " + res.selected;
            }
        },
        onSelectAll: function () {
            totalSelected = 1;
        },
        onDeselectAll: function () {
            totalSelected = 0;
        }


    });
       
        $("#TypeAccion").change(function () {          
            loadAccions();          
        });

       
          
   
    $("#btnFilter").click(function () {
        $("#dashBoardContent").hide();
        $("#buttonExport").hide();
        $("#progressBar").show(function () {
            var fromDateParseParcial = $("#filterFromFilters").datepicker("getDate");
            var toDateParseParcial = $("#filterToFilters").datepicker("getDate");
            if (fromDateParseParcial == null) {
                $("#MsgError").showAlert("<strong>Información!</strong> " + 'Error al solicitar efectuar la búsqueda: no ha ingresado Fecha Desde. Por favor, ingrese Fecha Desde e intentelo nuevamente.', 'error');
                 $("#progressBar").hide();
                $("#dashBoardContent").hide();
                $("#buttonExport").hide();
            }
            else {
                if (toDateParseParcial == null) {
                    $("#MsgError").showAlert("<strong>Información!</strong> " + 'Error al solicitar efectuar la búsqueda: no ha ingresado Fecha Hasta. Por favor, ingrese Fecha Hasta e intentelo nuevamente.', 'error');
                    $("#progressBar").hide();
                    $("#dashBoardContent").hide();
                    $("#buttonExport").hide();
                } else {
                    var acciones = obtenerDdAcciones();
                    if (acciones == '') {
                        $("#MsgError").showAlert("<strong>Información!</strong> " + 'Error al solicitar efectuar la búsqueda: no ha seleccionado al menos 1 acción. Por favor, seleccione una acción e intentelo nuevamente.', 'error');
                        $("#progressBar").hide();
                         $("#dashBoardContent").hide();
                         $("#buttonExport").hide();
                    } else {
                        filter();
                    }
                }
            }
        });
    });


    function filter() {
        var actParse = $("#selectAct").val();
        //alert(actParse);
        var fromDateParseParcial = $("#filterFromFilters").val();
        var toDateParseParcial = $("#filterToFilters").val();

        var DesdeTotal = fromDateParseParcial.split('/');
        var diaDesde = DesdeTotal[0];
        var mesDesde = DesdeTotal[1];
        var anoDesde = DesdeTotal[2];

        var hastaTotal = toDateParseParcial.split('/');
        var diaHasta = hastaTotal[0];
        var mesHasta = hastaTotal[1];
        var anoHasta = hastaTotal[2];

        var fromDateParse = anoDesde + "-" + mesDesde + "-" + diaDesde;
        var toDateParse = anoHasta + "-" + mesHasta + "-" + diaHasta;

        var urlGrid = Common.loaddataurl + "?fromDate=" + fromDateParse + "&toDate=" + toDateParse + "&act01=" + actParse + "&all=" + totalSelected;

        gridReload(actParse, fromDateParse, toDateParse, urlGrid);

    }

   
    $("#progressBar").progressbar({
        value: false
    });
    function progress() {
        var val = $("#progressBar").progressbar("value") || 0;
        $("#progressBar").progressbar("value", val + 1);
        if (val < 99) {
            setTimeout(progress, 100);
        }
    }
});


function gridReload(actParse, fromDateParse, toDateParse, urlGrid) {
    var jsonTemp = obtenerCondition(fromDateParse, toDateParse);


    $.ajax({
        url: urlGrid,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonTemp),
        success: function (result) {
            if (result.rows.length > 0) {
                 var listColModel = [];
                var tam = $("#filterDiv").width();
                for (var i = 0; i < result.ColModel.length; i++) {
                    listColModel.push({
                        name: result.ColModel[i].Name,
                        index: result.ColModel[i].index,
                        resizable: false,
                        sortable: false,
                        align: "left",
                        width: tam / 5
                    });
                }
                var colNames = result.ColNames;
                actualizarjQGrid(listColModel, colNames, result.rows, result.groupHeader, result.userdata, tam);
                $("#progressBar").hide();
                $("#dashBoardContent").show();
                $("#buttonExport").show();
            }else
            {
                $("#MsgError").showAlert("<strong>Información!</strong> " + 'No existen registros en log correspondientes a los filtros ingresados.', 'error');
                 $("#progressBar").hide();
                 $("#dashBoardContent").hide();
                 $("#buttonExport").hide();
                
            } 
        }
    });
};

function obtenerCondition(fromDateParse, toDateParse) {
    var jsonTemp = {
        fromDate: fromDateParse,
        toDate: toDateParse,
        act01: obtenerDdAcciones()  
    };

    return jsonTemp;
};



function actualizarjQGrid(listColModel, colNames, datos, groupHeader, footerData, tam) {
    $.jgrid.gridUnload("#gridAction"); 

    var gridPager = "#gridActionPager";

    var fromDateParseParcial = $("#filterFromFilters").val();
    var toDateParseParcial = $("#filterToFilters").val();

    var DesdeTotal = fromDateParseParcial.split('/');
    var diaDesde = DesdeTotal[0];
    var mesDesde = DesdeTotal[1];
    var anoDesde = DesdeTotal[2];

    var hastaTotal = toDateParseParcial.split('/');
    var diaHasta = hastaTotal[0];
    var mesHasta = hastaTotal[1];
    var anoHasta = hastaTotal[2];

    var fromDateParse = anoDesde + "-" + mesDesde + "-" + diaDesde;
    var toDateParse = anoHasta + "-" + mesHasta + "-" + diaHasta;
    
    var urlGrid = Common.loaddataurl
        + "?fromDate=" + fromDateParse
        + "&toDate=" + toDateParse
        + "&act01=" + obtenerDdAcciones();

    $("#gridAction").jqGrid({

        url: urlGrid,
        datatype: 'json',
        myType: 'GET',
        height: 'auto',
        shrinkToFit: true,
        forceFit: true,
        autowidth: true,
        rowNum: 20,
        rownumbers: false,
        colNames: colNames,
        colModel: listColModel,
        pager: gridPager,
        viewrecords: true,
        gridview: true,
        ignoreCase: true,
        //loadonce: true,
        sortname: 'Date',
        sortorder: 'DESC',
        rowList: [20, 50, 100]
    });

    $("#gridAction").navGrid(gridPager,
            { edit: false, add: false, del: false, search: false, refresh: false, view: false });

    var tamañoGrilla = $("#filterDiv").width();

    $(window).bind("resize", function () {
        var grid = $("#gridAction");
        grid.jqGrid("setGridWidth", (grid.parent().parent().parent().parent().parent().width()) <= tamañoGrilla ? (grid.parent().parent().parent().parent().parent().width()) : tamañoGrilla);
    }).trigger("resize");

};
