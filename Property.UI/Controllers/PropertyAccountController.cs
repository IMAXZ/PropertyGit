using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业账户设定控制器
    /// </summary>
    public class PropertyAccountController : BaseController
    {
        /// <summary>
        /// 物业账户表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "物业账户设定")]
        public ActionResult AccountList()
        {
            PropertyAccountModel model = new PropertyAccountModel();
            var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");

            //获取微信账户
            var propertyAccount1 = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取支付宝账户
            var propertyAccount2 = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //微信不为空，支付宝为空
            if (propertyAccount1 != null && propertyAccount2 == null)
            {
                model.WeChatNumber = propertyAccount1.Number;
                model.WeChatMerchantNo = propertyAccount1.MerchantNo;
                model.WeChatKey = propertyAccount1.AccountKey;

                return View(model);
            }

            //支付宝不为空，微信为空
            if (propertyAccount2 != null && propertyAccount1 == null)
            {
                model.AlipayNumber = propertyAccount2.Number;
                model.AlipayMerchantNo = propertyAccount2.MerchantNo;
                model.AlipayKey = propertyAccount2.AccountKey;

                return View(model);
            }

            //微信，支付宝都不为空
            if (propertyAccount1 != null && propertyAccount2 != null)
            {
                model.WeChatNumber = propertyAccount1.Number;
                model.WeChatMerchantNo = propertyAccount1.MerchantNo;
                model.WeChatKey = propertyAccount1.AccountKey;

                model.AlipayNumber = propertyAccount2.Number;
                model.AlipayMerchantNo = propertyAccount2.MerchantNo;
                model.AlipayKey = propertyAccount2.AccountKey;

                return View(model);
            }

            return View();
        }


        /// <summary>
        /// 设置微信账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "设置微信账号")]
        public ActionResult SetWeChatAccount()
        {
            PropertyAccountModel model = new PropertyAccountModel();
            var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");

            //获取微信账户
            var account = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //微信不为空
            if (account != null)
            {
                model.WeChatNumber = account.Number;
                model.WeChatMerchantNo = account.MerchantNo;
                model.WeChatKey = account.AccountKey;
                return View(model);
            }

            return View();
        }

        /// <summary>
        /// 设置微信账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public JsonResult SetWeChatAccount(PropertyAccountModel model)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
                IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");
                var account = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果微信账户不为空
                if (account != null)
                {
                    account.Number = model.WeChatNumber;
                    account.MerchantNo = model.WeChatMerchantNo;
                    account.AccountKey = model.WeChatKey;
                    //更新
                    propertyAccountBll.Update(account);
                }
                else
                {
                    T_PropertyAccount account2 = new T_PropertyAccount();
                    account2.PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
                    account2.AccountType = ConstantParam.PROPERTY_ACCOUNT_WeChat;
                    account2.Number = model.WeChatNumber;
                    account2.MerchantNo = model.WeChatMerchantNo;
                    account2.AccountKey = model.WeChatKey;
                    //保存
                    propertyAccountBll.Save(account2);
                }
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
        /// 设置支付宝账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "设置支付宝账号")]
        public ActionResult SetAlipayAccount()
        {
            PropertyAccountModel model = new PropertyAccountModel();
            var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");

            //获取支付宝账户
            var account = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //支付宝不为空
            if (account != null)
            {
                model.AlipayNumber = account.Number;
                model.AlipayMerchantNo = account.MerchantNo;
                return View(model);
            }

            return View();
        }

        /// <summary>
        /// 设置支付宝账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public JsonResult SetAlipayAccount(PropertyAccountModel model)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

                //存入文件的路径
                string directory = Server.MapPath(ConstantParam.ALIPAY_KEY);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //私钥
                HttpPostedFileBase privateFile = model.PrivatePath;

                //公钥
                HttpPostedFileBase publicFile = model.PublicPath;

                //获取上传文件名
                string privateFileName = Path.GetFileName(privateFile.FileName);
                string publicFileName = Path.GetFileName(publicFile.FileName);

                //获取上传文件的扩展名
                string privateFileEx = Path.GetExtension(privateFileName);
                string publicFileEx = Path.GetExtension(publicFileName);

                //定义上传文件的类型字符串
                string fileType = ".pem";

                //判断文件类型格式是否正确
                if (fileType != privateFileEx || fileType != publicFileEx)
                {
                    jm.Msg = "文件类型只能是pem格式的文件";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }

                //存入的文件名
                string PrivateName = Guid.NewGuid() + privateFileEx;
                string PublicName = Guid.NewGuid() + publicFileEx;

                //组装文件保存路径
                string savePrivatePath = Path.Combine(directory, PrivateName);
                string savePublicPath = Path.Combine(directory, PublicName);

                //保存数据文件
                privateFile.SaveAs(savePrivatePath);
                publicFile.SaveAs(savePublicPath);

                //读取私钥文件
                string content = PropertyUtils.ReadFile(savePrivatePath);
                if (content == "")
                {
                    jm.Msg = "私钥文件内容为空";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                string privatekey = content.Replace("\n", "").Replace("\r", "");
                string privateKey = privatekey.Substring(27);
                string PrivateKey = privateKey.Substring(0, privateKey.Length - 25);

                IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");
                var propertyAccount = propertyAccountBll.GetEntity(u => u.PropertyPlaceId == propertyPlaceId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果支付宝不为空
                if (propertyAccount != null)
                {
                    propertyAccount.Number = model.AlipayNumber;
                    propertyAccount.MerchantNo = model.AlipayMerchantNo;
                    propertyAccount.AccountKey = PrivateKey;
                    propertyAccount.PrivatePath = ConstantParam.ALIPAY_KEY + PrivateName;
                    propertyAccount.PublicPath = ConstantParam.ALIPAY_KEY + PublicName;
                    propertyAccountBll.Update(propertyAccount);
                }
                else
                {
                    T_PropertyAccount propertyaccount = new T_PropertyAccount()
                    {
                        PropertyPlaceId = propertyPlaceId,
                        AccountType = ConstantParam.PROPERTY_ACCOUNT_Alipay,
                        Number = model.AlipayNumber,
                        MerchantNo = model.AlipayMerchantNo,
                        AccountKey = PrivateKey,
                        PrivatePath = ConstantParam.ALIPAY_KEY + PrivateName,
                        PublicPath = ConstantParam.ALIPAY_KEY + PublicName
                    };
                    propertyAccountBll.Save(propertyaccount);
                }

                //日志记录
                //jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        ///// <summary>
        ///// 读取文件内容
        ///// </summary>
        ///// <param name="Path">文件路径</param>
        ///// <returns></returns>
        //public static string ReadFile(string Path)
        //{
        //    string s = "";
        //    StreamReader sr = new StreamReader(Path);
        //    s = sr.ReadToEnd();
        //    sr.Close();
        //    sr.Dispose();
        //    return s;
        //}
    }
}
