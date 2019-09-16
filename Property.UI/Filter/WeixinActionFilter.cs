using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Filter
{
    /// <summary>
    /// 微信开发身份验证过滤器
    /// </summary>
    public class WeixinActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 在某个action执行之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            //获取当前保存的Session
            HttpContextBase ctx = filterContext.HttpContext;
            string Unionid = (string)ctx.Session["Unionid"];
            string Openid = (string)ctx.Session["OpenId"];

            //获取请求地址
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            if (Openid == null || Unionid == null || !ownerBll.Exist(o => o.WeixinUnionId == Unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT)
                || !ownerBll.Exist(o => o.WeixinOpenId == Openid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                string url = HttpContext.Current.Request.Url.ToString();
                ctx.Session["VisitUrl"] = url;
                string oAuthUrl = OAuthApi.GetAuthorizeUrl(ConstantParam.AppId, "http://v.homekeeper.com.cn/WeixinOAuth2/UserInfoCallback", "sarnath", OAuthScope.snsapi_userinfo);
                filterContext.Result = new RedirectResult(oAuthUrl);
            }
            else if (!"WeixinPersonalCenter".Equals(controller) && !"WeixinIdentityBind".Equals(controller) && !"WeixinHome".Equals(controller))
            {
                var owner = ownerBll.GetEntity(o => o.WeixinUnionId == Unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                var userPlaces = owner.UserPlaces.Where(p => p.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (userPlaces.Count() == 0)
                {
                    filterContext.Result = new RedirectResult("/WeixinIdentityBind/AddPlace");
                }
                else if (!"WeixinPropertyNotice".Equals(controller) && userPlaces.Select(p => IsVerified(p.PropertyPlace, p.User)).Count(i => i) == 0)
                {
                    filterContext.Result = new RedirectResult("/WeixinIdentityBind/Index");
                }
            }
            base.OnActionExecuting(filterContext);
        }


        /// <summary>
        /// 判断是否通过验证
        /// </summary>
        /// <param name="Place">要验证的小区</param>
        /// <param name="User">注册用户</param>
        /// <returns>是否通过验证</returns>
        private bool IsVerified(T_PropertyPlace Place, T_User User)
        {
            //如果是住宅小区
            if (Place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.DoorId != null && v.BuildDoor.BuildUnit.Build.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification != null)
                {
                    return IdentityVerification.IsVerified == ConstantParam.IsVerified_YES;
                }
            }
            else
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.BuildCompany.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification != null)
                {
                    return IdentityVerification.IsVerified == ConstantParam.IsVerified_YES;
                }
            }
            return false;
        }
    }
}