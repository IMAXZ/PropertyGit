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
    /// <summary>
    /// 物业管理员用户控制器类
    /// </summary>
    public class PropertyUserController : BaseController
    {
        /// <summary>
        /// 物业用户列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "物业用户列表")]
        [HttpGet]
        public ActionResult UserList(SearchModel model)
        {
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_PropertyUser, bool>> where = u => (string.IsNullOrEmpty(model.Kword) ? true : (u.TrueName.Contains(model.Kword) || u.UserName.Contains(model.Kword))) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlaceId == propertyPlaceId;

            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = propertyUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(list);
        }

        /// <summary>
        /// 添加平台用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增物业用户")]
        [HttpGet]
        public ActionResult AddUser()
        {
            // 获取物业管理员所属的物业小区
            int placeId = GetSessionModel().PropertyPlaceId.Value;
            // 获取指定小区的名称
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = propertyPlaceBll.GetEntity(m => m.Id == placeId);
            if (place != null)
            {
                PropertyUserModel model = new PropertyUserModel()
                {
                    PlaceName = place.Name
                };
                return View(model);
            }
            return View();
        }

        /// <summary>
        /// 添加物业用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(PropertyUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = new T_PropertyUser()
                {
                    UserName = model.UserName,
                    TrueName = model.TrueName,
                    Password = PropertyUtils.GetMD5Str(model.Password),
                    Memo = model.Memo,
                    Tel = model.Tel,
                    Phone = model.Phone,
                    Email = model.Email,
                    PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value
                };
                // 保存到数据库
                propertyUserBll.Save(propertyUser);

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
        /// 删除指定物业用户
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteUser(int? id)
        {
            JsonModel jm = new JsonModel();
            //参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                // 修改指定用户记录中的已删除标识
                userInfo.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                propertyUserBll.Update(userInfo);
                //操作日志
                jm.Content = "删除物业用户 " + userInfo.TrueName;
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改指定物业用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑物业用户")]
        [HttpGet]
        public ActionResult EditUser(int? id)
        {
            // 参数校验
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                PropertyUserModel propertyUserModel = new PropertyUserModel();
                propertyUserModel.UserId = userInfo.Id;
                propertyUserModel.UserName = userInfo.UserName;
                propertyUserModel.TrueName = userInfo.TrueName;
                propertyUserModel.Memo = userInfo.Memo;
                propertyUserModel.Tel = userInfo.Tel;
                propertyUserModel.Phone = userInfo.Phone;
                propertyUserModel.Email = userInfo.Email;
                propertyUserModel.HeadPath = userInfo.HeadPath;
                propertyUserModel.PlaceId = GetSessionModel().PropertyPlaceId.Value;
                // 获取指定小区的名称
                IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = propertyPlaceBll.GetEntity(m => m.Id == propertyUserModel.PlaceId);
                if (place != null)
                {
                    propertyUserModel.PlaceName = place.Name;
                }

                return View(propertyUserModel);
            }
            else
            {
                return RedirectToAction("UserList");
            }
        }

        /// <summary>
        /// 编辑物业用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUser(PropertyUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (propertyUser != null)
                {
                    propertyUser.UserName = model.UserName;
                    propertyUser.TrueName = model.TrueName;
                    propertyUser.Memo = model.Memo;
                    propertyUser.Tel = model.Tel;
                    propertyUser.Phone = model.Phone;
                    propertyUser.Email = model.Email;
                    // 保存到数据库
                    propertyUserBll.Update(propertyUser);

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
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (userInfo != null)
            {
                Random r = new Random();
                int radVal = r.Next(100, 1000);
                userInfo.Password = PropertyUtils.GetMD5Str(userInfo.UserName + radVal);
                // 恢复初始密码值
                propertyUserBll.Update(userInfo);

                // 给物业用户发送邮件
                PropertyUtils.SendEmail(userInfo.Email, userInfo.UserName, "物业生活管理系统 用户密码重置", "您的用户密码已重置为" + userInfo.UserName + radVal + ", 请及时修改密码！");
                //操作日志
                jm.Content = "物业用户" + userInfo.TrueName + "密码一键重置成功";
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 物业用户角色查看
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "查看角色")]
        [HttpGet]
        public ActionResult ScanRole(int id)
        {
            // 创建物业用户角色模型
            PropertyUserRoleModel userRoleModel = new PropertyUserRoleModel();

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            // 根据指定id值获取实体对象
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            userRoleModel.User = new PropertyUserModel()
            {
                UserName = userInfo.UserName,
                UserId = userInfo.Id,
                TrueName = userInfo.TrueName
            };

            // 获取用户已分配的角色id列表
            var selectedRoleList = userInfo.PropertyUserRoles.Select(m => m.RoleId).ToList();
            userRoleModel.RoleIds = selectedRoleList;

            // 获取所有物业角色
            IPropertyRoleBLL propertyRoleBll = BLLFactory<IPropertyRoleBLL>.GetBLL("PropertyRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = propertyRoleBll.GetList(p => true, sortModel.SortName, sortModel.IsAsc).ToList();
            userRoleModel.RoleList = roleList;

            return View(userRoleModel);
        }

        /// <summary>
        /// 物业用户分配角色
        /// </summary> 
        /// <returns></returns>
        [BreadCrumb(Label = "分配角色")]
        [HttpGet]
        public ActionResult ConfigRole(int id)
        {
            // 创建物业用户角色模型
            PropertyUserRoleModel userRoleModel = new PropertyUserRoleModel();

            // 获取指定id的物业用户模型
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            T_PropertyUser propertyUser = propertyUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            userRoleModel.User = new PropertyUserModel()
            {
                UserId = propertyUser.Id,
                UserName = propertyUser.UserName,
                TrueName = propertyUser.TrueName,
                Tel = propertyUser.Tel,
                Phone = propertyUser.Phone,
                Memo = propertyUser.Memo,
                Email = propertyUser.Email
            };

            // 获取本小区中所有的物业角色
            IPropertyRoleBLL propertyRoleBll = BLLFactory<IPropertyRoleBLL>.GetBLL("PropertyRoleBLL");
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var roleList = propertyRoleBll.GetList(p => p.PropertyPlaceId == propertyUser.PropertyPlaceId && 
            p.IsSystem == ConstantParam.USER_ROLE_DEFAULT, sortModel.SortName, sortModel.IsAsc).ToList();
            userRoleModel.RoleList = roleList;

            //获取该用户已分配的角色id的集合
            userRoleModel.RoleIds = propertyUser.PropertyUserRoles.Select(m => m.RoleId).ToList();

            return View(userRoleModel);
        }


        /// <summary>
        /// 物业用户分配角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfigRole(PropertyUserConfigRoleModel model)
        {
            JsonModel jm = new JsonModel();

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            //获取要分配角色的物业用户
            T_PropertyUser user = propertyUserBll.GetEntity(m => m.Id == model.userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            // 新建用户角色关联表
            List<R_PropertyUserRole> roles = new List<R_PropertyUserRole>();
            if (model.ids != null)
            {
                //没有设置任何角色 则不执行循环操作
                foreach (var id in model.ids)
                {
                    R_PropertyUserRole item = new R_PropertyUserRole() { UserId = model.userId, RoleId = id };
                    roles.Add(item);
                }
            }

            //修改物业用户对应的角色集合
            if (propertyUserBll.ConfigRole(user, roles))
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
        /// 远程验证指定物业用户名称是否存在
        /// </summary>
        /// <param name="UserName">物业用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        public ContentResult RemoteCheckExist(int userId, string userName)
        {
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            // 用户名已存在
            if (propertyUserBll.Exist(m => m.UserName == userName && m.Id != userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
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
            IPropertyUserBLL UserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == id);

            //用户存在
            if (user != null)
            {
                PropertyUserModel userModel = new PropertyUserModel()
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
        /// 物业用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();

            string directory = Server.MapPath(ConstantParam.PROPERTY_USER_HEAD_DIR);
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

            ////生成缩略图
            //string thumpFile = DateTime.Now.Millisecond + PSPlatformUtils.CreateValidateCode(4) + ".jpg";
            //var thumpPath = Path.Combine(Server.MapPath("~/Upload/User"), thumpFile);
            //PSPlatformUtils.getThumImage(path, 18, 3, thumpPath);

            IPropertyUserBLL UserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);

            //用户存在
            if (user != null)
            {
                string oldFile = user.HeadPath;
                user.HeadPath = ConstantParam.PROPERTY_USER_HEAD_DIR + fileName;
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
