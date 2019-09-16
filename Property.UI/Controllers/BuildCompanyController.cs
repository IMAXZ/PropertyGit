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
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    public class BuildCompanyController : BaseController
    {
        /// <summary>
        /// 办公楼业主列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "办公楼单位业主列表")]
        [HttpGet]
        public ActionResult BuildCompanyList(BuildCompanyModel model)
        {
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            //查询
            Expression<Func<T_BuildCompany, bool>> where = u => (string.IsNullOrEmpty(model.Name) ? true : u.Name.Contains(model.Name)) && u.DelFlag == Property.Common.ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlaceId== propertyPlaceId;
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = buildCompanyBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }
        /// <summary>
        /// 办公楼单位业主新增
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增办公楼单位业主")]
        [HttpGet]
        public ActionResult AddBuildCompany()
        {
            return View();
        }
        /// <summary>
        /// 办公楼单位业主新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBuildCompany(BuildCompanyModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
                T_BuildCompany buildCompany = new T_BuildCompany()
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Desc = model.Desc,
                    PayDesc = model.PayDesc,
                    ServiceDesc = model.ServiceDesc,
                    PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value
                };
                // 保存
                buildCompanyBll.Save(buildCompany);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 远程验证指定办公楼单位业主名称是否存在
        /// </summary>

        [HttpPost]
        public ContentResult RemoteCheckExist(BuildCompanyModel model)
        {
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            // 用户名已存在
            if (buildCompanyBll.Exist(m => m.Name == model.Name && m.Id != model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                // 校验不通过

                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
        /// <summary>
        /// 编辑办公楼单位业主信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑办公楼单位业主")]
        [HttpGet]
        public ActionResult EditBuildCompany(int id)
        {
           
                IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");

                //获取要编辑的物业公司
                T_BuildCompany buildCompany = buildCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (buildCompany != null)
                {
                    //初始化返回页面的模型
                    BuildCompanyModel model = new BuildCompanyModel()
                    {
                        Id = buildCompany.Id,
                        Name = buildCompany.Name,
                        Phone = buildCompany.Phone,
                        Desc = buildCompany.Desc,
                        PayDesc = buildCompany.PayDesc,
                        ServiceDesc = buildCompany.ServiceDesc
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("BuildCompanyList");
                }
            
           
        }
        /// <summary>
        /// 编辑办公楼单位业主信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditBuildCompany(BuildCompanyModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");

                T_BuildCompany buildCompany = buildCompanyBll.GetEntity(m => m.Id == model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (buildCompany != null)
                {
                    buildCompany.Name = model.Name;
                    buildCompany.Phone = model.Phone;
                    buildCompany.Desc = model.Desc;
                    buildCompany.PayDesc = model.PayDesc;
                    buildCompany.ServiceDesc = model.ServiceDesc;
                    //保存到数据库
                    if (buildCompanyBll.Update(buildCompany))
                    {
                        //日志记录
                        jm.Content = PropertyUtils.ModelToJsonString(model);
                    }
                    else
                    {
                        jm.Msg = "编辑失败";
                    }
                }
                else
                {
                    jm.Msg = "该办公楼单位业主不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除办公楼单位业主
        /// </summary>
        /// <param name="id">删除的办公楼单位业主Id</param>
        /// <returns>删除结果返回</returns>
        [HttpPost]
        public JsonResult DeleteBuildCompany(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要删除的物业公司
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            T_BuildCompany buildCompany = buildCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            //如果该物业公司存在
            if (buildCompany == null)
            {
                jm.Msg = "该办公楼单位业主不存在";
            }
            else
            {
                //修改指定办公楼单位业主的已删除标识
                buildCompany.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                buildCompanyBll.Update(buildCompany);
                //操作日志
                jm.Content = "删除该办公楼单位业主 " + buildCompany.Name;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}