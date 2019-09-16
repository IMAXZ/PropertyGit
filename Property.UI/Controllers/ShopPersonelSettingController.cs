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
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 门店用户个人信息修改控制器
    /// </summary>
    [ShopPlatformActionFilter]
    public class ShopPersonelSettingController : Controller
    {
        /// <summary>
        /// 门店用户个人设置
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑个人信息")]
        [HttpGet]
        public ActionResult SetShopUserInfo()
        {
            // 获取Session Model
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = model.UserID;
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            var userInfo = shopUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                LoggedInAccountModel shopUserModel = new LoggedInAccountModel();
                shopUserModel.UserId = userInfo.Id;
                shopUserModel.UserName = userInfo.UserName;
                shopUserModel.TrueName = userInfo.TrueName;
                shopUserModel.Phone = userInfo.Phone;
                shopUserModel.Email = userInfo.Email;
                shopUserModel.Memo = userInfo.Memo;
                shopUserModel.HeadPath = userInfo.HeadPath;
                return View(shopUserModel);
            }
            else
            {
                return RedirectToAction("Index", "ShopPlatform");
            }

        }
        /// <summary>
        /// 登录门店用户个人信息设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetShopUserInfo(LoggedInAccountModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                // 获取Session Model
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);//查询
                //如果数据为空，将输入数据写入
                if (shopUser != null)
                {
                    shopUser.TrueName = model.TrueName;
                    shopUser.Phone = model.Phone;
                    shopUser.Email = model.Email;
                    shopUser.Memo = model.Memo;
                    // 保存到数据库
                    shopUserBll.Update(shopUser);

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
        /// 门店用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "头像上传")]
        [HttpGet]
        public ActionResult UploadShopPic(int id)
        {
            JsonModel jm = new JsonModel();
            IShopUserBLL UserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
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
        /// 门店用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadShopPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];

            string directory = Server.MapPath(ConstantParam.ShOPFORM_USER_HEAD_DIR);

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


            // 若当前登录门店用户为门店平台用户
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            var user = shopUserBll.GetEntity(m => m.DelFlag == 0 && m.Id == userId);

            //用户存在
            if (user != null)
            {
                string oldFile = user.HeadPath;
                user.HeadPath = ConstantParam.ShOPFORM_USER_HEAD_DIR + fileName;
                shopUserBll.Update(user);

                //更新SessionModel中的最新个人信息
                sessionModel.HeadPath = ConstantParam.ShOPFORM_USER_HEAD_DIR + fileName;

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
        /// 登录门店用户修改密码
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "修改个人密码")]
        [HttpGet]
        public ActionResult EditShopUserPwd()
        {
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = sessionModel.UserID;

            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            var userInfo = shopUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (userInfo != null)
            {
                AccountPasswordChangeModel shopUserModel = new AccountPasswordChangeModel();
                shopUserModel.UserId = userInfo.Id;
                shopUserModel.UserName = userInfo.UserName;
                return View(shopUserModel);
            }
            else
            {
                return RedirectToAction("Index", "ShopPlatform");
            }
        }

        /// <summary>
        /// 登录门店用户修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditShopUserPwd(AccountPasswordChangeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                var id = sessionModel.UserID;

                // 若当前登录用户为门店用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (shopUser != null)
                {
                    shopUser.Password = PropertyUtils.GetMD5Str(model.Password);
                    shopUser.Token = null;
                    shopUser.TokenInvalidTime = null;

                    // 保存到数据库
                    shopUserBll.Update(shopUser);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该门店用户不存在";
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
