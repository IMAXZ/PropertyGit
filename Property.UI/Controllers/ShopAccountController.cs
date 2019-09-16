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
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 门店账户设定控制器
    /// </summary>
    public class ShopAccountController : ShopBaseController
    {

        /// <summary>
        /// 设置门店账户表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label="门店账户设定")]
        public ActionResult AccountList()
        {
            //获取登录用户的门店
            int userId = GetSessionModel().UserID;
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shop = shopBll.GetEntity(u => u.ShopUserId == userId);

            //如果该门店存在
            if (shop != null)
            {
                ShopAccountModel model = new ShopAccountModel();
                var shopId = GetCurrentShopId().Value;

                IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");

                //微信账户
                var shopAccount1 = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //支付宝账户
                var shopAccount2 = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //微信不为空，支付宝为空
                if (shopAccount1 != null && shopAccount2 == null)
                {
                    model.WeChatNumber = shopAccount1.Number;
                    model.WeChatMerchantNo = shopAccount1.MerchantNo;
                    model.WeChatKey = shopAccount1.AccountKey;

                    return View(model);
                }

                //支付宝不为空，微信为空
                if (shopAccount2 != null && shopAccount1 == null)
                {
                    model.AlipayNumber = shopAccount2.Number;
                    model.AlipayMerchantNo = shopAccount2.MerchantNo;
                    model.AlipayKey = shopAccount2.AccountKey;

                    return View(model);
                }

                //微信，支付宝都不为空
                if (shopAccount1 != null && shopAccount2 != null)
                {
                    model.WeChatNumber = shopAccount1.Number;
                    model.WeChatMerchantNo = shopAccount1.MerchantNo;
                    model.WeChatKey = shopAccount1.AccountKey;

                    model.AlipayNumber = shopAccount2.Number;
                    model.AlipayMerchantNo = shopAccount2.MerchantNo;
                    model.AlipayKey = shopAccount2.AccountKey;

                    return View(model);
                }

                return View();
            }

            //否则返回首页
            return RedirectToAction("Index", "ShopPlatform");
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
            ShopAccountModel model = new ShopAccountModel();
            var shopId = GetCurrentShopId().Value;

            IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");

            //获取微信账户
            var account = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
        public JsonResult SetWeChatAccount(ShopAccountModel model)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                var shopId = GetCurrentShopId().Value;
                IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");
                var account = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果微信账户不为空
                if (account != null)
                {
                    account.Number = model.WeChatNumber;
                    account.MerchantNo = model.WeChatMerchantNo;
                    account.AccountKey = model.WeChatKey;
                    //更新
                    shopAccountBll.Update(account);
                }
                else
                {
                    T_ShopAccounts account2 = new T_ShopAccounts();
                    account2.ShopId = shopId;
                    account2.CreateDate = DateTime.Now;
                    account2.AccountType = ConstantParam.PROPERTY_ACCOUNT_WeChat;
                    account2.Number = model.WeChatNumber;
                    account2.MerchantNo = model.WeChatMerchantNo;
                    account2.AccountKey = model.WeChatKey;
                    //保存
                    shopAccountBll.Save(account2);
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
        /// 设置支付账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "设置支付宝账号")]
        public ActionResult SetAlipayAccount()
        {
            ShopAccountModel model = new ShopAccountModel();
            var shopId = GetCurrentShopId().Value;

            IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");

            //获取支付宝账户
            var account = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //支付宝不为空
            if (account != null)
            {
                model.AlipayNumber = account.Number;
                model.AlipayMerchantNo = account.MerchantNo;
                model.AlipayKey = account.AccountKey;
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
        public JsonResult SetAlipayAccount(ShopAccountModel model)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                var shopId = GetCurrentShopId().Value;

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
                if (privateFileEx != fileType || publicFileEx != fileType)
                {
                    jm.Msg = "文件类型只能是pem格式的文件";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }

                //存入文件名
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

                IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");
                var shopAccount = shopAccountBll.GetEntity(u => u.ShopId == shopId && u.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果支付宝不为空
                if (shopAccount != null)
                {
                    shopAccount.Number = model.AlipayNumber;
                    shopAccount.MerchantNo = model.AlipayMerchantNo;
                    shopAccount.AccountKey = PrivateKey;
                    shopAccount.PrivatePath = ConstantParam.ALIPAY_KEY + PrivateName;
                    shopAccount.PublicPath = ConstantParam.ALIPAY_KEY + PublicName;
                    shopAccountBll.Update(shopAccount);
                }
                else
                {
                    T_ShopAccounts shopaccount = new T_ShopAccounts()
                    {
                        ShopId = shopId,
                        AccountType = ConstantParam.PROPERTY_ACCOUNT_Alipay,
                        CreateDate = DateTime.Now,
                        Number = model.AlipayNumber,
                        MerchantNo = model.AlipayMerchantNo,
                        AccountKey = PrivateKey,
                        PrivatePath = ConstantParam.ALIPAY_KEY + PrivateName,
                        PublicPath = ConstantParam.ALIPAY_KEY + PublicName
                    };
                    shopAccountBll.Save(shopaccount);
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

    }
}
