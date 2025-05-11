using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Utility;

namespace DepositControl.Areas.Bcri
{
    public static class BcriConfig
    {
        public static void Register()
        {
            //mejora bindeo decimal y booleano para integracion con culturizacion 
            ModelBinders.Binders.Add(typeof(decimal), new DecimalBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalNullableBinder());

            ModelBinders.Binders.Add(typeof(bool), new BoolBinder());
            ModelBinders.Binders.Add(typeof(bool?), new BoolNullableBinder());

            //mejora parseo para bcp y report y sobreescribe ConvertfromString del TypeConverter
            TypeDescriptor.AddAttributes(typeof(decimal), new TypeConverterAttribute(typeof(DecimalConverterEx)));
            TypeDescriptor.AddAttributes(typeof(double), new TypeConverterAttribute(typeof(DoubleConverterEx)));

            TypeDescriptor.AddAttributes(typeof(bool), new TypeConverterAttribute(typeof(BooleanConverterEx)));
            TypeDescriptor.AddAttributes(typeof(bool?), new TypeConverterAttribute(typeof(BooleanConverterEx)));

            //aca abria que agregar mediante el config los Actionfilters, para controlar login y seguridad en general 

            var bundles = BundleTable.Bundles;

            bundles.Add(new ScriptBundle("~/bundles/bcriVendor").Include(
               "~/Scripts/jquery-2.2.4.min.js",
               "~/Scripts/jquery-ui-{version}.js",
               "~/Scripts/jquery-ui-i18n.js",
               "~/Scripts/jszip.js",
               //$"~/Scripts/i18n/grid.locale-{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.js",
               "~/Scripts/bootstrap.js",
               "~/Scripts/metisMenu.js",
               "~/Scripts/respond.js",
               "~/Scripts/modernizr-*",
               "~/Scripts/jquery.jqGrid.js",
               "~/Scripts/jquery.maskedinput.min.js",
               "~/Scripts/jquery.validate.js",
               "~/Scripts/jquery.validate.unobtrusive.js",
               "~/Scripts/jquery.multiselect.min.js",
               "~/Scripts/moment-with-locales.js",
               "~/Scripts/bootstrap-dialog.js",
               "~/Scripts/bootstrap-datetimepicker.js",
               "~/Scripts/bootstrap-multiselect.js",
               "~/Scripts/jquery.fileDownload.js",
               "~/Scripts/select2.js"
            ));

            //todas las librerias de este bundle ya estan minificadas, por eso la clase es Bundle y no ScriptBundle
            //si se ponen en un ScriptBundle el minificado da error
            //tambien los js del nuget de DevExtreme no tienen .min y eso causa problemas
            bundles.Add(new Bundle("~/bundles/bcriVendorDxEj").Include(
                "~/Scripts/cldr.js",
                "~/Scripts/cldr/event.js",
                "~/Scripts/cldr/supplemental.js",
                "~/Scripts/globalize.js",
                "~/Scripts/globalize/message.js",
                "~/Scripts/globalize/number.js",
                "~/Scripts/globalize/currency.js",
                "~/Scripts/globalize/date.js",
                "~/Scripts/dx.viz-web.js"//,
                //"~/Scripts/devextreme-localization/dx.messages.de.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bcri").Include(
                "~/Scripts/bcri/comments.js",
                "~/Scripts/bcri/GridCustomView.js",
                "~/Scripts/bcri/JqGridBcri.js",
                "~/Scripts/template.js",
                "~/Scripts/jquery.slimscroll.min.js",
                "~/Scripts/pace.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/bcriCss").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/metisMenu.css",
                "~/Content/jqgrid.ui.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/bootstrap-dialog.css",
                "~/Content/bootstrap-multiselect.css",
                "~/Content/jquery.ui.css",
                "~/Content/dx.common.css",
                "~/Content/dx.light.compact.css",
                "~/Content/ejthemes/ej.theme.min.css",
                "~/Content/ejthemes/ej.widgets.core.min.css",
                "~/Content/animate.css",
                "~/Content/TemplateBCRI.css",
                "~/Content/BCRI.css",
                "~/Content/style-overrides.css",
                //"~/PhantomJS/tools/phantomjs/Exporter/dx.exporter.light.css"
                "~/Content/css/select2.min.css"
                ));

            /*dejo esto por si ahi que seaparar el jqVal del site bundle */
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive*"));//,

            /*TreeView Security*/
            bundles.Add(new ScriptBundle("~/bundles/treeview").Include(
                "~/Scripts/bootstrap-treeview.js"));
            bundles.Add(new StyleBundle("~/Content/cssTreeview").Include(
                "~/Content/bootstrap-treeview.css"));

            /*Pivot*/
            bundles.Add(new StyleBundle("~/Content/cssPivot").Include(
                "~/Content/pivot.css",
                "~/Content/pivotTable.css"));

            bundles.Add(new ScriptBundle("~/bundles/pivot").Include(
                "~/Scripts/pivottable/pivot.js",
                "~/Scripts/pivottable/gchart_renderers.js",
                "~/Scripts/pivottable/jsapi.js"));

            /*Login*/
            bundles.Add(new StyleBundle("~/Content/cssLogin").Include(
               "~/Content/bootstrap.css",
               "~/Content/TemplateBCRI.css",
               "~/Content/font-awesome.css"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                "~/Scripts/jquery-2.2.4.min.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/popper.min.js"
            ));

            /*D3 */
            bundles.Add(new ScriptBundle("~/bundles/d3").Include(
                "~/Scripts/d3.v3.min.js",
                "~/Scripts/d3.tip.v0.6.3.js"));
        }
    }
}

