using Property.Common;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Property.UI.Filter
{
    public class PlatformLoggedAccountActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 在某个action执行之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //获取登录的session信息
            HttpContextBase ctx = filterContext.HttpContext;
            var model = (UserSessionModel)ctx.Session[ConstantParam.SESSION_USERINFO];

            //如果用户信息为空或类型不是平台用户
            if (model == null || model.UserType != ConstantParam.USER_TYPE_PLATFORM)
            {
                filterContext.Result = new RedirectResult("~/Account/PlatformLogOff");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
