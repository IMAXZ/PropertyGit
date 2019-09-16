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
    /// 物业已登录用户个人信息设置控制器
    /// </summary>
    [PropertyLoggedAccountActionFilter]
    public class PropertyLoggedAccountController : Controller
    {
        /// <summary>
        /// 登录物业用户-个人信息设置
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑个人信息")]
        [HttpGet]
        public ActionResult SetPropUserInfo()
        {
            // 获取Session Model
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = model.UserID;

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (userInfo != null)
            {
                LoggedInAccountModel propertyUserModel = new LoggedInAccountModel();
                propertyUserModel.UserId = userInfo.Id;
                propertyUserModel.UserName = userInfo.UserName;
                propertyUserModel.TrueName = userInfo.TrueName;
                propertyUserModel.Memo = userInfo.Memo;
                propertyUserModel.Tel = userInfo.Tel;
                propertyUserModel.Phone = userInfo.Phone;
                propertyUserModel.Email = userInfo.Email;
                propertyUserModel.HeadPath = userInfo.HeadPath;
                propertyUserModel.PlaceId = model.PropertyPlaceId.Value;
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
                return RedirectToAction("Index", "Property");
            }
        }


        /// <summary>
        /// 登录物业用户个人信息设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPropUserInfo(LoggedInAccountModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                // 获取Session Model
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

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
        /// 物业用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "头像上传")]
        [HttpGet]
        public ActionResult UploadPropPic(int id)
        {
            JsonModel jm = new JsonModel();
            IPropertyUserBLL UserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
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
        /// 物业用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPropPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];

            string directory = Server.MapPath(ConstantParam.PROPERTY_USER_HEAD_DIR);

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

            // 若当前登录用户为物业用户
            IPropertyUserBLL UserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var user = UserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);
            //用户存在
            if (user != null)
            {
                string oldFile = user.HeadPath;
                user.HeadPath = ConstantParam.PROPERTY_USER_HEAD_DIR + fileName;
                UserBll.Update(user);

                //更新SessionModel中的最新个人信息
                sessionModel.HeadPath = ConstantParam.PROPERTY_USER_HEAD_DIR + fileName;

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
        /// 登录物业用户修改密码
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "修改个人密码")]
        [HttpGet]
        public ActionResult EditPropUserPwd()
        {
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = sessionModel.UserID;

            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var userInfo = propertyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (userInfo != null)
            {
                AccountPasswordChangeModel propertyUserModel = new AccountPasswordChangeModel();
                propertyUserModel.UserId = userInfo.Id;
                propertyUserModel.UserName = userInfo.UserName;
                return View(propertyUserModel);
            }
            else
            {
                return RedirectToAction("Index", "Property");
            }
        }

        /// <summary>
        /// 登录物业用户修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPropUserPwd(AccountPasswordChangeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

                // 若当前登录用户为物业用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (propertyUser != null)
                {
                    propertyUser.Password = PropertyUtils.GetMD5Str(model.Password);
                    propertyUser.Token = null;
                    propertyUser.TokenInvalidTime = null;
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
    }
}
