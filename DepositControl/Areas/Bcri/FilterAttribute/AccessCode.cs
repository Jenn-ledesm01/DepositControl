using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DNF.Security.Bussines;

namespace DepositControl
{
    public class AccessCode : ActionFilterAttribute
    {
        public string Code { get; set;  }
        public AccessCode(string code)
        {
            Code = code;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((Current.User != null) && (!string.IsNullOrEmpty(Code)))
            {
                if (!Current.User.HasAccess(Code))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                                 {
                                                     controller = "Out",
                                                     action = "AccessDenied"
                                                 }));
                    return;
                }
                else
                    Current.Access = Access.Dao.GetByCode(Code);
            }
           
            base.OnActionExecuting(filterContext);
        }
    }
}
