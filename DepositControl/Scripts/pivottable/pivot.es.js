(function() {
  var callWithJQuery;

  callWithJQuery = function(pivotModule) {
    if (typeof exports === "object" && typeof module === "object") {
      return pivotModule(require("jquery"));
    } else if (typeof define === "function" && define.amd) {
      return define(["jquery"], pivotModule);
    } else {
      return pivotModule(jQuery);
    }
  };

  callWithJQuery(function($) {
    var c3r, d3r, frFmt, frFmtInt, frFmtPct, gcr, nf, r, tpl;
    nf = $.pivotUtilities.numberFormat;
    tpl = $.pivotUtilities.aggregatorTemplates;
    r = $.pivotUtilities.renderers;
    gcr = $.pivotUtilities.gchart_renderers;
    d3r = $.pivotUtilities.d3_renderers;
    c3r = $.pivotUtilities.c3_renderers;
    frFmt = nf({
      thousandsSep: ".",
      decimalSep: ","
    });
    frFmtInt = nf({
      digitsAfterDecimal: 0,
      thousandsSep: ".",
      decimalSep: ","
    });
    frFmtPct = nf({
      digitsAfterDecimal: 1,
      scaler: 100,
      suffix: "%",
      thousandsSep: ".",
      decimalSep: ","
    });
    $.pivotUtilities.locales.es = {
      localeStrings: {
        renderError: "Ocurri&oacute; un error durante la interpretaci&oacute;n de la tabla din&acute;mica.",
        computeError: "Ocurri&oacute; un error durante el c&acute;lculo de la tabla din&acute;mica.",
        uiRenderError: "Ocurri&oacute; un error durante el dibujado de la tabla din&acute;mica.",
        selectAll: "Seleccionar todo",
        selectNone: "Deseleccionar todo",
        tooMany: "(demasiados valores)",
        filterResults: "Filtrar resultados",
        totals: "Totales",
        vs: "vs",
        by: "por"
      },
      aggregators: {
        "Cuenta": tpl.count(frFmtInt),
        "Cuenta de valores &uacute;nicos": tpl.countUnique(frFmtInt),
        "Lista de valores &uacute;nicos": tpl.listUnique(", "),
        "Suma": tpl.sum(frFmt),
        "Suma de enteros": tpl.sum(frFmtInt),
        "Promedio": tpl.average(frFmt),
        "Mínimo": tpl.min(frFmt),
        "Máximo": tpl.max(frFmt),
        "Suma de sumas": tpl.sumOverSum(frFmt),
        "Cota 80% superior": tpl.sumOverSumBound80(true, frFmt),
        "Cota 80% inferior": tpl.sumOverSumBound80(false, frFmt),
        "Proporci&oacute;n del total (suma)": tpl.fractionOf(tpl.sum(), "total", frFmtPct),
        "Proporci&oacute;n de la fila (suma)": tpl.fractionOf(tpl.sum(), "row", frFmtPct),
        "Proporci&oacute;n de la columna (suma)": tpl.fractionOf(tpl.sum(), "col", frFmtPct),
        "Proporci&oacute;n del total (cuenta)": tpl.fractionOf(tpl.count(), "total", frFmtPct),
        "Proporci&oacute;n de la fila (cuenta)": tpl.fractionOf(tpl.count(), "row", frFmtPct),
        "Proporci&oacute;n de la columna (cuenta)": tpl.fractionOf(tpl.count(), "col", frFmtPct)
      },
      renderers: {
        "Tabla": r["Table"],
        "Tabla con barras": r["Table Barchart"],
        "Mapa de calor": r["Heatmap"],
        "Mapa de calor por filas": r["Row Heatmap"],
        "Mapa de calor por columnas": r["Col Heatmap"]
      }
    };
    if (gcr) {
      $.pivotUtilities.locales.es.gchart_renderers = {
        "Gr&aacute;fico de línea": gcr["Line Chart"],
        "Gr&aacute;fico de barras": gcr["Bar Chart"],
        "Gr&aacute;fico de barras apiladas": gcr["Stacked Bar Chart"],
        "Gr&aacute;fico de &aacute;rea": gcr["Area Chart"],
        "Gr&aacute;fico de dispersi&oacute;n": gcr["Scatter Chart"]
      };
    }
    if (d3r) {
      $.pivotUtilities.locales.es.d3_renderers = {
        "Mapa de &aacute;rbol": d3r["Treemap"]
      };
    }
    if (c3r) {
      $.pivotUtilities.locales.es.c3_renderers = {
        "Gr&aacute;fico de línea": c3r["Line Chart C3"],
        "Gr&aacute;fico de barras": c3r["Bar Chart C3"]
      };
    }
    return $.pivotUtilities.locales.es;
  });

}).call(this);

//# sourceMappingURL=pivot.es.js.map