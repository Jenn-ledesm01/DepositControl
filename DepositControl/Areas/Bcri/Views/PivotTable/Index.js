var Common = $("#PivotTableJsData").data();
var forExport = null;
var globalData = null;
var validFormQueryDiv = null;

// load google visualization API
google.load("visualization", "1", { packages: ["corechart", "charteditor"], 'language': "en" });
var renderers = $.extend($.pivotUtilities.renderers, $.pivotUtilities.gchart_renderers);

$.ajaxSetup({ cache: false, async: false });

$(document).ready(function () {
    //Datepicker
    $.datepicker.setDefaults($.datepicker.regional["en"]);
    $.datepicker.setDefaults({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy"
    });
    $("#filterDateFromFilters").datepicker({
        onClose: function (selectedDate) {
            $("#filterDateToFilters").datepicker("option", "minDate", selectedDate);
        }
    });
    $("#filterDateToFilters").datepicker({
        onClose: function (selectedDate) {
            $("#filterDateFromFilters").datepicker("option", "maxDate", selectedDate);
        }
    });

    //Events
    $("#selectDateRangeFilters").change(function () {
        $("#optPeriod").attr("checked", true);
    });
    $("#btnFilter").click(function () {
        filter();
    });
    $("#btnClearFilter").click(function () {
        cleanFilters();
    });
    $("#btnAddQuery").click(function () {
        $("#inputNameNewQuery").val("");
        $("#divLoadModeQuery").hide();
        $("#divEditModeQuery").show();
    });
    $("#btnSaveQuery").click(function () {
        saveQuery();
    });
    $("#btnSaveCurrentQuery").click(function () {
        saveCurrentQuery();
    });
    $("#btnCancelSaveQuery").click(function () {
        $("#divLoadModeQuery").show();
        $("#divEditModeQuery").hide();
        $("#inputNameNewQuery").val("");
    });
    $("#btnDeleteQuery").click(function () {
        deleteQuery();
    });
    $("#selectSavedQueries").change(function () {
        var value = $(this).val();
        if (value !== "0") {
            loadQuery();
            //$("#btnAddQuery").hide();
            $("#divBtnSaveDeleteQuery").show();
        } else {
            //$("#btnAddQuery").show();
            //$("#divBtnSaveDeleteQuery").hide();

            $("#outputPivotTable").pivotUI(globalData,
            {
                renderers: renderers,
                onRefresh: function (config) {
                    getState(config);
                }
            }, true);
        }
    });

    //Validation    
    $.validator.addMethod("queryExist", function (value, element) {
        var input = value.trim();
        var e = 0;

        if (input != null) {
            var alphaReg = /^[a-z0-9\-\_\s]+$/i;
            if (alphaReg.test(input)) {
                $.ajax({
                    url: Common.checkqueryurl,
                    global: false,
                    type: "POST",
                    data: "nameQuery=" + input,
                    dataType: "json",
                    async: false,
                    success: function (dataJson) {
                        if (dataJson !== true) {
                            e++;
                            $.validator.messages.queryExist = dataJson;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log(xhr.status + " " + thrownError);
                    }
                });
            } else {
                e++;
                $.validator.messages.queryExist = res.validatorMessageNameLetters;
            }
        }

        return this.optional(element) || e === 0;
    }, $.validator.messages.queryExist);
    validFormQueryDiv = $("#valid-form-queryDiv").validate({
        rules: {
            inputNameNewQuery: {
                required: true,
                queryExist: true,
                maxlength: 35,
                minlength: 1
            }
        }
    });

    cleanFilters();
    //filter(); //BORRAR
});

function getCurrentDate() {
    var currentDate = new Date();
    return getFormatedDate(currentDate);;
}
function getDateParse() {
    var dateDesde = new Date();
    dateDesde.setMonth(dateDesde.getMonth() - 1);
    return getFormatedDate(dateDesde);
}
function getFormatedDate(date) {
    //var convertDate = date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
    var convertDate = $.datepicker.formatDate("mm/dd/yy", date);
    return convertDate;
}

function cleanFilters() {
    $("#divLoadModeQuery").show();
    $("#divEditModeQuery").hide();

    $("#btnAddQuery").show();
    //$("#divBtnSaveDeleteQuery").hide();

    //loadQueryInfo();
    $("#filterDateFromFilters").val(getDateParse());
    $("#filterDateToFilters").val(getCurrentDate());

    $("#selectDataSourceFilters").val(5); //Trade Blotter Developers (5) - Abi File (13)
    $("#selectDateRangeFilters").val("lastYear"); //lastYear //lastMonth
    $("#radioFilterPeriod").prop("checked", true);

    $("#inputNameNewQuery").val("");

    $("#progressBar").hide();
    $("#dashBoard").hide();

    $("#btnToggleFilters").click();

    forExport = false;
}
function filter() {
    $("#progressBar").hide();
    $("#dashBoard").hide();

    //$("#selectDataSourceFilters").val(13); //Trade Blotter Developers (5) - Abi File (13)
    var selectDataSource = $("#selectDataSourceFilters").val();
    var optionFilter = $("input:radio[name=radioFilter]:checked").val();
    var sLink = "";

    if (optionFilter === "optDate") {
        var sFec1 = $("#filterDateFromFilters").val();
        var sFec2 = $("#filterDateToFilters").val();
        sLink = Common.loaddataurl + "?dataSource=" + selectDataSource
            + "&dateStart=" + sFec1
            + "&dateEnd=" + sFec2;
    } else if (optionFilter === "optPeriod") {
        var sPeriod = $("#selectDateRangeFilters").val();
        sLink = Common.loaddataurl + "?dataSource=" + selectDataSource
            + "&dateRange=" + sPeriod;
    } else {
        alert(res.alertNotSelectedOption);
        return;
    }

    $("#progressBar").show();

    $.getJSON(sLink, function (data) {
        if (data.length > 0) {
            loadQueryInfo();

            globalData = data;

            $("#outputPivotTable").pivotUI(data,
            {
                renderers: renderers,
                onRefresh: function (config) {
                    getState(config);
                }
            }, true);

            $("#progressBar").hide();
            $("#dashBoard").show();

            $("#btnToggleFilters").click();
            //$(".collape-button i").click();

            forExport = true;
        } else {
            if (data.Redirect) {
                window.location = data.Redirect;
            } else {
                $("#progressBar").hide();
                $("#dashBoard").hide();

                forExport = false;

                alert(res.noDataAvaliable);
            }
        }
    }).fail(function (jqXhr, textStatus, errorThrown) {
        $("#progressBar").hide();
        $("#dashBoard").hide();

        forExport = false;

        console.error(textStatus + " - " + errorThrown);
        alert(res.couldNotPerformOp);

        return;
    });
}

function loadQueryInfo() {
    var selectDataSource = $("#selectDataSourceFilters").val();
    if (selectDataSource !== "0") {
        var sLink = Common.loadqueryinfourl + "?idDataSource=" + selectDataSource;
        $.getJSON(sLink, null,
            function (data) {
                $("#selectSavedQueries").empty();
                $.each(data, function () {
                    $("#selectSavedQueries").append($("<option />").val(this.Value).text(this.Text));
                });
            }).fail(function (jqXhr, textStatus, errorThrown) {
                console.error(textStatus + " - " + errorThrown);
                alert(res.couldNotPerformOp);
            });
    }
}+
function loadQuery() {
    var selectedQuery = $("#selectSavedQueries").val();
    if (selectedQuery !== "0") {
        $.ajax({
            url: Common.loadqueryurl,
            global: false,
            type: "POST",
            data: "idSelectedQuery=" + selectedQuery,
            dataType: "json",
            success: function (data) {
                //var objectJson = JSON.stringify(data[0], undefined, 2);
                var objectJsonParse = JSON.parse(JSON.stringify(data[0]));
                //console.log(config[0]);
                //console.log(objectJson);
                //alert(objectJson);
                //alert(JSON.stringify(objectJsonParse, undefined, 2));
                var stateLoaded = setState(objectJsonParse);
                $("#outputPivotTable").pivotUI(globalData, stateLoaded, true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.status + " " + thrownError);
                alert(res.couldNotPerformOp);
            }
        });
    }
}
function saveQuery() {
    var nameNewQuery = $("#inputNameNewQuery").val();
    var currentConfigQuery = $("#configPivotTable").val();

    var selectDataSource = $("#selectDataSourceFilters").val();
    if (selectDataSource !== "0") {
        if ($("#valid-form-queryDiv").valid()) {
            if (nameNewQuery !== "" && currentConfigQuery !== "") {
                var configCopy = JSON.parse(currentConfigQuery);
                var objectJson = JSON.stringify(configCopy);
                var data = {
                    idDataSource: selectDataSource,
                    idSelectedQuery: 0,
                    nameNewQuery: nameNewQuery,
                    dataNewQuery: objectJson
                }
                $.ajax({
                    url: Common.savequeryurl,
                    global: false,
                    type: "POST",
                    data: "dataJson=" + JSON.stringify(data),
                    dataType: "json",
                    success: function (data) {
                        loadQueryInfo();
                        $("#selectSavedQueries").val(data);
                        console.log("Data saved correctly.");
                        //alert("Data saved correctly.");
                        $("#divLoadModeQuery").show();
                        $("#divEditModeQuery").hide();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log(xhr.status + " " + thrownError);
                        alert(res.couldNotPerformOp);
                    }
                });
            }
        }
    }
}
function saveCurrentQuery() {
    var selectedQuery = $("#selectSavedQueries").val();
    var currentConfigQuery = $("#configPivotTable").val();
    var configCopy = JSON.parse(currentConfigQuery);
    var objectJson = JSON.stringify(configCopy);

    var selectDataSource = $("#selectDataSourceFilters").val();
    if (selectDataSource !== "0") {
        if (selectedQuery !== "0" && currentConfigQuery !== "") {
            var data = {
                idDataSource: selectDataSource,
                idSelectedQuery: selectedQuery,
                nameNewQuery: "",
                dataNewQuery: objectJson
            }
            $.ajax({
                url: Common.savequeryurl,
                global: false,
                type: "POST",
                data: "dataJson=" + JSON.stringify(data),
                dataType: "json",
                success: function (data) {
                    loadQueryInfo();
                    $("#selectSavedQueries").val(data);
                    console.log("Data saved correctly.");
                    //alert("Data saved correctly.");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log(xhr.status + " " + thrownError);
                    alert(res.couldNotPerformOp);
                }
            });
        } else {
            alert(res.pleaseSelectReport);
        }
    }
}
function deleteQuery() {
    var selectedQuery = $("#selectSavedQueries").val();
    if (selectedQuery > 0) {
        $.ajax({
            url: Common.deletequeryurl,
            global: false,
            type: "POST",
            data: "idSelectedQuery=" + selectedQuery,
            dataType: "json",
            success: function (data) {
                loadQueryInfo();
                console.log("Data deleted correctly.");
                //$("#divBtnSaveDeleteQuery").hide();
                $("#btnAddQuery").show();
                $("#outputPivotTable").pivotUI(globalData,
                {
                    renderers: renderers,
                    onRefresh: function (config) {
                        getState(config);
                    }
                }, true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.status + " " + thrownError);
                alert(res.couldNotPerformOp);
            }
        });
    } else {
        alert(res.pleaseSelectReport);
    }
}

function getState(config) {
    var configCopy = JSON.parse(JSON.stringify(config));
    //delete some values which are functions
    delete configCopy["aggregators"];
    delete configCopy["renderers"];
    delete configCopy["derivedAttributes"];
    //delete some bulky default values
    delete configCopy["rendererOptions"];
    delete configCopy["localeStrings"];
    var objectJson = JSON.stringify(configCopy, undefined, 2);
    $("#configPivotTable").text(objectJson);
}
function setState(configCopy) {
    var defaults = {
        aggregatorName: "",
        rendererName: "",
        //derivedAttributes: {},
        //aggregators: null,//locales[locale].aggregators,
        //renderers: null,//locales[locale].renderers,
        renderers: renderers,
        hiddenAttributes: [],
        menuLimit: 200,
        cols: [],
        rows: [],
        vals: [],
        exclusions: {},
        inclusions: {},
        unusedAttrsVertical: 85,
        autoSortUnusedAttrs: false,
        //rendererOptions: {
        //    localeStrings: locales[locale].localeStrings
        //},
        //onRefresh: null//,
        onRefresh: function (config) {
            getState(config);
        }
        //filter: function () {
        //    return true;
        //},
        //sorters: function () { },
        //localeStrings: locales[locale].localeStrings
    };
    $.each(defaults, function (keyDef, valueDef) {
        $.each(configCopy, function (keyConfig, valueConfig) {
            if (keyDef === keyConfig) {
                switch (keyDef) {
                    case "exclusions":
                    case "inclusions":
                    case "inclusionsInfo":
                        if (valueConfig)
                            defaults[keyDef] = JSON.parse(valueConfig);
                        break;
                    default:
                        defaults[keyDef] = valueConfig;
                }
            }
            //if (valueConfig === "" || valueConfig === null) {
            //    //delete configCopy[keyConfig];
            //} else if (typeof (valueConfig) === "object") {
            //    //configCopy[keyConfig] = removeAllBlankOrNull(value);
            //}
        });
    });

    return defaults;
}