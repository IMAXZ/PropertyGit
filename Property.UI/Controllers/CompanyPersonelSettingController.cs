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

namespace Property.UI.Controllers
{
    /// <summary>
    /// 总公司个人信息设置控制器
    /// </summary>
    [CompanyPlatformActionFilter]
    public class CompanyPersonelSettingController : Controller
    {
        /// <summary>
        /// 总公司用户个人设置
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑个人信息")]
        [HttpGet]
        public ActionResult SetUserInfo()
        {
            //获取当前用户
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = model.UserID;
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            var companyUser = companyUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (companyUser != null)
            {
                LoggedInAccountModel companyUserModel = new LoggedInAccountModel();
                companyUserModel.UserId = companyUser.Id;
                companyUserModel.UserName = companyUser.UserName;
                companyUserModel.TrueName = companyUser.TrueName;
                companyUserModel.Phone = companyUser.Phone;
                companyUserModel.Email = companyUser.Email;
                companyUserModel.Memo = companyUser.Memo;
                companyUserModel.HeadPath = companyUser.HeadPath;
                return View(companyUserModel);
            }
            else
            {
                return RedirectToAction("Index", "CompanyPlatform");
            }

        }
        /// <summary>
        /// 登录门店用户个人信息设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetUserInfo(LoggedInAccountModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //获取要编辑个人信息的总公司用户
                ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (companyUser != null)
                {
                    companyUser.TrueName = model.TrueName;
                    companyUser.Phone = model.Phone;
                    companyUser.Email = model.Email;
                    companyUser.Memo = model.Memo;
                    // 保存到数据库
                    companyUserBll.Update(companyUser);

                    //更新SessionModel中的最新个人信息
                    UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
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
        /// 总公司用户头像上传
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "设置个人头像")]
        [HttpGet]
        public ActionResult UploadHeadPic(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要上传头像的总公司用户
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //用户存在
            if (companyUser != null)
            {
                LoggedInAccountModel userModel = new LoggedInAccountModel()
                {
                    UserId = companyUser.Id,
                    HeadPath = companyUser.HeadPath
                };
                return View(userModel);
            }
            //用户不存在 
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 总公司用户头像上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadHeadPic(string data, int userId)
        {
            JsonModel jm = new JsonModel();
            //保存头像文件
            string directory = Server.MapPath(ConstantParam.COMPANY_USER_HEAD_DIR);
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

            //获取要上传头像的总公司用户
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == userId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            //用户存在
            if (companyUser != null)
            {
                string oldFile = companyUser.HeadPath;
                companyUser.HeadPath = ConstantParam.COMPANY_USER_HEAD_DIR + fileName;
                companyUserBll.Update(companyUser);

                //更新SessionModel中的最新个人信息
                UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
                sessionModel.HeadPath = ConstantParam.COMPANY_USER_HEAD_DIR + fileName;

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
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登录总公司用户修改密码
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "修改个人密码")]
        [HttpGet]
        public ActionResult EditUserPwd()
        {
            UserSessionModel sessionModel = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            var id = sessionModel.UserID;

            //获取要修改密码的总公司用户
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (companyUser != null)
            {
                AccountPasswordChangeModel model = new AccountPasswordChangeModel();
                model.UserId = companyUser.Id;
                model.UserName = companyUser.UserName;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "CompanyPlatform");
            }
        }

        /// <summary>
        /// 登录总公司用户修改密码提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUserPwd(AccountPasswordChangeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //获取要修改密码的用户
                ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                T_CompanyUser companyUser = companyUserBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (companyUser != null)
                {
                    companyUser.Password = PropertyUtils.GetMD5Str(model.Password);
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
    }
}
