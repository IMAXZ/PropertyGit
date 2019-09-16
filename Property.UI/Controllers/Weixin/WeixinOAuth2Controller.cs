using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using Property.Common;
using Property.IBLL;
using Property.FactoryBLL;
using Senparc.Weixin;
using Property.Entity;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// OAuth2 授权控制器
    /// </summary>
    public class WeixinOAuth2Controller : Controller
    {
        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult UserInfoCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }
            if (state != "sarnath")
            {
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                return Content("验证失败！请从正规途径进入！");
            }
            OAuthAccessTokenResult result = null;
            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(ConstantParam.AppId, ConstantParam.AppSecret, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            Session["OAuthAccessToken"] = result;

            //获取用户详细信息
            try
            {
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                //如果微信Unionid不存在
                if (!ownerBll.Exist(o => o.WeixinUnionId == userInfo.unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    //随机字符串
                    string str = "1234567890abcdefghijklmnopqrstuvwxyz";
                    Random r = new Random();
                    string RandomStr = "";
                    for (int i = 0; i < 16; i++)
                    {
                        RandomStr += str[r.Next(str.Length)].ToString();
                    }
                    T_User user = new T_User()
                    {
                        UserName = RandomStr,
                        WeixinOpenId = userInfo.openid,
                        WeixinUnionId = userInfo.unionid
                    };
                    if (userInfo.sex > 0)
                    {
                        user.Gender = userInfo.sex == 1 ? 0 : 1;
                    }
                    ownerBll.Save(user);
                }
                else if (!ownerBll.Exist(o => o.WeixinOpenId == userInfo.openid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    T_User user = ownerBll.GetEntity(o => o.WeixinUnionId == userInfo.unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                    user.WeixinOpenId = userInfo.openid;
                    ownerBll.Update(user);
                }
                var owner = ownerBll.GetEntity(o => o.WeixinUnionId == userInfo.unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                Session["OpenId"] = owner.WeixinOpenId;
                Session["Unionid"] = owner.WeixinUnionId;
                return Redirect((string)Session["VisitUrl"]);
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }
    }
}