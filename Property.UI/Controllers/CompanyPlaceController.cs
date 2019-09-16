using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 公司小区管理控制器
    /// </summary>
    public class CompanyPlaceController : PlaceBaseController
    {
        /// <summary>
        /// 物业小区列表
        /// </summary>
        /// <returns>小区列表页面</returns>
        [BreadCrumb(Label = "物业小区列表")]
        [HttpGet]
        public ActionResult PlaceList(PropertyPlaceSearchModel model)
        {
            model.CompanyId = GetSessionModel().CompanyId.Value;
            return View(GetPlaceList(model));
        }

        /// <summary>
        /// 物业小区详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "物业小区详细")]
        public ActionResult PlaceDetail(int id)
        {
            //获取要查看详细的小区
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = placeBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (place != null)
            {
                //如果存在,将数据传递到页面
                return View(place);
            }
            else
            {
                //如果不存在，则回到小区列表页
                return RedirectToAction("PlaceList");
            }
        }


        /// <summary>
        /// 添加物业小区
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增物业小区")]
        [HttpGet]
        public ActionResult AddPlace()
        {
            PropertyPlaceModel model = new PropertyPlaceModel();
            model.ProvinceList = GetProvinceList();
            model.CityList = new List<SelectListItem>();
            model.CountyList = new List<SelectListItem>();
            model.PlaceTypeList = GetPlaceTypeList();

            model.CompanyId = GetSessionModel().CompanyId.Value;
            model.IsValidate = true;
            return View(model);
        }

        /// <summary>
        /// 添加物业小区表单提交
        /// </summary>
        /// <param name="model">物业小区表单数据模型</param>
        /// <returns>添加结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPlace(PropertyPlaceModel model)
        {
            var jm = base.AddPlace(model);
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 编辑物业小区
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑物业小区")]
        [HttpGet]
        public ActionResult EditPlace(int id)
        {
            //获取指定ID且未删除的小区
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = placeBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //如果该小区存在
            if (place != null)
            {
                //初始化小区模型
                var model = InitPlaceEditModel(place);
                //将模型传给页面
                return View(model);
            }
            else
            {
                return RedirectToAction("PlaceList");
            }
        }

        /// <summary>
        /// 物业小区编辑提交
        /// </summary>
        /// <param name="model">物业小区表单模型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditPlace(PropertyPlaceModel model)
        {
            var jm = base.EditPlace(model);
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除指定物业小区
        /// </summary>
        /// <param name="id">要删除的物业小区ID</param>
        /// <returns>删除结果</returns>
        [HttpPost]
        public JsonResult DeletePlace(int id)
        {
            var jm = base.DelPlace(id);
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 跳转到设置管理员界面
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "设置管理员")]
        [HttpGet]
        public ActionResult SetAdministrator(int id)
        {
            //获取要设置管理员的小区
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = placeBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //如果该小区存在
            if (place != null)
            {
                //初始化返回页面的模型
                PropertyUserModel model = new PropertyUserModel();
                model.PlaceId = place.Id;
                model.PlaceName = place.Name;
                ViewBag.Admins = place.PropertyUsers.Where(u => u.IsMgr == ConstantParam.USER_ROLE_MGR).Select(u => u.UserName).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("PlaceList");
            }
        }

        /// <summary>
        /// 设置管理员提交
        /// </summary>
        /// <param name="model">物业用户模型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetAdministrator(PropertyUserModel model)
        {
            var jm = base.SetAdmin(model);
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 物业小区图标上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "设置小区图标")]
        public ActionResult UploadImg(int id)
        {
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = propertyPlaceBll.GetEntity(m => m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.Id == id);

            //物业小区存在
            if (place != null)
            {
                PropertyPlaceModel model = new PropertyPlaceModel()
                {
                    PlaceId = place.Id,
                    Img = place.Img
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("PlaceList");
            }
        }

        /// <summary>
        /// 物业小区图标上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadImg(string data, int id)
        {
            var jm = base.UploadPlaceImg(data, id);
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证物业小区是否存在
        /// </summary>
        /// <param name="UserName"></param>
        [HttpPost]
        public ContentResult CheckPlaceExist(PropertyPlaceModel model)
        {
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            if (propertyPlaceBll.Exist(m => m.Name == model.PlaceName && m.Id != model.PlaceId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }

        /// <summary>
        /// 远程验证指定物业用户名称是否存在
        /// </summary>
        /// <param name="UserName">物业用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        [HttpPost]
        public ContentResult RemoteUserCheckExist(string userName)
        {
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            // 用户名已存在
            if (propertyUserBll.Exist(m => m.UserName == userName && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                // 校验不通过
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
    }
}
