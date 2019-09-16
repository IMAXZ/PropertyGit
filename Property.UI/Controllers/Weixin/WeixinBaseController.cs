using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Filter;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 微信控制器基类
    /// </summary>
    [WeixinActionFilter]
    public class WeixinBaseController : Controller
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public T_User GetCurrentUser()
        {
            string Unionid = (string)Session["Unionid"];
            IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            T_User owner = ownerBll.GetEntity(o => o.WeixinUnionId == Unionid && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            return owner;
        }

        /// <summary>
        /// 获取当前用户通过验证的小区ID
        /// </summary>
        /// <returns></returns>
        public List<int> GetVerifiedPlaceIds()
        {
            var owner = GetCurrentUser();
            //获取通过验证的小区ID集合
            var PlaceIds = owner.UserPlaces.Where(p => p.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(p => new
            {
                PlaceId = p.PropertyPlaceId,
                VerifyStatus = IsVerified(p.PropertyPlace, p.User)
            }).Where(bp => bp.VerifyStatus).Select(bp => bp.PlaceId).ToList();

            return PlaceIds;
        }

        /// <summary>
        /// 获取当前用户通过验证的小区下拉列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetVerifiedPlaceList()
        {
            var owner = GetCurrentUser();
            //获取通过验证的小区ID集合
            var Places = owner.UserPlaces.Where(p => p.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(p => new
            {
                PlaceId = p.PropertyPlaceId,
                PlaceName = p.PropertyPlace.Name,
                VerifyStatus = IsVerified(p.PropertyPlace, p.User)
            }).ToList();
            return Places.Where(bp => bp.VerifyStatus).Select(bp => new SelectListItem()
            {
                Text = bp.PlaceName,
                Value = bp.PlaceId.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 下载保存多媒体文件,返回多媒体保存路径 
        /// </summary>
        /// <param name="dirPath">下载文件保存目录</param>
        /// <param name="MEDIA_ID">多媒体文件服务器标识</param>
        /// <returns>下载的文件路径，用;分割</returns>
        public string GetMultimedia(string dirPath,string MEDIA_ID)
        {
            //目录创建
            string dir = Server.MapPath(dirPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string ACCESS_TOKEN = GetTokenUtils.GetToken();
            string file = string.Empty;
            string[] MEDIA_IDs = MEDIA_ID.Split(',');
            for (int i = 0; i < MEDIA_IDs.Length; i++)
            {
                string content = string.Empty;
                string strpath = string.Empty;
                string savepath = string.Empty;
                string stUrl = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + ACCESS_TOKEN + "&media_id=" + MEDIA_IDs[i];

                WebClient mywebclient = new WebClient();
                string imgFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next().ToString().Substring(0, 4) + ".jpg";
                savepath = Path.Combine(dir, imgFileName);
                try
                {
                    mywebclient.DownloadFile(stUrl, savepath);
                    file += dirPath + imgFileName + ";";
                }
                catch (Exception ex)
                {
                    PubFunction.ErrorLogPrint("服务器下载图片发生异常:", ex.ToString());
                }

            }
            file = file.Substring(0, file.Length - 1);
            return file;
        }

        /// <summary>
        /// 微信API调用参数初始化
        /// </summary>
        public void WeixinApiInit()
        {
            string jstoken = GetTokenUtils.GetJsApiTicket();

            ViewBag.appId = ConstantParam.AppId;
            int timestamp = TickenUtils.ConvertDateTimeInt(DateTime.Now);
            ViewBag.timestamp = timestamp;
            string nonceStr = TickenUtils.createNonceStr();
            ViewBag.nonceStr = nonceStr;

            string url = PropertyUtils.GetConfigParamValue("HostUrl") + Request.RawUrl;
            // 这里参数的顺序要按照 key 值 ASCII 码升序排序  
            string rawstring = "jsapi_ticket=" + jstoken + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url;
            ViewBag.signature = TickenUtils.SHA1_Hash(rawstring);
        }

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="date">要格式化的日期</param>
        /// <returns></returns>
        public string TimeFormat(DateTime date)
        {
            if (date.Year == DateTime.Now.Year)
            {
                if (date.Month == DateTime.Now.Month && date.Day == DateTime.Now.Day)
                {
                    return date.ToString("HH:mm");
                }
                else
                {
                    return date.ToString("MM/dd HH:mm");
                }
            }
            else
            {
                return date.ToString("yyyy/MM/dd");
            }
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

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="filterContext">过滤器上下文</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //ajax请求的场合
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonModel model = new JsonModel();
                model.Msg = "请求发生异常";
                JsonResult result = new JsonResult();
                result.Data = model;
                filterContext.Result = result;
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Weixin/Error");
            }
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
    }
}
