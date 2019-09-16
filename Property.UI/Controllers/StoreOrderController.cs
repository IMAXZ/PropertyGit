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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Webdiyer.WebControls.Mvc;
using Alipay = Com.Alipay;
namespace Property.UI.Controllers
{
    /// <summary>
    /// 商家订单管理控制器
    /// </summary>
    public class StoreOrderController : ShopBaseController
    {
        /// <summary>
        /// 商家订单管理列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "商家订单列表")]
        [HttpGet]
        public ActionResult OrderList(StoreOrderSearchModel model)
        {
            //获取登录用户的门店
            int userId = GetSessionModel().UserID;
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shop = shopBll.GetEntity(u => u.ShopUserId == userId);

            //如果该门店存在
            if (shop != null)
            {
                //获取订单状态下拉列表
                model.OrderStatusList = GetOrderStatusList(shop.Type.Contains(ConstantParam.SHOP_TYPE_0.ToString()));

                //获取支付方式下拉列表
                model.PayWayList = GetPayWayList(shop.Type.Contains(ConstantParam.SHOP_TYPE_0.ToString()));

                //初始化默认查询模型
                DateTime today = DateTime.Today;
                if (model.StartDate == null)
                    model.StartDate = today.AddDays(-today.Day + 1);
                if (model.EndDate == null)
                    model.EndDate = today;

                //获取当前门店ID
                var shopId = GetCurrentShopId().Value;

                //根据订单日期查询
                DateTime endDate = model.EndDate.Value.AddDays(1);
                Expression<Func<T_Order, bool>> where = u => u.OrderDate >= model.StartDate.Value && u.OrderDate < endDate && u.ShopId == shopId 
                    && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT;

                //根据订单状态查询
                if (model.OrderStatus != null)
                {
                    where = PredicateBuilder.And(where, u => u.OrderStatus == model.OrderStatus);
                }

                //根据支付方式查询
                if (model.PayWay != null)
                {
                    where = PredicateBuilder.And(where, u => u.PayWay == model.PayWay);
                }

                //根据查询条件调用BLL层 获取分页数据
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var sortName = this.SettingSorting("Id", false);
                model.DataList = orderBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<T_Order>;
                return View(model);
            }

            //否则返回首页
            return RedirectToAction("Index", "ShopPlatform");
        }

        /// <summary>
        /// 订单详细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "订单详细")]
        [HttpGet]
        public ActionResult OrderDetail(int id)
        {
            IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");

            //获取要查看的订单
            T_Order order = orderBll.GetEntity(u => u.Id == id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);
            if (order != null)
            {
                return View(order);
            }
            else
            {
                return RedirectToAction("OrderList");
            }
        }

