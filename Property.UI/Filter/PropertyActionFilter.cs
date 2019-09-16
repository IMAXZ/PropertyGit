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

namespace Property.UI.Filter
{
    public class PropertyActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 在某个action执行之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //1.获取登录的session信息
            HttpContextBase ctx = filterContext.HttpContext;
            var model = (UserSessionModel)ctx.Session[ConstantParam.SESSION_USERINFO];

            //获取请求地址
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            string reqUrl = "/" + controller + "/" + action;

            //获取到请求地址对应的权限
            IActionBLL actionBll = BLLFactory<IActionBLL>.GetBLL("ActionBLL");
            var act = actionBll.GetEntity(a => a.Href.Equals(reqUrl));
            if (act == null)
            {
                IActionItemBLL actionItemBll = BLLFactory<IActionItemBLL>.GetBLL("ActionItemBLL");
                var actionItem = actionItemBll.GetEntity(i => i.Href.Equals(reqUrl));
                if (actionItem != null)
                {
                    act = actionItem.Action;
                }
            }

            //判断是否为空
            if (model == null)
            {
                if (act != null)
                {
                    //进行注销
                    if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PLATFORM)
                    {
                        filterContext.Result = new RedirectResult("~/Account/PlatformLogOff");
                    }
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PROPERTY)
                    {
                        filterContext.Result = new RedirectResult("~/Account/PropertyLogOff");
                    }
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_COMPANY)
                    {
                        filterContext.Result = new RedirectResult("~/Account/CompanyPlatformLogOff");
                    }
                }
            }
            //如果要访问物业平台，但没保存当前小区
            else if (act != null && act.Menu.IsPlatform == ConstantParam.USER_TYPE_PROPERTY && model.PropertyPlaceId == null)
            {
                filterContext.Result = new RedirectResult("~/Account/PropertyLogOff");
            }

            //权限验证
            //判断用户类型-普通用户的场合
            else if (model.IsMgr == ConstantParam.USER_ROLE_DEFAULT)
            {
                //如果请求地址不在权限字典中
                if (!model.ActionDic.ContainsKey(reqUrl))
                {
                    //如果访问后台，进行注销
                    if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PLATFORM)
                    {
                        filterContext.Result = new RedirectResult("~/Account/PlatformLogOff");
                    }
                    //如果访问物业总公司平台
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_COMPANY)
                    {
                        filterContext.Result = new RedirectResult("~/Account/CompanyPlatformLogOff");
                    }
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PROPERTY)
                    {
                        //如果是物业用户访问物业平台，进行注销
                        if (model.UserType == ConstantParam.USER_TYPE_PROPERTY)
                        {
                            filterContext.Result = new RedirectResult("~/Account/PropertyLogOff");
                        }
                    }
                }
            }
            else
            {
                if (act != null)
                {
                    //如果非后台用户访问后台
                    if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PLATFORM) 
                    {
                        if (model.UserType != ConstantParam.USER_TYPE_PLATFORM)
                        {
                            //进行注销
                            filterContext.Result = new RedirectResult("~/Account/PlatformLogOff");
                        }
                    }
                    //如果非后台或物业用户访问物业平台
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_PROPERTY) 
                    {
                        if (model.UserType != ConstantParam.USER_TYPE_PLATFORM && model.UserType != ConstantParam.USER_TYPE_PROPERTY)
                        {
                            //进行注销
                            filterContext.Result = new RedirectResult("~/Account/PropertyLogOff");
                        }
                    }
                    //如果非总公司用户访问总公司平台
                    else if (act.Menu.IsPlatform == ConstantParam.USER_TYPE_COMPANY)
                    {
                        if (model.UserType != ConstantParam.USER_TYPE_COMPANY)
                        {
                            //进行注销
                            filterContext.Result = new RedirectResult("~/Account/CompanyPlatformLogOff");
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);

        }

        /// <summary>
        /// 请求方法返回之前，用户log日志记录
        /// </summary>
        /// <param name="filterContext">拦截器的上下文</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //执行action后执行这个方法 比如做操作日志
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string reqUrl = "/" + controller + "/" + action;

                //进行log记录的方案
                string[] ExecutingMethodNames = { "Add", "Save", "Edit", "Update", "Del", "Set", "Config", "Reset", "Dispose","Pay"};

                foreach (var method in ExecutingMethodNames)
                {
                    if (action.StartsWith(method))
                    {
                        //进行日志记录
                        var model = (UserSessionModel)filterContext.HttpContext.Session[Property.Common.ConstantParam.SESSION_USERINFO];

                        JsonResult rs = (JsonResult)filterContext.Result;
                        if (rs != null && rs.Data != null)
                        {
                            JsonModel returnModel = (JsonModel)rs.Data;
                            if (returnModel.Code == Property.Common.ConstantParam.JSON_RESULT_OK)
                            {
                                //添加日志
                                AddOpreateLog(returnModel.Content, model, reqUrl);
                                break;
                            }
                        }
                    }
                }
            }
            base.OnResultExecuting(filterContext);

        }


        #region 添加执行操作log

        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="content">操作数据内容</param>
        public void AddOpreateLog(string content, UserSessionModel model, string reqUrl)
        {
            try
            {
                IActionBLL actionBll = BLLFactory<IActionBLL>.GetBLL("ActionBLL");
                //获取到请求地址对应的权限
                var act = actionBll.GetEntity(a => a.Href.Equals(reqUrl));

                //如果当前为平台后台用户
                if (model.UserType == ConstantParam.USER_TYPE_PLATFORM)
                {
                    T_PlatformOpreateLog log = new T_PlatformOpreateLog()
                    {
                        Action = act.ActionName,
                        Desc = content,
                        OpreaterId = model.UserID,
                        OpreateTime = DateTime.Now
                    };
                    IPlatformOpreateLogBLL bll = FactoryBLL.BLLFactory<IPlatformOpreateLogBLL>.GetBLL("PlatformOpreateLogBLL");
                    bll.Save(log);
                }
                else if (model.UserType == ConstantParam.USER_TYPE_PROPERTY)
                {
                    T_PropertyOpreateLog log = new T_PropertyOpreateLog()
                    {
                        Action = act.ActionName,
                        Desc = content,
                        OpreaterId = model.UserID,
                        OpreateTime = DateTime.Now
                    };
                    IPropertyOpreateLogBLL bll = FactoryBLL.BLLFactory<IPropertyOpreateLogBLL>.GetBLL("PropertyOpreateLogBLL");
                    bll.Save(log);
                }
                else if (model.UserType == ConstantParam.USER_TYPE_COMPANY)
                {
                    T_CompanyOpreateLog log = new T_CompanyOpreateLog()
                    {
                        Action = act.ActionName,
                        Desc = content,
                        OpreaterId = model.UserID,
                        OpreateTime = DateTime.Now
                    };
                    ICompanyOpreateLogBLL bll = FactoryBLL.BLLFactory<ICompanyOpreateLogBLL>.GetBLL("CompanyOpreateLogBLL");
                    bll.Save(log);
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}