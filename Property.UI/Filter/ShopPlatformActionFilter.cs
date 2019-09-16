using Property.Common;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Filter
{
    /// <summary>
    /// 门店平台过滤器
    /// </summary>
    public class ShopPlatformActionFilter : ActionFilterAttribute
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

            //如果用户信息为空或类型不是门店用户
            if (model == null || model.UserType != ConstantParam.USER_TYPE_SHOP)
            {
                filterContext.Result = new RedirectResult("~/Account/ShopPlatformLogOff");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}