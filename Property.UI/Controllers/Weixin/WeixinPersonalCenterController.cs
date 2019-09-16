using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 微信个人中心模块
    /// </summary>
    public class WeixinPersonalCenterController : WeixinBaseController
    {
        /// <summary>
        /// 个人中心主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var owner = GetCurrentUser();
            if (owner != null)
            {
                ViewBag.HeadPath = string.IsNullOrEmpty(owner.HeadPath) ? "/Images/Weixin/personal_top_default_head.png" : owner.HeadPath;
                ViewBag.UserName = owner.UserName;
                return View();
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonInfo()
        {
            WeixinApiInit();
            var owner = GetCurrentUser();
            if (owner != null)
            {
                PersonInfoModel model = new PersonInfoModel();
                model.HeadPath = owner.HeadPath;
                model.UserName = owner.UserName;
                model.Gender = owner.Gender;
                model.GenderList = GetGenderList();
                model.Phone = owner.Phone;
                model.Email = owner.Email;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserNameInfo()
        {
            var owner = GetCurrentUser();
            UserNameInfoModel model = new UserNameInfoModel();
            model.Id = owner.Id;
            model.UserName = owner.UserName;
            return View(model);
        }

        /// <summary>
        /// 修改用户名 提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserNameInfo(UserNameInfoModel model)
        {
            JsonModel jm = new JsonModel();
            var owner = GetCurrentUser();

            if (owner != null)
            {
                owner.UserName = model.UserName;
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                ownerBll.Update(owner);
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改头像 提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult HeadImgInfo(string Img)
        {
            JsonModel jm = new JsonModel();

            try
            {
                var owner = GetCurrentUser();
                if (owner != null)
                {
                    if (!string.IsNullOrEmpty(Img))
                    {
                        string oldImgPath = owner.HeadPath;

                        //头像保存路径
                        owner.HeadPath = GetMultimedia(ConstantParam.OWNER_HEAD_DIR, Img);
                        IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                        userBll.Update(owner);

                        //删除旧头像
                        if (!string.IsNullOrEmpty(oldImgPath))
                        {
                            FileInfo f = new FileInfo(Server.MapPath(oldImgPath));
                            if (f.Exists)
                                f.Delete();
                        }
                    }
                }
                else
                {
                    jm.Msg = "用户不存在";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PhoneInfo()
        {
            var owner = GetCurrentUser();

            PhoneInfoModel model = new PhoneInfoModel();
            model.Id = owner.Id;
            return View(model);
        }

        /// <summary>
        /// 修改手机号 提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PhoneInfo(PhoneInfoModel model)
        {
            JsonModel jm = new JsonModel();

            var owner = GetCurrentUser();

            if (owner != null)
            {
                //判断验证码是否正确
                IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.Phone && v.ActionCode == 0);


                //如果验证码不准确
                if (val == null && model.VerityCode != val.ValidateCode)
                {
                    jm.Msg = APIMessage.VALIDATE_ERROR;
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }

                //验证码已失效
                if (val.InvalidTime < DateTime.Now)
                {
                    jm.Msg = APIMessage.VALIDATE_INVALID;
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                owner.Phone = model.Phone;

                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                ownerBll.Update(owner);
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmailInfo()
        {
            var owner = GetCurrentUser();

            EmailInfoModel model = new EmailInfoModel();
            model.Id = owner.Id;
            model.Email = owner.Email;
            return View(model);
        }

        /// <summary>
        /// 修改邮箱 提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EmailInfo(EmailInfoModel model)
        {
            JsonModel jm = new JsonModel();
            var owner = GetCurrentUser();

            if (owner != null)
            {
                owner.Email = model.Email;
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                ownerBll.Update(owner);
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 性别提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GenderInfo(GenderInfoModel model)
        {
            JsonModel jm = new JsonModel();
            var owner = GetCurrentUser();
            if (owner != null)
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                if (model.Gerder != null)
                {
                    owner.Gender = model.Gerder;
                    ownerBll.Update(owner);
                }
                else
                {
                    owner.Gender = null;
                    ownerBll.Update(owner);
                }
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 性别下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetGenderList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "男",
                Value = ConstantParam.GENDER_ZERO.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "女",
                Value = ConstantParam.GENDER_ONE.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 密码管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PasswordMgr()
        {
            var owner = GetCurrentUser();
            if (owner != null)
            {
                PasswordModel model = new PasswordModel();
                model.Password = owner.Password;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 密码管理提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PasswordMgr(PasswordModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                var owner = GetCurrentUser();
                if (owner != null)
                {
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    if (!string.IsNullOrEmpty(owner.Password))
                    {
                        string md5Str = PropertyUtils.GetMD5Str(model.BeforePassword);
                        if (md5Str != owner.Password)
                        {
                            jm.Msg = "原密码不正确";
                        }
                        else
                        {
                            string Md5str = PropertyUtils.GetMD5Str(model.NewPassword);
                            owner.Password = Md5str;
                            ownerBll.Update(owner);
                        }
                    }
                    else
                    {
                        string md5str = PropertyUtils.GetMD5Str(model.sPassword);
                        owner.Password = md5str;
                        ownerBll.Update(owner);
                    }
                }
                else
                {
                    jm.Msg = "该用户不存在";
                }
                return Json(jm, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //jm.Msg = "请重新输入";
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR; ;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
