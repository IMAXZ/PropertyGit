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
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业公司平台 -- 物业公司详细 控制器
    /// </summary>
    public class CompanyDetailController : BaseController
    {
        /// <summary>
        /// 物业总公司详细 
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "物业总公司详细")]
        [HttpGet]
        public ActionResult Detail()
        {
            //获取登录物业公司
            int CompanyId = GetSessionModel().CompanyId.Value;
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == CompanyId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (company != null)
            {
                //初始化返回页面的模型
                PropertyCompanyModel model = new PropertyCompanyModel()
                {
                    Name = company.Name,
                    Address = company.Address,
                    Content = company.Content,
                    Img = company.Img,
                    Tel = company.Tel
                };
                return View(model);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "设置图标")]
        public ActionResult SetCompanyInfo()
        {
            int CompanyId = GetSessionModel().CompanyId.Value;
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

            //获取要编辑的物业公司
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == CompanyId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (company != null)
            {
                //初始化返回页面的模型
                PropertyCompanyModel model = new PropertyCompanyModel()
                {
                    Id = company.Id,
                    Img = company.Img
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CompanyDetail");
            }
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetCompanyInfo(SetPropertyCompanyModel model)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                //存入文件的路径
                string directory = Server.MapPath(ConstantParam.PROPERTY_COMPANY_DIR);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                HttpPostedFileBase file = model.UploadImg;
                string filename = Path.GetFileName(file.FileName);//获取上传文件名
                string fileEx = Path.GetExtension(filename);//获取上传文件的扩展名

                //存入的文件名
                string FileName = DateTime.Now.ToFileTime().ToString() + fileEx;

                //保存数据文件
                string savrPath = Path.Combine(directory, FileName);
                file.SaveAs(savrPath);

                IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
                T_Company company = propertyCompanyBll.GetEntity(m => m.Id == model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (company != null)
                {
                    string oldFile = company.Img;
                    company.Img = ConstantParam.PROPERTY_COMPANY_DIR + FileName;
                    propertyCompanyBll.Update(company);

                    //删除旧图标
                    if (!string.IsNullOrEmpty(oldFile))
                    {
                        oldFile = Server.MapPath(oldFile);
                        FileInfo f = new FileInfo(oldFile);
                        if (f.Exists)
                            f.Delete();
                    }
                }

                //日志记录
                //jm.Content = PropertyUtils.ModelToJsonString(company);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑物业总公司
        /// </summary>
        /// <param name="id">编辑的物业公司Id</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑物业总公司")]
        [HttpGet]
        public ActionResult EditCompany()
        {
            int CompanyId = GetSessionModel().CompanyId.Value;
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

            //获取要编辑的物业公司
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == CompanyId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (company != null)
            {
                //初始化返回页面的模型
                PropertyCompanyModel model = new PropertyCompanyModel()
                {
                    Id = company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    Content = company.Content,
                    Tel = company.Tel
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CompanyDetail");
            }
        }

        /// <summary>
        /// 编辑总物业公司
        /// </summary>
        /// <param name="model">编辑的物业公司模型</param>
        /// <returns>编辑结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditCompany(PropertyCompanyModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

                T_Company company = propertyCompanyBll.GetEntity(m => m.Id == model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (company != null)
                {
                    company.Address = model.Address;
                    company.Content = model.Content;
                    company.Tel = model.Tel;
                    //保存到数据库
                    if (propertyCompanyBll.Update(company))
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
                    jm.Msg = "该物业公司不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

    }
}
