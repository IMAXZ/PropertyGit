using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Property.UI.Models;
using Property.UI.Controllers;
using Property.IBLL;
using Property.FactoryBLL;
using Property.Entity;
using Property.Common;
using MvcBreadCrumbs;
using System.IO;


namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业公司控制器
    /// </summary>
    public class PropertyCompanyController : BaseController
    {

        /// <summary>
        /// 物业公司列表
        /// </summary>
        /// <param name="model">搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "物业公司列表")]
        [HttpGet]
        public ActionResult CompanyList(PropertyCompanySearchModel model)
        {
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

            //查询条件
            Expression<Func<T_Company, bool>> where = u => (string.IsNullOrEmpty(model.Name) ? true : u.Name.Contains(model.Name)) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = propertyCompanyBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(list);
        }

        /// <summary>
        /// 新增物业公司
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增物业公司")]
        [HttpGet]
        public ActionResult AddCompany()
        {
            return View();
        }

        /// <summary>
        /// 新增物业公司
        /// </summary>
        /// <param name="model">新增的物业公司模型</param>
        /// <returns>新增结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddCompany(PropertyCompanyModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

                //如果图片不为空
                if (model.UploadImg != null)
                {
                    //存入文件的路径
                    string directory = Server.MapPath(ConstantParam.PROPERTY_COMPANY_DIR);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    HttpPostedFileBase file = model.UploadImg;
                    string filename = Path.GetFileName(file.FileName);//获取上传文件名
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件扩展名

                    //存入的文件名
                    string FileName = DateTime.Now.ToFileTime().ToString() + fileEx;

                    //保存的数据文件
                    string savrPath = Path.Combine(directory, FileName);
                    file.SaveAs(savrPath);

                    //初始化平台物业公司数据实体
                    T_Company company = new T_Company()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Content = model.Content,
                        Tel = model.Tel,
                        Img = ConstantParam.PROPERTY_COMPANY_DIR + FileName
                    };

                    //保存
                    propertyCompanyBll.AddCompany(company);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(company);
                }

                else
                {
                    //初始化平台物业公司数据实体
                    T_Company Company = new T_Company()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Content = model.Content,
                        Tel = model.Tel
                    };

                    //保存
                    propertyCompanyBll.AddCompany(Company);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(Company);
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证物业公司名称是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult RemoteCheck(PropertyCompanyModel model)
        {
            IPropertyCompanyBLL propertyCompanybll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            if(propertyCompanybll.Exist(m=>m.Name==model.Name&&m.Id!=model.Id&&m.DelFlag==ConstantParam.DEL_FLAG_DEFAULT))
            {
                return Content("false");
            }
            else
            {
                return Content("true");      
            }
        }

        /// <summary>
        /// 删除物业公司
        /// </summary>
        /// <param name="id">删除的物业公司Id</param>
        /// <returns>删除结果返回</returns>
        [HttpPost]
        public JsonResult DeleteCompany(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要删除的物业公司
            IPropertyCompanyBLL propertyCompanBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            T_Company company = propertyCompanBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            //如果该物业公司存在
            if (company == null)
            {
                jm.Msg = "该物业公司不存在";
            }
            else if (company.PropertyPlaces.Count(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT) > 0) 
            {
                jm.Msg = "该公司下有小区存在，不能删除";
            }
            else
            {
                //修改指定物业公司中的已删除标识
                company.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                propertyCompanBll.Update(company);
                //操作日志
                jm.Content = "删除物业公司 " + company.Name;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置公司图标
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "设置公司图标")]
        [HttpGet]
        public ActionResult SetPlatCompanyInfo(int id)
        {
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (company != null)
            {
                SetPropertyCompanyModel model = new SetPropertyCompanyModel()
                {
                    Id = id,
                    Img = company.Img
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CompanyList");
            }
        }

        /// <summary>
        /// 设置公司图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetPlatCompanyInfo(SetPropertyCompanyModel model)
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
        /// 编辑物业公司
        /// </summary>
        /// <param name="id">编辑的物业公司Id</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑物业公司")]
        [HttpGet]
        public ActionResult EditCompany(int id)
        {
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");

            //获取要编辑的物业公司
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (company != null)
            {
                //初始化返回页面的模型
                SetPropertyCompanyModel model = new SetPropertyCompanyModel()
                {
                    Id = company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    Content = company.Content,
                    Tel = company.Tel,
                    Img = company.Img
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CompanyList");
            }
        }

        /// <summary>
        /// 编辑物业公司
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
                    company.Name = model.Name;
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

        /// <summary>
        /// 物业公司详细
        /// </summary>
        /// <param name="id">查看的物业公司Id</param>
        /// <returns>详细结果返回</returns>
        [BreadCrumb(Label = "物业公司详细")]
        [HttpGet]
        public ActionResult CompanyDetail(int id)
        {
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            //获取要查看的物业公司介绍
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (company != null)
            {
                return View(company);
            }
            else
            {
                return RedirectToAction("CompanyList");
            }
        }

        /// <summary>
        /// 物业总公司管理员设置
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label="设置管理员")]
        [HttpGet]
        public ActionResult SetCompanyAdministrator(int id)
        {
            //获取要设置的物业总公司
            IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            T_Company company = propertyCompanyBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (company != null)
            {
                //返回初始化模型页面
                CompanyUserModel model = new CompanyUserModel();
                model.CompanyId = company.Id;
                model.CompanyName = company.Name;
                ViewBag.Admins = company.CompanyUsers.Where(u => u.IsMgr == ConstantParam.USER_ROLE_MGR).Select(u => u.UserName).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("CompanyList");
            }
        }

        /// <summary>
        /// 设置管理员提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetCompanyAdministrator(CompanyUserModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyUserBLL propertyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                T_CompanyUser companyUser = new T_CompanyUser()
                {
                    CompanyId = model.CompanyId,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = PropertyUtils.GetMD5Str(model.Password),
                    IsMgr = ConstantParam.USER_ROLE_MGR,
                    DelFlag = ConstantParam.DEL_FLAG_DEFAULT,
                };

                //为管理员添加角色
                ICompanyRoleBLL roleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
                var role = roleBll.GetEntity(r => r.IsSystem == ConstantParam.USER_ROLE_MGR && r.CompanyId == model.CompanyId);
                if (role != null)
                {
                    companyUser.CompanyUserRoles.Add(new R_CompanyUserRole()
                    {
                        RoleId = role.Id,
                    });
                }
                //创建管理员
                propertyUserBll.Save(companyUser);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证指定物业用户名称是否存在
        /// </summary>
        /// <param name="UserName">物业用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        [HttpPost]
        public ContentResult RemoteUserCheckExist(string userName)
        {
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            // 用户名已存在
            if (companyUserBll.Exist(m => m.UserName == userName && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
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
