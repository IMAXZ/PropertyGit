using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业公司平台 -- 物业业主一览 控制器
    /// </summary>
    public class PropertyOwnerController : BaseController
    {

        /// <summary>
        /// 住宅业主列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "住宅业主列表")]
        [HttpGet]
        public ActionResult HouseUserList(HouseUserSearchModel Model)
        {
            int CompanyId = GetSessionModel().CompanyId.Value;

            //根据业主姓名查询
            Expression<Func<T_HouseUser, bool>> where = u => (string.IsNullOrEmpty(Model.Name) ? true : u.Name.Contains(Model.Name)) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlace.CompanyId == CompanyId && u.PropertyPlace.PlaceType == ConstantParam.PLACE_TYPE_HOUSE;

            //根据小区名称查询
            if (Model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == Model.PropertyPlaceId.Value);
            }

            //根据楼座，单元，单元户名称查询
            if (!string.IsNullOrEmpty(Model.Kword))
            {
                where = PredicateBuilder.And(where, u => u.BuildDoor.BuildUnit.Build.BuildName.Contains(Model.Kword) || u.BuildDoor.BuildUnit.UnitName.Contains(Model.Kword) || u.BuildDoor.DoorName.Contains(Model.Kword));
            }

            //排序
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            var sortModel = this.SettingSorting("Id", false);
            Model.DataList = houseUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, Model.PageIndex) as PagedList<T_HouseUser>;

            //获取所有物业小区列表
            Model.PropertyPlaceList = GetPropertyPlaceList();

            return View(Model);
        }

        /// <summary>
        /// 住宅业主详细
        /// </summary>
        /// <param name="id">住宅业主Id</param>
        /// <returns>结果返回</returns>
        [BreadCrumb(Label = "住宅业主详细")]
        [HttpGet]
        public ActionResult HouseUserDetail(int id)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            //获取要查看的住宅业主
            T_HouseUser houseUser = houseUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (houseUser != null)
            {
                return View(houseUser);
            }
            else
            {
                return RedirectToAction("HouseUserList");
            }
        }

        /// <summary>
        /// 获取住宅小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetPropertyPlaceList()
        {
            int CompanyId = GetSessionModel().CompanyId.Value;
            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CompanyId == CompanyId && p.PlaceType == ConstantParam.PLACE_TYPE_HOUSE);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取办公楼小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetCompanyPlaceList()
        {
            int CompanyId = GetSessionModel().CompanyId.Value;
            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CompanyId == CompanyId && p.PlaceType == ConstantParam.PLACE_TYPE_COMPANY);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }


        /// <summary>
        /// 办公楼业主列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "办公楼业主列表")]
        [HttpGet]
        public ActionResult BuildCompanyList(BuildCompanyModel model)
        {
            //物业公司Id
            int CompanyId = GetSessionModel().CompanyId.Value;

            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");

            //查询
            Expression<Func<T_BuildCompany, bool>> where = u => (string.IsNullOrEmpty(model.Name) ? true : u.Name.Contains(model.Name)) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlace.CompanyId == CompanyId && u.PropertyPlace.PlaceType == ConstantParam.PLACE_TYPE_COMPANY;

            //根据小区名称查询
            if (model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == model.PropertyPlaceId.Value);
            }

            //排序
            var sortModel = this.SettingSorting("Id", false);
            model.DataList = buildCompanyBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_BuildCompany>;

            //获取所有物业小区列表
            model.CompanyPlaceList = GetCompanyPlaceList();

            return View(model);
        }

    }
}
