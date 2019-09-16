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
    /// APP注册用户收货地址管理
    /// </summary>
    public class UserShippingController : BaseController
    {
        /// <summary>
        /// 收货地址列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "收货地址一览")]
        [HttpGet]
        public ActionResult AddressList(ShippingAddressSearchModel model)
        {
            //获取当前物业小区
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId ?? 0;
            //查询条件
            Expression<Func<T_AppUserShippingAddress, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && 
                u.User.UserPlaces.Any(up => up.PropertyPlaceId == CurrentPlaceId);
            //如果要查询默认地址
            if (model.IsDefault == 1) 
            {
                where = PredicateBuilder.And(where, u => u.IsDefault == 1);
            }
            //根据所属APP用户用户名模糊查询
            if (!string.IsNullOrEmpty(model.Kword)) 
            {
                where = PredicateBuilder.And(where, u => u.User.UserName.Contains(model.Kword));
            }
            //排序
            var sortModel = this.SettingSorting("Id", false);
            //获取用户收货地址分页数据
            IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
            model.ResultList = addressBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_AppUserShippingAddress>;
            model.IsDefaultList = GetIsDefaultList();
            return View(model);
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteAddress(int id) 
        {
            JsonModel jm = new JsonModel();
            try
            {
                //获取要删除的收货地址
                IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
                T_AppUserShippingAddress address = addressBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果该收货地址存在
                if (address != null)
                {
                    address.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                    //删除
                    if (addressBll.Update(address))
                    {
                        //操作日志
                        jm.Content = PropertyUtils.ModelToJsonString(address);
                    }
                    else
                    {
                        jm.Msg = "删除失败";
                    }
                }
                else
                {
                    jm.Msg = "该收货地址不存在";
                }
            }
            catch
            {
                jm.Msg = "删除失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取地址是否默认下拉选择列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetIsDefaultList()
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = "全部",
                Value = "0",
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = "默认",
                Value = "1",
                Selected = false
            });
            return typeList;
        }
    }
}
