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
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    public class CompanyUserController : BaseController
    {
        /// <summary>
        /// 物业总公司用户列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "物业总公司用户列表")]
        [HttpGet]
        public ActionResult CompanyUserList(SearchModel model)
        {
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            //所属总公司
            int companyId = GetSessionModel().CompanyId.Value;
            Expression<Func<T_CompanyUser, bool>> where = u => (string.IsNullOrEmpty(model.Kword) ? true : (u.TrueName.Contains(model.Kword) || u.UserName.Contains(model.Kword))) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.CompanyId == companyId;

            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = companyUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(list);
        }

        /// <summary>
        /// 添加总公司用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增总公司用户")]
        [HttpGet]
        public ActionResult AddCompanyUser()
        {
            // 获取物业总公司管理员所属的物业总公司
            int companyId = GetSessionModel().CompanyId.Value;
            // 获取指定物业总公司的名称
            IPropertyCompanyBLL propertyCompanyBLL = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            var company = propertyCompanyBLL.GetEntity(m => m.Id == companyId);

            if (company != null)
            {
                CompanyUserModel model = new CompanyUserModel()
                {
                     CompanyName = company.Name
                };

                return View(model);
            }

            return View();
        }

        /// <summary>
        /// 添加物业总公司用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompanyUser(CompanyUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");

                T_CompanyUser propertyUser = new T_CompanyUser()
                {
                    UserName = model.UserName,
                    TrueName = model.TrueName,
                    Password = PropertyUtils.GetMD5Str(model.Password),
                    Memo = model.Memo,
                    Tel = model.Tel,
                    Phone = model.Phone,
                    Email = model.Email,
                    CompanyId = GetSessionModel().CompanyId.Value
                };
                // 保存到数据库
                companyUserBll.Save(propertyUser);

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
        /// 删除指定物业总公司用户
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteCompanyUser(int? id)
        {
            JsonModel jm = new JsonModel();
            //参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            // 根据指定id值获取实体对象
            var companyUserInfo = companyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (companyUserInfo != null)
            {
                // 修改指定用户记录中的已删除标识
                companyUserInfo.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                companyUserBll.Update(companyUserInfo);
                //操作日志
                jm.Content = "删除物业用户 " + companyUserInfo.TrueName;
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改指定物业总公司用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑物业总公司用户")]
        [HttpGet]
        public ActionResult EditCompanyUser(int? id)
        {
            // 参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            var companyUserInfo = companyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (companyUserInfo != null)
            {
                CompanyUserModel companyUserModel = new CompanyUserModel();

                companyUserModel.CompanyUserId = companyUserInfo.Id;
                companyUserModel.UserName = companyUserInfo.UserName;
                companyUserModel.TrueName = companyUserInfo.TrueName;
                companyUserModel.Memo = companyUserInfo.Memo;
                companyUserModel.Tel = companyUserInfo.Tel;
                companyUserModel.Phone = companyUserInfo.Phone;
                companyUserModel.Email = companyUserInfo.Email;
                companyUserModel.HeadPath = companyUserInfo.HeadPath;
                companyUserModel.CompanyId = GetSessionModel().CompanyId.Value;
                // 获取指定小区的名称
                IPropertyCompanyBLL propertyCompanyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
                var propertyCompany = propertyCompanyBll.GetEntity(m => m.Id == companyUserModel.CompanyId);
                if (propertyCompany != null)
                {
                    companyUserModel.CompanyName = propertyCompany.Name;
                }

                return View(companyUserModel);
            }
            else
            {
                return RedirectToAction("UserList");
            }
        }

        /// <summary>
        /// 编辑物业总公司用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCompanyUser(CompanyUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == model.CompanyUserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (companyUser != null)
                {
                    companyUser.UserName = model.UserName;
                    companyUser.TrueName = model.TrueName;
                    companyUser.Memo = model.Memo;
                    companyUser.Tel = model.Tel;
                    companyUser.Phone = model.Phone;
                    companyUser.Email = model.Email;
                    // 保存到数据库
                    companyUserBll.Update(companyUser);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该用户不存在";
                }
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 物业总公司用户角色查看
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "查看角色")]
        [HttpGet]
        public ActionResult ScanRole(int id)
        {
            // 创建物业总公司用户角色模型
            CompanyUserRoleModel companyUserRoleModel = new CompanyUserRoleModel();

            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            // 根据指定id值获取实体对象
            var companyUserInfo = companyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            companyUserRoleModel.User = new CompanyUserModel()
            {
                UserName = companyUserInfo.UserName,
                CompanyUserId = companyUserInfo.Id,
                TrueName = companyUserInfo.TrueName
            };

            // 获取物业总公司用户已分配的角色id列表
            var selectedRoleList = companyUserInfo.CompanyUserRoles.Select(m => m.RoleId).ToList();
            companyUserRoleModel.RoleIds = selectedRoleList;

            // 获取所有物业总公司角色
            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = companyRoleBll.GetList(p => true, sortModel.SortName, sortModel.IsAsc).ToList();
            companyUserRoleModel.RoleList = roleList;

            return View(companyUserRoleModel);
        }

        /// <summary>
        /// 物业总公司用户分配角色
        /// </summary> 
        /// <returns></returns>
        [BreadCrumb(Label = "分配角色")]
        [HttpGet]
        public ActionResult ConfigRole(int id)
        {
            // 创建物业总公司用户角色模型
            CompanyUserRoleModel companyUserRoleModel = new CompanyUserRoleModel();

            // 获取指定id的物业总公司用户模型
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            companyUserRoleModel.User = new CompanyUserModel()
            {
                CompanyUserId = companyUser.Id,
                UserName = companyUser.UserName,
                TrueName = companyUser.TrueName,
                Tel = companyUser.Tel,
                Phone = companyUser.Phone,
                Memo = companyUser.Memo,
                Email = companyUser.Email
            };

            // 获取本总公司中所有的角色
            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = companyRoleBll.GetList(p => p.CompanyId  == companyUser.CompanyId &&
            p.IsSystem == ConstantParam.USER_ROLE_DEFAULT, sortModel.SortName, sortModel.IsAsc).ToList();
            companyUserRoleModel.RoleList = roleList;

            //获取该总公司用户已分配的角色id的集合
            companyUserRoleModel.RoleIds = companyUser.CompanyUserRoles.Select(m => m.RoleId).ToList();

            return View(companyUserRoleModel);
        }

        /// <summary>
        /// 物业总公司用户分配角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfigRole(CompanyUserConfigRoleModel model)
        {
            JsonModel jm = new JsonModel();

            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            //获取要分配角色的物业总公司用户
            T_CompanyUser user = companyUserBll.GetEntity(m => m.Id == model.userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            // 新建物业总公司用户角色关联表
            List<R_CompanyUserRole> roles = new List<R_CompanyUserRole>();

            if (model.ids != null)
            {
                //没有设置任何角色 则不执行循环操作
                foreach (var id in model.ids)
                {
                    R_CompanyUserRole item = new R_CompanyUserRole() { UserId = model.userId, RoleId = id };
                    roles.Add(item);
                }
            }

            //修改物业用户对应的角色集合
            if (companyUserBll.ConfigRole(user, roles))
            {
                jm.Content = "物业用户 " + user.TrueName + " 分配角色";
            }
            else
            {
                jm.Msg = "分配角色失败";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 恢复初始密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(int? id)
        {
            JsonModel jm = new JsonModel();
            //参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ICompanyUserBLL companyUserBLL = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            // 根据指定id值获取实体对象
            var companyUserInfo = companyUserBLL.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (companyUserInfo != null)
            {
                Random r = new Random();
                int radVal = r.Next(100, 1000);
                companyUserInfo.Password = PropertyUtils.GetMD5Str(companyUserInfo.UserName + radVal);
                // 恢复初始密码值
                companyUserBLL.Update(companyUserInfo);

                // 给物业总公司用户发送邮件
                PropertyUtils.SendEmail(companyUserInfo.Email, companyUserInfo.UserName, "物业总公司管理系统 用户密码重置", "您的用户密码已重置为" + companyUserInfo.UserName + radVal + ", 请及时修改密码！");
                //操作日志
                jm.Content = "物业总公司用户" + companyUserInfo.TrueName + "密码一键重置成功";
            }
            else
            {
                jm.Msg = "该用户不存在";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证指定物业总公司用户名称是否存在
        /// </summary>
        /// <param name="UserName">物业总公司用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        public ContentResult RemoteCheckExist(int userId, string userName)
        {
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            // 用户名已存在
            if (companyUserBll.Exist(m => m.UserName == userName && m.Id != userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
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
        /// 物业总公司用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "设置头像")]
        public ActionResult UploadPic(int id)
        {
            JsonModel jm = new JsonModel();
            ICompanyUserBLL UserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            var companyUser = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == id);

            //总公司用户存在
            if (companyUser != null)
            {
                CompanyUserModel userModel = new CompanyUserModel()
                {
                    CompanyUserId = companyUser.Id,
                    HeadPath = companyUser.HeadPath
                };
                return View(userModel);
            }
            //用户不存在 
            else
            {
                jm.Msg = "用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 物业总公司用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();

            string directory = Server.MapPath(ConstantParam.COMPANY_USER_HEAD_DIR);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
            var path = Path.Combine(directory, fileName);

            using (FileStream fs = new FileStream(path, FileMode.Create))//使用指定的路径初始化实例
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] datas = Convert.FromBase64String(data);
                    bw.Write(datas);
                    bw.Close();
                }
            }

            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            var companyUser = companyUserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);

            //总公司用户存在
            if (companyUser != null)
            {
                string oldFile = companyUser.HeadPath;
                companyUser.HeadPath = ConstantParam.COMPANY_USER_HEAD_DIR + fileName;
                companyUserBll.Update(companyUser);
                //删除旧头像
                if (!string.IsNullOrEmpty(oldFile))
                {
                    oldFile = Server.MapPath(oldFile);
                    FileInfo f = new FileInfo(oldFile);

                    if (f.Exists)
                        f.Delete();
                }
            }
            //总公司用户不存在 
            else
            {
                jm.Msg = "总公司用户不存在";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
