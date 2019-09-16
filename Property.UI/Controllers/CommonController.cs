using Com.Alipay;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Mobile;
using Senparc.Weixin.MP.TenPayLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace PSPlatform.UI.Controllers
{
    /// <summary>
    /// 系统共通的一些控制器
    /// </summary>
    public class CommonController : Controller
    {

        /// <summary>
        /// 跳转到404画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// 跳转到500画面
        /// </summary>
        /// <returns></returns>
        public ActionResult Error500()
        {
            return View();
        }

        /// <summary>
        /// 根据省份ID获取市列表
        /// </summary>
        /// <param name="provinceId">省ID</param>
        /// <returns>城市列表</returns>
        [HttpPost]
        public JsonResult GetCityList(int? provinceId)
        {
            List<object> list = new List<object>();
            if (provinceId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ICityBLL bll = BLLFactory<ICityBLL>.GetBLL("CityBLL");
            foreach (var item in bll.GetList(m => m.ProvinceId == provinceId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.CityName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 根据市获取区县列表
        /// </summary>
        /// <param name="cityId">城市ID</param>
        /// <returns>区县列表</returns>
        [HttpPost]
        public JsonResult GetCountyList(int? cityId)
        {
            List<object> list = new List<object>();
            if (cityId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ICountyBLL bll = BLLFactory<ICountyBLL>.GetBLL("CountyBLL");
            foreach (var item in bll.GetList(m => m.CityId == cityId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.CountyName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据楼盘ID获取单元列表
        /// </summary>
        /// <param name="buildId">楼盘ID</param>
        /// <returns>单元列表</returns>
        [HttpPost]
        public JsonResult GetUnitList(int? buildId)
        {
            List<object> list = new List<object>();
            if (buildId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            IBuildUnitBLL bll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            foreach (var item in bll.GetList(m => m.BuildId == buildId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.UnitName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据单元ID获取单元户列表
        /// </summary>
        /// <param name="buildId">单元ID</param>
        /// <returns>单元户列表</returns>
        [HttpPost]
        public JsonResult GetDoorList(int? unitId)
        {
            List<object> list = new List<object>();
            if (unitId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            IBuildDoorBLL bll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            foreach (var item in bll.GetList(m => m.UnitId == unitId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.DoorName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <param name="actionCode">0：注册验证  1：找回密码验证 2：身份验证</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetValidateCode(string phoneNum, int actionCode)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //如果是注册（设置手机号）操作获取验证码
                if (actionCode == 0)
                {
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    //如果手机号已存在
                    if (ownerBll.Exist(o => o.Phone == phoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        jm.Msg = APIMessage.PHONE_EXIST;
                        return Json(jm, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (actionCode == 1)
                {
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    //如果手机号对应用户不存在
                    if (!ownerBll.Exist(o => o.Phone == phoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        jm.Msg = APIMessage.PHONE_NO_EXIST;
                        return Json(jm, JsonRequestBehavior.AllowGet);
                    }
                }
                string code = PropertyUtils.CreateValidateCode(6);
                string msg = "感谢使用【Ai我家】微信公众号,验证码为:" + code + ",请在页面输入完成验证,如非本人操作请忽略";
                //如果短信发送成功
                if (PropertyUtils.SendSMS(phoneNum, msg, null))
                {
                    IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                    var phoneValidate = phoneValidateBll.GetEntity(v => v.PhoneNum == phoneNum && v.ActionCode == actionCode);
                    //如果该手机号在相同操作中不存在
                    if (phoneValidate == null)
                    {
                        T_PhoneValidate v = new T_PhoneValidate()
                        {
                            PhoneNum = phoneNum,
                            ValidateCode = code,
                            InvalidTime = DateTime.Now.AddMinutes(Convert.ToInt32(PropertyUtils.GetConfigParamValue("ValidateCodeInvalid"))),
                            ActionCode = actionCode
                        };
                        phoneValidateBll.Save(v);
                    }
                    else
                    {
                        phoneValidate.ValidateCode = code;
                        phoneValidate.InvalidTime = DateTime.Now.AddMinutes(Convert.ToInt32(PropertyUtils.GetConfigParamValue("ValidateCodeInvalid")));
                        phoneValidateBll.Update(phoneValidate);
                    }
                }
                else
                {
                    jm.Msg = APIMessage.VALDATE_GET_FAIL;
                }
            }
            catch
            {
                jm.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 登录时链接的使用许可协议页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CertificateWebView()
        {
            return View();
        }

        /// <summary>
        /// 业主重置密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserResetPassword(PassResetActiveModel model)
        {
            if (model.Body == null)
            {
                return RedirectToAction("ResetUrlInvalid");
            }
            //将用户ID解密
            int UserId = Int32.Parse(PropertyUtils.DecodeBase64(model.Body));
            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            var user = userBll.GetEntity(u => u.Id == UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (user != null)
            {
                //如果重置密码激活码存在且未失效
                if (!string.IsNullOrEmpty(user.Activecode) && model.Activecode == user.Activecode
                    && user.ActivecodeInvalidTime != null && DateTime.Now <= user.ActivecodeInvalidTime.Value)
                {
                    UserPassResetModel m = new UserPassResetModel();
                    m.UserId = user.Id;
                    m.UserName = user.UserName;
                    m.Activecode = user.Activecode;
                    return View(m);
                }
                else
                {
                    return RedirectToAction("ResetUrlInvalid");
                }
            }
            else
            {
                return RedirectToAction("ResetUrlInvalid");
            }
        }

        /// <summary>
        /// 重置密码提交
        /// </summary>
        /// <param name="model">密码重置模型</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserResetPassword(UserPassResetModel model)
        {
            //判断提交模型数据是否正确
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            var user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (user != null)
            {
                //如果重置密码激活码存在且未失效
                if (!string.IsNullOrEmpty(user.Activecode) && model.Activecode == user.Activecode
                    && user.ActivecodeInvalidTime != null && DateTime.Now < user.ActivecodeInvalidTime.Value)
                {
                    user.Password = PropertyUtils.GetMD5Str(model.Password);
                    //密码重置链接失效
                    user.Activecode = "";
                    user.ActivecodeInvalidTime = null;
                    //如果修改成功
                    if (userBll.Update(user))
                    {

                        return RedirectToAction("ResetSuccess");
                    }
                }
            }
            return RedirectToAction("Error500");
        }


        /// <summary>
        /// 重置成功界面
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetSuccess()
        {
            return View();
        }

        /// <summary>
        /// 重置链接失效界面
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetUrlInvalid()
        {
            return View();
        }

        /// <summary>
        /// 微信支付异步通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WeixinPayNotifyUrl()
        {
            Stream st = Request.InputStream;
            StreamReader sr = new StreamReader(st);
            string SRstring = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SRstring);
            sr.Close();

            string return_code = doc.GetElementsByTagName("return_code")[0].InnerText;
            //如果返回成功
            if (return_code == "SUCCESS")
            {
                string result_code = doc.GetElementsByTagName("result_code")[0].InnerText;
                if (result_code == "SUCCESS")
                {
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    string orderNo = doc.GetElementsByTagName("out_trade_no")[0].InnerText;
                    var order = orderBll.GetEntity(o => o.PayTradeNo == orderNo);
                    if (order != null && order.OrderStatus == ConstantParam.OrderStatus_NOPAY)
                    {
                        //获取商家账户信息
                        var wxAccount = order.Shop.ShopAccounts.Where(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                        if (wxAccount != null)
                        {
                            //获取商家账户信息
                            string WeixinPayKey = wxAccount.AccountKey;
                            //组装签名字符串
                            StringBuilder signStr = new StringBuilder();
                            signStr.Append("appid=" + doc.GetElementsByTagName("appid")[0].InnerText + "&");
                            signStr.Append("bank_type=" + doc.GetElementsByTagName("bank_type")[0].InnerText + "&");
                            signStr.Append("cash_fee=" + doc.GetElementsByTagName("cash_fee")[0].InnerText + "&");
                            signStr.Append("fee_type=" + doc.GetElementsByTagName("fee_type")[0].InnerText + "&");
                            signStr.Append("is_subscribe=" + doc.GetElementsByTagName("is_subscribe")[0].InnerText + "&");
                            signStr.Append("mch_id=" + doc.GetElementsByTagName("mch_id")[0].InnerText + "&");
                            signStr.Append("nonce_str=" + doc.GetElementsByTagName("nonce_str")[0].InnerText + "&");
                            signStr.Append("openid=" + doc.GetElementsByTagName("openid")[0].InnerText + "&");
                            signStr.Append("out_trade_no=" + orderNo + "&");
                            signStr.Append("result_code=" + result_code + "&");
                            signStr.Append("return_code=" + return_code + "&");
                            signStr.Append("time_end=" + doc.GetElementsByTagName("time_end")[0].InnerText + "&");
                            signStr.Append("total_fee=" + doc.GetElementsByTagName("total_fee")[0].InnerText + "&");
                            signStr.Append("trade_type=" + doc.GetElementsByTagName("trade_type")[0].InnerText + "&");
                            signStr.Append("transaction_id=" + doc.GetElementsByTagName("transaction_id")[0].InnerText + "&");
                            signStr.Append("key=" + WeixinPayKey);
                            string sign = PropertyUtils.GetMD5Str(signStr.ToString()).ToUpper();
                            //签名验证成功
                            if (doc.GetElementsByTagName("sign")[0].InnerText == sign)
                            {
                                //更新订单状态
                                order.OrderStatus = ConstantParam.OrderStatus_RECEIPT;
                                order.PayWay = 1;
                                order.PayDate = DateTime.Now;
                                if (orderBll.Update(order)) 
                                {
                                    IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                                    var ShopUserId = shopBLL.GetEntity(s => s.Id == order.ShopId).ShopUserId;

                                    //推送给订单所属商家
                                    IShopUserPushBLL userPushBLL = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                                    var userPush = userPushBLL.GetEntity(p => p.UserId == ShopUserId);
                                    if (userPush != null)
                                    {
                                        string registrationId = userPush.RegistrationId;
                                        string alert = "订单号为" + order.OrderNo + "的订单已支付，点击查看详情";
                                        //通知信息
                                        PropertyUtils.SendPush("订单支付通知", alert, ConstantParam.MOBILE_TYPE_SHOP, registrationId);
                                    }
                                }
                            }
                        }
                    }
                    return Content("success");
                }
            }
            return Content("fail");
        }

        /// <summary>
        /// 物业缴费微信异步通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WeixinExpenseNotifyUrl()
        {
            Stream st = Request.InputStream;
            StreamReader sr = new StreamReader(st);
            string SRstring = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SRstring);
            sr.Close();

            string return_code = doc.GetElementsByTagName("return_code")[0].InnerText;
            //如果返回成功
            if (return_code == "SUCCESS")
            {
                string result_code = doc.GetElementsByTagName("result_code")[0].InnerText;
                if (result_code == "SUCCESS")
                {
                    string expenseOrderNo = doc.GetElementsByTagName("out_trade_no")[0].InnerText;

                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var record = expenseDetailsBLL.GetEntity(e => e.PayTradeNo == expenseOrderNo);

                    if (record != null && record.IsPayed == ConstantParam.PAYED_FALSE)
                    {
                        //获取物业微信账户信息
                        T_PropertyAccount wxAccount = null;
                        if (record.BuildDoorId != null)
                        {
                            wxAccount = record.BuildDoor.BuildUnit.Build.PropertyPlace.PropertyAccounts.Where(
                            a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                        }
                        else
                        {
                            wxAccount = record.BuildCompany.PropertyPlace.PropertyAccounts.Where(
                            a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                        }
                        if (wxAccount != null)
                        {
                            string WeixinPayKey = wxAccount.AccountKey;
                            //组装签名字符串
                            StringBuilder signStr = new StringBuilder();
                            signStr.Append("appid=" + doc.GetElementsByTagName("appid")[0].InnerText + "&");
                            signStr.Append("bank_type=" + doc.GetElementsByTagName("bank_type")[0].InnerText + "&");
                            signStr.Append("cash_fee=" + doc.GetElementsByTagName("cash_fee")[0].InnerText + "&");
                            signStr.Append("fee_type=" + doc.GetElementsByTagName("fee_type")[0].InnerText + "&");
                            signStr.Append("is_subscribe=" + doc.GetElementsByTagName("is_subscribe")[0].InnerText + "&");
                            signStr.Append("mch_id=" + doc.GetElementsByTagName("mch_id")[0].InnerText + "&");
                            signStr.Append("nonce_str=" + doc.GetElementsByTagName("nonce_str")[0].InnerText + "&");
                            signStr.Append("openid=" + doc.GetElementsByTagName("openid")[0].InnerText + "&");
                            signStr.Append("out_trade_no=" + expenseOrderNo + "&");
                            signStr.Append("result_code=" + result_code + "&");
                            signStr.Append("return_code=" + return_code + "&");
                            signStr.Append("time_end=" + doc.GetElementsByTagName("time_end")[0].InnerText + "&");
                            signStr.Append("total_fee=" + doc.GetElementsByTagName("total_fee")[0].InnerText + "&");
                            signStr.Append("trade_type=" + doc.GetElementsByTagName("trade_type")[0].InnerText + "&");
                            signStr.Append("transaction_id=" + doc.GetElementsByTagName("transaction_id")[0].InnerText + "&");
                            signStr.Append("key=" + WeixinPayKey);
                            string sign = PropertyUtils.GetMD5Str(signStr.ToString()).ToUpper();
                            //签名验证成功
                            if (doc.GetElementsByTagName("sign")[0].InnerText == sign)
                            {
                                record.IsPayed = ConstantParam.PAYED_TRUE;
                                record.PaymentType = 2;
                                record.PayedDate = DateTime.Now;
                                expenseDetailsBLL.Update(record);
                            }
                        }
                    }
                    return Content("success");
                }
            }
            return Content("fail");
        }

        /// <summary>
        /// 支付异步通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AlipayPayNotifyUrl()
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            coll = Request.Form;
            String[] requestItem = coll.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            //判断是否有带返回参数
            if (sArray.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sArray, Request.Form["notify_id"], Request.Form["sign"]);
                verifyResult = true;
                //如果验证成功 
                if (verifyResult)
                {
                    //如果支付成功
                    if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                        string orderNo = Request.Form["out_trade_no"];
                        var order = orderBll.GetEntity(o => o.OrderNo == orderNo);
                        if (order != null)
                        {
                            order.OrderStatus = ConstantParam.OrderStatus_RECEIPT;
                            order.PayTradeNo = Request.Form["trade_no"];
                            orderBll.Update(order);
                        }
                    }
                    return Content("success");
                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }


        /// <summary>
        /// 退款异步通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AlipayRefundNotifyUrl()
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            coll = Request.Form;
            String[] requestItem = coll.AllKeys;

            for (int i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            //判断是否有带返回参数
            if (sArray.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sArray, Request.Form["notify_id"], Request.Form["sign"]);
                verifyResult = true;
                if (verifyResult)//验证成功
                {
                    string TradeNo = Request.Form["result_details"].Split('^')[0];
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.PayTradeNo == TradeNo);
                    if (order != null)
                    {
                        order.OrderStatus = ConstantParam.OrderStatus_EXIT;
                        order.RecedeType = 2;
                        order.RecedeTime = DateTime.Now;

                        if (Request.Form["result_details"].Split('^')[2] == "SUCCESS")
                        {
                            order.RefundResult = "退款成功";
                        }
                        else
                        {
                            order.RefundResult = "退款失败";
                        }
                        string alert = "您在" + order.Shop.ShopName + "提交的订单已被商家退单";

                        //如果修改成功
                        if (orderBll.Update(order))
                        {
                            //推送给订单所属用户
                            IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                            var userPush = userPushBLL.GetEntity(p => p.UserId == order.AppUserId);
                            if (userPush != null)
                            {
                                string registrationId = userPush.RegistrationId;

                                //通知信息
                                PropertyUtils.SendPush("订单最新状态", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                            }
                        }
                    }
                    return Content("success");
                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }
    }
}
