using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Filter;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 门店平台基类控制器
    /// </summary>
    [ShopPlatformActionFilter]
    public class ShopBaseController : Controller
    {
        #region 帮助方法

        /// <summary>
        /// 获取session模型
        /// </summary>
        /// <returns></returns>
        public UserSessionModel GetSessionModel()
        {
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            return model;
        }

        /// <summary>
        /// 获取当前门店ID
        /// </summary>
        /// <returns></returns>
        public int? GetCurrentShopId()
        {
            //获取登录用户的门店
            int UserId = GetSessionModel().UserID;
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var Shop = ShopBll.GetEntity(s => s.ShopUserId == UserId);
            if (Shop != null)
            {
                return Shop.Id;
            }
            return null;
        }

        /// <summary>
        /// 获取省份列表
        /// </summary>
        /// <returns>省份列表</returns>
        public List<SelectListItem> GetProvinceList()
        {
            //获取省份列表
            IProvinceBLL provinceBll = BLLFactory<IProvinceBLL>.GetBLL("ProvinceBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = provinceBll.GetList(u => true, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.ProvinceName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns>城市列表</returns>
        public List<SelectListItem> GetCityList(int provinceId)
        {
            //获取指定省份的城市列表
            ICityBLL cityBll = BLLFactory<ICityBLL>.GetBLL("CityBLL");
            var sortModel = this.SettingSorting("Id", false);
            var list = cityBll.GetList(u => u.ProvinceId == provinceId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.CityName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取区县下拉列表
        /// </summary>
        /// <returns>区县列表</returns>
        public List<SelectListItem> GetCountyList(int cityId)
        {
            //获取指定城市的区县列表
            ICountyBLL countyBll = BLLFactory<ICountyBLL>.GetBLL("CountyBLL");
            var sortModel = this.SettingSorting("Id", false);
            var list = countyBll.GetList(u => u.CityId == cityId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.CountyName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        #endregion

        #region 排序工具类

        /// <summary>
        /// 排序工具类，返回排序的名称以及排序的顺序
        /// </summary>
        /// <param name="model">排序模型</param>
        /// <param name="defaultSortName">默认的排序名称</param>
        /// <param name="defaultAsc">模型的排序顺序</param>
        /// <returns></returns>
        protected SortModel SettingSorting(string defaultSortName, bool defaultAsc)
        {
            SortModel sortModel = new SortModel();
            var sortName = Request.QueryString["SortName"];
            var IsAsc = Request.QueryString["IsAsc"];
            if (string.IsNullOrEmpty(sortName))
            {
                sortModel.SortName = defaultSortName;
            }
            else
            {
                sortModel.SortName = sortName;
            }

            if (string.IsNullOrEmpty(IsAsc))
            {
                sortModel.IsAsc = defaultAsc;
            }
            else
            {
                try
                {
                    sortModel.IsAsc = Convert.ToBoolean(IsAsc);
                }
                catch
                {
                    sortModel.IsAsc = defaultAsc;

                }
            }
            return sortModel;
        }

        #endregion

        #region 共通异常处理
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
                model.Msg = "请求发生异常，请重试";
                JsonResult result = new JsonResult();
                result.Data = model;
                filterContext.Result = result;
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Common/Error500");
            }

            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
        #endregion
    }
}