        /// <summary>
        /// 退单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RecedeOrder(int id)
        {
            IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
            T_Order order = orderBll.GetEntity(o => o.Id == id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);
            if (order != null)
            {
                RecedeReasonModel model = new RecedeReasonModel()
                {
                    OrderId = order.Id,
                    OrderNo = order.OrderNo
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("OrderList");
            }
        }

        /// <summary>
        /// 退单 提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RecedeOrder(RecedeReasonModel model)
        {
            if (ModelState.IsValid)
            {
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                T_Order order = orderBll.GetEntity(o => o.Id == model.OrderId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);

                if (order != null)
                {
                    string alert = "";
                    //如果订单状态为待收货
                    if (order.OrderStatus == ConstantParam.OrderStatus_RECEIPT)
                    {
                        //如果订单支付方式为微信在线支付
                        if (order.PayWay == 1)
                        {
                            //微信退款
                            //获取商家信息
                            var wxAccount = order.Shop.ShopAccounts.Where(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                            if (wxAccount != null)
                            {
                                //获取商家账户信息
                                string WeixinAppId = wxAccount.Number;
                                string WeixinMchId = wxAccount.MerchantNo;
                                string WeixinPayKey = wxAccount.AccountKey;

                                //申请退款
                                string result = ApplyRefund(order, WeixinAppId, WeixinMchId, WeixinPayKey);
                                //如果请求失败
                                if (result == null)
                                {
                                    ModelState.AddModelError("OrderId", "订单退款申请失败");
                                    return View(model);
                                }
                                //解析返回数据
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(result);
                                string return_code = doc.GetElementsByTagName("return_code")[0].InnerText;

                                //如果返回成功
                                if (return_code == "SUCCESS")
                                {
                                    string result_code = doc.GetElementsByTagName("result_code")[0].InnerText;
                                    if (result_code == "SUCCESS")
                                    {
                                        order.RecedeType = 2;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("OrderId", "订单退款申请失败");
                                        return View(model);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("OrderId", "订单退款申请失败");
                                    return View(model);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("OrderId", "该订单所属商家未设置账户信息");
                                return View(model);
                            }
                        }
                        //支付宝支付退款
                        else if (order.PayWay == 2)
                        {
                            order.Reason = model.Reason;
                            orderBll.Update(order);

                            var r = new Random();
                            string batch_no = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(1000);
                            string detail_data = order.PayTradeNo + "^" + order.OrderPrice + "^" + model.Reason;

                            //把请求参数打包成数组
                            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                            sParaTemp.Add("service", Alipay.Config.service);
                            sParaTemp.Add("partner", Alipay.Config.partner);
                            sParaTemp.Add("_input_charset", Alipay.Config.input_charset.ToLower());
                            sParaTemp.Add("notify_url", PropertyUtils.GetConfigParamValue("HostUrl") + "/Common/AlipayRefundNotifyUrl");
                            sParaTemp.Add("seller_user_id", Alipay.Config.seller_user_id);
                            sParaTemp.Add("refund_date", Alipay.Config.refund_date);
                            sParaTemp.Add("batch_no", batch_no);
                            sParaTemp.Add("batch_num", "1");
                            sParaTemp.Add("detail_data", detail_data);

                            //建立请求
                            string html = Alipay.Submit.BuildRequest(sParaTemp, "get", "确定");
                            return Content(html);
                        }
                        else
                        {
                            order.RecedeType = 0;
                        }
                    }
                    //订单状态为待确认
                    else if (order.OrderStatus == ConstantParam.OrderStatus_CONFIRM)
                    {
                        order.RecedeType = 0;
                    }

                    order.OrderStatus = ConstantParam.OrderStatus_EXIT;
                    order.Reason = model.Reason;
                    order.RecedeTime = DateTime.Now;
                    alert = "您在" + order.Shop.ShopName + "提交的订单已被商家退单";

                    //如果退单成功
                    if (orderBll.CancelOrder(order))
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
                    return RedirectToAction("OrderList");
                }
                else
                {
                    return RedirectToAction("OrderList");
                }
            }
            else
            {
                ModelState.AddModelError("OrderId", ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR);
                return View(model);
            }
        }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateOrderStatus(OrderStatusUpdateModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //获取指定订单
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var order = orderBll.GetEntity(o => o.Id == model.OrderId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);
                if (order != null)
                {
                    string alert = "";

                    if (model.OrderStatus == ConstantParam.OrderStatus_RECEIPT)
                    {
                        alert = "您在" + order.Shop.ShopName + "提交的订单商家已接单，请您耐心等待收货";
                        order.OrderStatus = model.OrderStatus;
                    }
                    else if (model.OrderStatus == ConstantParam.OrderStatus_FINISH)
                    {
                        alert = "您在" + order.Shop.ShopName + "提交的订单交易已完成";
                        order.OrderStatus = ConstantParam.OrderStatus_FINISH;
                        order.CompleteTime = DateTime.Now;
                    }
                    //如果订单修改成功
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
                else
                {
                    jm.Msg = "该订单已不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Post请求 订单申请退款
        /// </summary>
        /// <returns></returns>
        private string ApplyRefund(T_Order order, string WeixinAppId, string WeixinMchId, string WeixinPayKey)
        {
            Random r = new Random();

            #region 组装参数
            //组装签名字符串
            StringBuilder signStr = new StringBuilder();
            //组装xml格式
            StringBuilder varBody = new StringBuilder();

            varBody.Append("<xml>");
            //APP应用ID
            varBody.Append("<appid>" + WeixinAppId + "</appid>");
            signStr.Append("appid=" + WeixinAppId + "&");
            //商户号
            varBody.Append("<mch_id>" + WeixinMchId + "</mch_id>");
            signStr.Append("mch_id=" + WeixinMchId + "&");
            //随机字符串
            string str = "1234567890abcdefghijklmnopqrstuvwxyz";
            string randomStr = "";
            for (int i = 0; i < 32; i++)
            {
                randomStr = randomStr + str[r.Next(str.Length)].ToString();
            }
            varBody.Append("<nonce_str>" + randomStr + "</nonce_str>");
            signStr.Append("nonce_str=" + randomStr + "&");
            //操作员
            varBody.Append("<op_user_id>" + WeixinMchId + "</op_user_id>");
            signStr.Append("op_user_id=" + WeixinMchId + "&");
            //商户退款单号
            string refundNo = DateTime.Now.ToFileTime().ToString() + new Random().Next(1000);
            varBody.Append("<out_refund_no>" + refundNo + "</out_refund_no>");
            signStr.Append("out_refund_no=" + refundNo + "&");
            //商户订单号
            varBody.Append("<out_trade_no>" + order.PayTradeNo + "</out_trade_no>");
            signStr.Append("out_trade_no=" + order.PayTradeNo + "&");

            int fee = Convert.ToInt32(order.OrderPrice * 100);
            //退款金额
            varBody.Append("<refund_fee>" + fee + "</refund_fee>");
            signStr.Append("refund_fee=" + fee + "&");
            //总金额
            varBody.Append("<total_fee>" + fee + "</total_fee>");
            signStr.Append("total_fee=" + fee + "&");
            //签名
            signStr.Append("key=" + WeixinPayKey);
            varBody.Append("<sign>" + PropertyUtils.GetMD5Str(signStr.ToString()).ToUpper() + "</sign>");
            varBody.Append("</xml>");
            #endregion
            //发送HTTP POST请求
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";

            string cert = Server.MapPath("~/App_Data/apiclient_cert.p12");
            string password = WeixinMchId;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            X509Certificate cer = new X509Certificate(cert, password);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ClientCertificates.Add(cer);

            byte[] bytes = Encoding.UTF8.GetBytes(varBody.ToString());
            request.ContentLength = bytes.Length;
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(bytes, 0, bytes.Length);
                writer.Flush();
                writer.Close();
            }
            //处理返回结果
            string result = null;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    reader.Close();
                }
            }
            PubFunction.ErrorLogPrint("result", result);
            return result;
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>
        /// 订单状态列表
        /// </summary>
        /// <param name="isGreenType">是否绿色直供</param>
        /// <returns></returns>
        private List<SelectListItem> GetOrderStatusList(bool isGreenType)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "待付款",
                Value = ConstantParam.OrderStatus_NOPAY.ToString(),
                Selected = false
            });
            if (!isGreenType)
            {
                list.Add(new SelectListItem()
                {
                    Text = "待确认",
                    Value = ConstantParam.OrderStatus_CONFIRM.ToString(),
                    Selected = false
                });
            }
            list.Add(new SelectListItem()
            {
                Text = "待收货",
                Value = ConstantParam.OrderStatus_RECEIPT.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已退单",
                Value = ConstantParam.OrderStatus_EXIT.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "交易完成",
                Value = ConstantParam.OrderStatus_FINISH.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "交易关闭",
                Value = ConstantParam.OrderStatus_CLOSE.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 支付方式列表
        /// </summary>
        /// <param name="isGreenType">是否绿色直供</param>
        /// <returns></returns>
        private List<SelectListItem> GetPayWayList(bool isGreenType)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "未支付",
                Value = ConstantParam.DEFAULT_NO_PAY.ToString(),
                Selected = false
            });
            if (isGreenType)
            {
                list.Add(new SelectListItem()
                {
                    Text = "微信在线支付",
                    Value = ConstantParam.WeChat_ONLINE_PAY.ToString(),
                    Selected = false
                });
                list.Add(new SelectListItem()
                {
                    Text = "支付宝在线支付",
                    Value = ConstantParam.AliPay_ONLINE_PAY.ToString(),
                    Selected = false
                });
            }
            list.Add(new SelectListItem()
            {
                Text = "货到现金支付",
                Value = ConstantParam.DELIVER_CASH_PAY.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "货到微信付款",
                Value = ConstantParam.DELIVER_WeChat_PAY.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "货到支付宝付款",
                Value = ConstantParam.DELIVER_AliPay_PAY.ToString(),
                Selected = false
            });
            return list;
        }
    }
}
