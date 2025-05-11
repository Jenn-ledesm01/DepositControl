using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace System.Web.Mvc
{
    /// <summary>
    /// Mvc extensions for dynamic CSS and JS
    /// </summary>
    public static class MvcExtensions
    {
        /// <summary>
        /// CSS content result rendered by partial view specified
        /// </summary>
        /// <param name="controller">current controller</param>
        /// <param name="cssViewName">view name, which contains partial view with one STYLE block only</param>
        /// <param name="model">optional model to pass to partial view for rendering</param>
        /// <returns></returns>
        public static ActionResult CssFromView(this Controller controller, string cssViewName = null, object model = null)
        {
            var cssContent = ParseViewToContent(controller,cssViewName, "style", model);
            if (cssContent==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "CSS not found");
            }
            return new ContentResult() { Content = cssContent, ContentType = "text/css" };
        }

        /// <summary>
        /// Javascript content result rendered by partial view specified
        /// </summary>
        /// <param name="controller">current controller</param>
        /// <param name="javascriptViewName">view name, which contains partial view with one SCRIPT block only</param>
        /// <param name="model">optional model to pass to partial view for rendering</param>
        /// <returns></returns>
        public static ActionResult JavaScriptFromView(this Controller controller, string javascriptViewName=null, object model=null)
        {
            var jsContent = ParseViewToContent(controller,javascriptViewName, "script", model);
            if (jsContent==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "JS not found");
            }
            return new JavaScriptResult() {
                Script = jsContent
            };
        }

        /// <summary>
        /// Parse view and render it to a string, trimming specified HTML tag
        /// </summary>
        /// <param name="controller">controller which renders the view</param>
        /// <param name="viewName">name of cshtml file with content. If null, then actionName used</param>
        /// <param name="tagName">Content rendered expected to be wrapped with this html tag, and it will be trimmed from result</param>
        /// <param name="model">model to pass for view to render</param>
        /// <returns></returns>
        static string ParseViewToContent(Controller controller, string viewName, string tagName, object model = null)
        {
            using (var viewContentWriter = new StringWriter())
            {
                if (model != null)
                {
                    controller.ViewData.Model = model;
                }

                if (string.IsNullOrEmpty(viewName))
                {
                    viewName = controller.RouteData.GetRequiredString("action");
                }

                var viewResult = new ViewResult()
                {
                    ViewName = viewName,
                    ViewData = controller.ViewData,
                    TempData = controller.TempData,
                    ViewEngineCollection = controller.ViewEngineCollection
                };

                var viewEngineResult = controller.ViewEngineCollection.FindPartialView(controller.ControllerContext, viewName);
                if (viewEngineResult.View == null)
                {
                    return null;
                }

                try
                {
                    var viewContext = new ViewContext(controller.ControllerContext, viewEngineResult.View, controller.ViewData, controller.TempData, viewContentWriter);
                    viewEngineResult.View.Render(viewContext, viewContentWriter);
                    var viewString = viewContentWriter.ToString().Trim('\r', '\n', ' ');
                    var regex = string.Format("<{0}[^>]*>(.*?)</{0}>", tagName);
                    var res = Regex.Match(viewString, regex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Singleline);
                    if (res.Success && res.Groups.Count > 1)
                        return res.Groups[1].Value;
                    else throw new InvalidProgramException(string.Format("Dynamic content produced by viewResult '{0}' expected to be wrapped in '{1}' tag", viewName, tagName));
                }
                finally
                {
                    if (viewEngineResult.View != null)
                        viewEngineResult.ViewEngine.ReleaseView(controller.ControllerContext, viewEngineResult.View);
                }
            }

        }

    }
}
