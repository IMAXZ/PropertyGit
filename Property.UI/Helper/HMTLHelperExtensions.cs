using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// @HTML帮助扩展类
    /// </summary>
    public static class HMTLHelperExtensions
    {

        /// <summary>
        /// 判断用户是否显示该按钮权限
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static bool IsHasButton(this HtmlHelper html, string url)
        {

            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            //获取session对象
            var session = HttpContext.Current.Session;

            //获取
            UserSessionModel model = (UserSessionModel)session[ConstantParam.SESSION_USERINFO];

            //普通用户
            if (model.IsMgr == ConstantParam.USER_ROLE_DEFAULT)
            {
                if (model.ActionDic.ContainsKey(url))
                {
                    return true;
                }
                //获取到请求地址对应的权限
                IActionBLL actionBll = BLLFactory<IActionBLL>.GetBLL("ActionBLL");
                var act = actionBll.GetEntity(a => a.Href.Equals(url));
                //如果是平台用户，菜单为物业菜单，则拥有该按钮权限
                if (act != null && act.Menu.IsPlatform == 0 && model.UserType == ConstantParam.USER_TYPE_PLATFORM)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前登录用户的头像图片路径
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static string GetLoginUserHeadImgPath(this HtmlHelper html)
        {
            //获取session对象
            var session = HttpContext.Current.Session;
            var model = (UserSessionModel)session[ConstantParam.SESSION_USERINFO];
            if (model != null)
            {
                if (model.UserType == ConstantParam.USER_TYPE_PROPERTY)
                {
                    IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    return user.HeadPath;
                }
                else if (model.UserType == ConstantParam.USER_TYPE_SHOP)
                {
                    IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    return user.HeadPath;
                }
                else if (model.UserType == ConstantParam.USER_TYPE_PLATFORM)
                {
                    IPlatformUserBLL userBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    return user.HeadPath;
                }
                else if (model.UserType == ConstantParam.USER_TYPE_COMPANY)
                {
                    ICompanyUserBLL userBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    return user.HeadPath;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前登录用户的名字
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static string GetLoginUserName(this HtmlHelper html)
        {
            string name = "";
            //获取session对象
            var session = HttpContext.Current.Session;
            var model = (UserSessionModel)session[ConstantParam.SESSION_USERINFO];
            if (model != null)
            {
                if (model.UserType == ConstantParam.USER_TYPE_PROPERTY)
                {
                    IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                    var user = propertyUserBll.GetEntity(u => u.Id == model.UserID);
                    if (!string.IsNullOrEmpty(user.TrueName))
                    {
                        name = user.TrueName;
                    }
                    else
                    {
                        name = user.UserName;
                    }
                }
                else if (model.UserType == ConstantParam.USER_TYPE_SHOP)
                {
                    IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                    var user = shopUserBll.GetEntity(u => u.Id == model.UserID);
                    if (!string.IsNullOrEmpty(user.TrueName))
                    {
                        name = user.TrueName;
                    }
                    else
                    {
                        name = user.UserName;
                    }
                }
                else if (model.UserType == ConstantParam.USER_TYPE_PLATFORM)
                {
                    IPlatformUserBLL userBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    if (!string.IsNullOrEmpty(user.TrueName))
                    {
                        name = user.TrueName;
                    }
                    else
                    {
                        name = user.UserName;
                    }
                }
                else if (model.UserType == ConstantParam.USER_TYPE_COMPANY)
                {
                    ICompanyUserBLL userBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                    var user = userBll.GetEntity(u => u.Id == model.UserID);
                    if (!string.IsNullOrEmpty(user.TrueName))
                    {
                        name = user.TrueName;
                    }
                    else
                    {
                        name = user.UserName;
                    }
                }
            }
            return name;
        }

        /// <summary>
        /// 获取当前session对象
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static UserSessionModel GetSessionModel(this HtmlHelper html)
        {
            //获取session对象
            var session = HttpContext.Current.Session;
            var model = (UserSessionModel)session[ConstantParam.SESSION_USERINFO];

            return model;
        }

        /// <summary>
        /// 获取当前登录的物业用户所属小区
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static string GetPropertyPlaceName(this HtmlHelper html)
        {
            var model = GetSessionModel(html);
            if (model != null)
            {
                int placeId = model.PropertyPlaceId.Value;
                IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBll.GetEntity(p => p.Id == placeId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (place != null)
                {
                    return place.Name;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取当前登录的总公司用户所属公司
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static string GetPropertyCompanyName(this HtmlHelper html)
        {
            var model = GetSessionModel(html);
            if (model != null)
            {
                int companyId = model.CompanyId.Value;
                IPropertyCompanyBLL companyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
                var company = companyBll.GetEntity(p => p.Id == companyId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (company != null)
                {
                    return company.Name;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前登录用户ID
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static int GetLoginUserId(this HtmlHelper html)
        {
            var model = GetSessionModel(html);
            if (model != null)
            {
                return model.UserID;
            }
            return 0;
        }

        /// <summary>
        /// 获取未处理的上报问题个数
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static int GetNoDisposeQuestionNum(this HtmlHelper html)
        {
            int CurrentPlaceId = GetSessionModel(html).PropertyPlaceId.Value;

            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            return questionBll.Count(q => q.Status == ConstantParam.NO_DISPOSE && q.PropertyPlaceId == CurrentPlaceId);
        }

        /// <summary>
        /// 获取最新上报的问题
        /// </summary>
        /// <param name="html">HTML对象</param>
        /// <returns></returns>
        public static List<QuestionIntroModel> GetNewestNoDisposeQuestion(this HtmlHelper html)
        {
            int CurrentPlaceId = GetSessionModel(html).PropertyPlaceId.Value;

            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            //分页获取前3条数据：分页每页3条数据，获取第一页
            var que = questionBll.GetPageList(q => q.Status == ConstantParam.NO_DISPOSE && q.PropertyPlaceId == CurrentPlaceId, "UploadTime", false, 1, 3).Select(q => new QuestionIntroModel
            {
                Title = q.Title,
                TimeAgoStr = (DateTime.Now - q.UploadTime).Days > 0 ? (DateTime.Now - q.UploadTime).Days + "天前" :
                    ((DateTime.Now - q.UploadTime).Hours > 0 ? (DateTime.Now - q.UploadTime).Hours + "小时前" : (DateTime.Now - q.UploadTime).Minutes + "分钟前")
            });
            if (que != null)
            {
                return que.ToList();
            }
            return null;
        }
    }
}