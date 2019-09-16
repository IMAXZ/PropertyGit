using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Filter;
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
    /// 平台已登录用户个人信息设置控制器
    /// </summary>
    [PlatformLoggedAccountActionFilter]
    public class PlatformLoggedAccountController : Controller
    {
        /// <summary>
        /// 登录平台用户-个人信息设置
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑个人信息")]
        [HttpGet]
        public ActionResult SetPlatUserInfo()
        {
            // 获取Session Model
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = model.UserID;

            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                LoggedInAccountModel platformUserModel = new LoggedInAccountModel();
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
                return RedirectToAction("Index", "Platform");
            }

        }
    

        /// <summary>
        /// 登录平台用户个人信息设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPlatUserInfo(LoggedInAccountModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                // 获取Session Model
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

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

                    //更新SessionModel中的最新个人信息
                    sessionModel.TrueName = model.TrueName;

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
        /// 平台用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "头像上传")]
        [HttpGet]
        public ActionResult UploadPlatPic(int id)
        {
            JsonModel jm = new JsonModel();
            IPlatformUserBLL UserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == id);

            //用户存在
            if (user != null)
            {
                LoggedInAccountModel userModel = new LoggedInAccountModel()
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
        public ActionResult UploadPlatPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];

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


            // 若当前登录用户为平台用户
            IPlatformUserBLL UserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);

            //用户存在
            if (user != null)
            {
                string oldFile = user.HeadPath;
                user.HeadPath = ConstantParam.PLATFORM_USER_HEAD_DIR + fileName;
                UserBll.Update(user);

                //更新SessionModel中的最新个人信息
                sessionModel.HeadPath = ConstantParam.PLATFORM_USER_HEAD_DIR + fileName;

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

        /// <summary>
        /// 登录平台用户修改密码
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "修改个人密码")]
        [HttpGet]
        public ActionResult EditPlatUserPwd()
        {
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = sessionModel.UserID;

            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            var userInfo = platformUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                AccountPasswordChangeModel platformUserModel = new AccountPasswordChangeModel();
                platformUserModel.UserId = userInfo.Id;
                platformUserModel.UserName = userInfo.UserName;
                return View(platformUserModel);
            }
            else
            {
                return RedirectToAction("Index", "Platform");
            }
        }

        /// <summary>
        /// 登录平台用户修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPlatUserPwd(AccountPasswordChangeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

                // 若当前登录用户为平台用户
                IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
                T_PlatformUser platformUser = platformUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (platformUser != null)
                {
                    platformUser.Password = PropertyUtils.GetMD5Str(model.Password);
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
    }
}
