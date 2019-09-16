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
using System.Web.Security;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 平台用户管理控制器
    /// </summary>
    public class PlatformUserController : BaseController
    {
        /// <summary>
        /// 平台用户列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "用户列表")]
        [HttpGet]
        public ActionResult UserList(PlatformUserSearchModel model)
        {
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            Expression<Func<T_PlatformUser, bool>> where = u => (string.IsNullOrEmpty(model.TrueName) ? true : u.TrueName.Contains(model.TrueName)) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
            //排序模型
            var sortModel = this.SettingSorting("Id", false);
            var list = platformUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }

        /// <summary>
        /// 添加平台用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增用户")]
        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        /// <summary>
        /// 添加平台用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(PlatformUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
                T_PlatformUser platformUser = new T_PlatformUser()
                {
                    UserName = model.UserName,
                    TrueName = model.TrueName,
                    Password = PropertyUtils.GetMD5Str(model.Password),
                    Memo = model.Memo,
                    Tel = model.Tel,
                    Phone = model.Phone,
                    Email = model.Email
                };
                // 保存到数据库
                platformUserBll.Save(platformUser);

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
        /// 删除指定平台用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteUser(int? id)
        {
            JsonModel jm = new JsonModel();
            //参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (userInfo != null)
            {
                // 修改指定用户记录中的已删除标识
                userInfo.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                platformUserBll.Update(userInfo);
                //操作日志
                jm.Content = "删除平台用户 " + userInfo.TrueName;
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改指定平台用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑用户")]
        [HttpGet]
        public ActionResult EditUser(int? id)
        {
            // 参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                PlatformUserModel platformUserModel = new PlatformUserModel();
                platformUserModel.UserId = userInfo.Id;
                platformUserModel.UserName = userInfo.UserName;
                platformUserModel.TrueName = userInfo.TrueName;
                platformUserModel.Memo = userInfo.Memo;
                platformUserModel.Tel = userInfo.Tel;
                platformUserModel.Phone = userInfo.Phone;
                platformUserModel.Email = userInfo.Email;
                platformUserModel.HeadPath = userInfo.HeadPath;
                return View(platformUserModel);
            }
            else
            {
                return RedirectToAction("UserList");
            }
        }

        /// <summary>
        /// 修改平台用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUser(PlatformUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
                T_PlatformUser platformUser = platformUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (platformUser != null)
                {
                    platformUser.UserName = model.UserName;
                    platformUser.TrueName = model.TrueName;
                    platformUser.Memo = model.Memo;
                    platformUser.Tel = model.Tel;
                    platformUser.Phone = model.Phone;
                    platformUser.Email = model.Email;

                    // 保存到数据库
                    platformUserBll.Update(platformUser);

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
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (userInfo != null)
            {
                Random r = new Random();
                int radVal = r.Next(100, 1000);
                userInfo.Password = PropertyUtils.GetMD5Str(userInfo.UserName + radVal);
                // 恢复初始密码值
                platformUserBll.Update(userInfo);

                // 给用户发送邮件
                PropertyUtils.SendEmail(userInfo.Email, userInfo.UserName, "物业生活管理系统 用户密码重置", "您的用户密码已重置为" + userInfo.UserName + radVal + ", 请及时修改密码！");
                //操作日志
                jm.Content = "平台用户" + userInfo.TrueName + "密码一键重置成功";
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 平台用户角色查看
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "查看角色")]
        [HttpGet]
        public ActionResult ScanRole(int id)
        {
            // 创建平台用户角色模型
            PlatformUserRoleModel userRoleModel = new PlatformUserRoleModel();

            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            userRoleModel.User = new PlatformUserModel()
            {
                UserName = userInfo.UserName,
                UserId = userInfo.Id,
                TrueName = userInfo.TrueName
            };

            // 获取用户已分配的角色id列表
            var selectedRoleList = userInfo.PlatformUserRoles.Select(m => m.RoleId).ToList();
            userRoleModel.RoleIds = selectedRoleList;

            // 获取所有平台角色
            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = platformRoleBll.GetList(p => true, sortModel.SortName, sortModel.IsAsc).ToList();
            userRoleModel.RoleList = roleList;

            return View(userRoleModel);
        }

        /// <summary>
        /// 平台用户分配角色
        /// </summary> 
        /// <returns></returns>
        [BreadCrumb(Label = "分配角色")]
        [HttpGet]
        public ActionResult ConfigRole(int id)
        {
            // 创建平台用户角色模型
            PlatformUserRoleModel userRoleModel = new PlatformUserRoleModel();

            // 获取指定id的平台用户模型
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            T_PlatformUser platformUser = platformUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            userRoleModel.User = new PlatformUserModel()
            {
                UserId = platformUser.Id,
                UserName = platformUser.UserName,
                TrueName = platformUser.TrueName,
                Tel = platformUser.Tel,
                Phone = platformUser.Phone,
                Memo = platformUser.Memo,
                Email = platformUser.Email
            };

            // 获取所有平台角色
            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = platformRoleBll.GetList(p => p.IsSystem == ConstantParam.USER_ROLE_DEFAULT, sortModel.SortName, sortModel.IsAsc).ToList();
            userRoleModel.RoleList = roleList;

            //获取该用户已分配的角色id的集合
            userRoleModel.RoleIds = platformUser.PlatformUserRoles.Select(m => m.RoleId).ToList();

            return View(userRoleModel);
        }

        /// <summary>
        /// 平台用户分配角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfigRole(UserConfigRoleModel model)
        {
            JsonModel jm = new JsonModel();

            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            //获取要分配角色的平台用户
            T_PlatformUser user = platformUserBll.GetEntity(m => m.Id == model.userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            // 新建用户角色关联表
            List<R_PlatformUserRole> roles = new List<R_PlatformUserRole>();
            if (model.ids != null)
            {
                //没有设置任何角色 则不执行循环操作
                foreach (var id in model.ids)
                {
                    R_PlatformUserRole item = new R_PlatformUserRole() { UserId = model.userId, RoleId = id };
                    roles.Add(item);
                }
            }

            //修改平台用户对应的角色集合
            if (platformUserBll.ConfigRole(user, roles))
            {
                jm.Content = "平台用户 " + user.TrueName + " 分配角色";
            }
            else
            {
                jm.Msg = "分配角色失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 远程验证指定平台用户名称是否存在
        /// </summary>
        /// <param name="UserName">平台用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        public ContentResult RemoteCheckExist(int userId, string userName)
        {
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            // 用户名已存在
            if (platformUserBll.Exist(m => m.UserName == userName && m.Id != userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
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
        /// 平台用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "设置头像")]
        public ActionResult UploadPic(int id)
        {
            JsonModel jm = new JsonModel();
            IPlatformUserBLL UserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == id);

            //用户存在
            if (user != null)
            {
                PlatformUserModel userModel = new PlatformUserModel()
                {
                    UserId = user.Id,
                    HeadPath = user.HeadPath
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
        /// 平台用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();

            string directory = Server.MapPath(ConstantParam.PLATFORM_USER_HEAD_DIR);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
            var path = Path.Combine(directory, fileName);

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] datas = Convert.FromBase64String(data);
                    bw.Write(datas);
                    bw.Close();
                }
            }

            ////生成缩略图
            //string thumpFile = DateTime.Now.Millisecond + PSPlatformUtils.CreateValidateCode(4) + ".jpg";
            //var thumpPath = Path.Combine(Server.MapPath("~/Upload/User"), thumpFile);
            //PSPlatformUtils.getThumImage(path, 18, 3, thumpPath);

            IPlatformUserBLL UserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);

            //用户存在
            if (user != null)
            {
                string oldFile = user.HeadPath;
                user.HeadPath = ConstantParam.PLATFORM_USER_HEAD_DIR + fileName;
                UserBll.Update(user);
                //删除旧头像
                if (!string.IsNullOrEmpty(oldFile))
                {
                    oldFile = Server.MapPath(oldFile);
                    FileInfo f = new FileInfo(oldFile);
                    if (f.Exists)
                        f.Delete();
                }
            }
            //用户不存在 
            else
            {
                jm.Msg = "用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
