using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 商家订单接口控制器
    /// </summary>
    public class StoreOrderController : ApiController
    {
        /// <summary>
        /// 获取订单分页列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel OrderList([FromUri]OrderPagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                //获取当前商家用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                var user = shopUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    shopUserBll.Update(user);
                    //如果该用户还未创建门店
                    if (user.Shops.Count < 1)
                    {
                        resultModel.Msg = APIMessage.SHOP_NOEXIST;
                        return resultModel;
                    }
                    else
                    {
                        //获取订单列表数据
                        IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                        int shopId = user.Shops.FirstOrDefault().Id;
                        Expression<Func<T_Order, bool>> where = o => o.ShopId == shopId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT;
                        if (model.OrderStatus != null)
                        {
                            where = PredicateBuilder.And(where, o => o.OrderStatus == model.OrderStatus);
                        }
                        resultModel.result = orderBll.GetPageList(where, "OrderDate", false, model.PageIndex).Select(o => new
                        {
                            Id = o.Id,
                            OrderNo = o.OrderNo,
                            ShopId = o.Shop.Id,
                            ShopName = o.Shop.ShopName,
                            ShopImg = string.IsNullOrEmpty(o.Shop.ImgThumbnail) ? "" : o.Shop.ImgThumbnail.Split(';')[0],
                            BuyUserName = o.User.UserName,
                            BuyUserHeadPic = o.User.HeadPath,
                            BuyUserPhone = o.ShippingAddress.Telephone,
                            OrderTime = o.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            OrderStatus = o.OrderStatus,
                            RecedeType = o.RecedeType,
                            RemainingTime = o.PayDate == null || o.PayDate.Value.AddHours(2) < DateTime.Now ? "0小时0分" : (o.PayDate.Value.AddHours(2) - DateTime.Now).Hours + "小时" + (o.PayDate.Value.AddHours(2) - DateTime.Now).Minutes + "分",
                            ExitOrderReason = o.Reason,
                            OrderPrice = o.OrderPrice,
                            SendAddress = o.ShippingAddress.County.City.Province.ProvinceName + o.ShippingAddress.County.City.CityName + o.ShippingAddress.County.CountyName + "  " + o.ShippingAddress.AddressDetails,
                            PayWay = o.PayWay,
                            RefundStatus = GetRefundResult(o.Id),
                            PayTradeNo = o.PayTradeNo,
                            RecedeTime = o.RecedeTime != null ? o.RecedeTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            Memo = string.IsNullOrEmpty(o.Memo) ? "" : o.Memo,
                        });
                        resultModel.Total = orderBll.Count(where);
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }


        /// <summary>
        /// 获取退款结果数据
        /// </summary>
        /// <param name="OrderId">订单ID</param>
        /// <returns></returns>
        private string GetRefundResult(int OrderId)
        {
            string RefundResult = "";
            //获取订单数据
            IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
            var order = orderBll.GetEntity(o => o.Id == OrderId);

            //如果已经生成了微信支付订单号且退单类型为退单退款
            if (!string.IsNullOrEmpty(order.PayTradeNo) && order.RecedeType == 2)
            {
                //如果是微信在线支付
                if (order.PayWay == 1)
                {
                    //获取商家账户信息
                    var wxAccount = order.Shop.ShopAccounts.Where(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                    if (wxAccount != null)
                    {
                        //获取商家账户信息
                        string WeixinAppId = wxAccount.Number;
                        string WeixinMchId = wxAccount.MerchantNo;
                        string WeixinPayKey = wxAccount.AccountKey;

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

                        //支付订单号
                        varBody.Append("<out_trade_no>" + order.PayTradeNo + "</out_trade_no>");
                        signStr.Append("out_trade_no=" + order.PayTradeNo + "&");
                        //签名
                        signStr.Append("key=" + WeixinPayKey);
                        varBody.Append("<sign>" + PropertyUtils.GetMD5Str(signStr.ToString()).ToUpper() + "</sign>");
                        varBody.Append("</xml>");
                        #endregion

                        //发送HTTP POST请求
                        string url = "https://api.mch.weixin.qq.com/pay/refundquery";

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "POST";
                        request.ContentType = "text/xml";
                        // 信任证书
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

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
                                String RefundStatus = doc.GetElementsByTagName("refund_status_0")[0].InnerText;
                                switch (RefundStatus)
                                {
                                    case "SUCCESS":
                                        RefundResult = "退款成功";
                                        break;
                                    case "FAIL":
                                        RefundResult = "退款失败";
                                        break;
                                    case "PROCESSING":
                                        RefundResult = "退款处理中";
                                        break;
                                    default:
                                        RefundResult = "退款失败";
                                        break;
                                }
                            }
                        }
                    }
                }
                //如果是支付宝在线支付
                else if (order.PayWay == 2)
                {
                    RefundResult = order.RefundResult;
                }
            }
            return RefundResult;
        }

        /// <summary>
        /// 订单详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel OrderDetail([FromUri]DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前商家用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                var user = shopUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user == null)
                {
                    resultModel.Msg = APIMessage.NO_USER;
                    return resultModel;
                }
                //如果验证Token不通过或已过期
                if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                {
                    resultModel.Msg = APIMessage.TOKEN_INVALID;
                    return resultModel;
                }
                //更新最近登录时间和Token失效时间
                user.LatelyLoginTime = DateTime.Now;
                user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                shopUserBll.Update(user);

                //获取指定订单
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);
                if (order != null)
                {
                    resultModel.result = new
                    {
                        Id = order.Id,
                        OrderNo = order.OrderNo,
                        ShopName = order.Shop.ShopName,
                        ShopPhone = string.IsNullOrEmpty(order.Shop.Phone) ? "" : order.Shop.Phone,
                        OrderTime = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        OrderStatus = order.OrderStatus,
                        Shipper = order.ShippingAddress.ShipperName + "(" + order.ShippingAddress.Telephone + ")",
                        SendAddress = order.ShippingAddress.County.City.Province.ProvinceName + order.ShippingAddress.County.City.CityName + order.ShippingAddress.County.CountyName + "  " + order.ShippingAddress.AddressDetails,
                        PayWay = order.PayWay,
                        BuyUserName = order.User.UserName,
                        BuyUserHeadPic = order.User.HeadPath,
                        BuyUserPhone = order.User.Phone,
                        Memo = order.Memo,
                        RecedeType = order.RecedeType,
                        RemainingTime = order.PayDate == null || order.PayDate.Value.AddHours(2) < DateTime.Now ? "0小时0分" : (order.PayDate.Value.AddHours(2) - DateTime.Now).Hours + "小时" + (order.PayDate.Value.AddHours(2) - DateTime.Now).Minutes + "分",
                        ExitOrderReason = order.Reason,
                        RefundStatus = GetRefundResult(order.Id),
                        RecedeTime = order.RecedeTime != null ? order.RecedeTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        TotalPrice = Convert.ToDouble(Convert.ToDecimal(order.OrderDetails.Select(d => d.Price).ToArray().Sum())),
                        Freight = Convert.ToDouble(Convert.ToDecimal(order.OrderPrice) - Convert.ToDecimal(order.OrderDetails.Select(d => d.Price).ToArray().Sum())),
                        GoodsList = order.OrderDetails.Select(d => new
                        {
                            SaleName = d.ShopSale.Title,
                            SaleImg = string.IsNullOrEmpty(d.ShopSale.ImgThumbnail) ? "" : d.ShopSale.ImgThumbnail.Split(';')[0],
                            SaledAmount = d.SaledAmount,
                            Price = Convert.ToDouble(Convert.ToDecimal(d.Price / d.SaledAmount))
                        })
                    };
                }
                else
                {
                    resultModel.Msg = "订单已不存在";
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel UpdateOrderStatus(OrderUpdateStatusModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前商家用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                var user = shopUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user == null)
                {
                    resultModel.Msg = APIMessage.NO_USER;
                    return resultModel;
                }
                //如果验证Token不通过或已过期
                if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                {
                    resultModel.Msg = APIMessage.TOKEN_INVALID;
                    return resultModel;
                }
                //更新最近登录时间和Token失效时间
                user.LatelyLoginTime = DateTime.Now;
                user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                shopUserBll.Update(user);

                //获取指定订单
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var order = orderBll.GetEntity(o => o.Id == model.OrderId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);
                if (order != null)
                {
                    string alert = "";
                    //如果已退单
                    if (model.OrderStatus == ConstantParam.OrderStatus_EXIT)
                    {
                        order.Reason = model.Reason;
                        //如果订单状态为待收货
                        if (order.OrderStatus == ConstantParam.OrderStatus_RECEIPT)
                        {
                            //如果订单支付方式为微信在线支付
                            if (order.PayWay == 1)
                            {
                                //微信退款
                                //获取商家账户信息
                                var wxAccount = order.Shop.ShopAccounts.Where(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_WeChat).FirstOrDefault();
                                if (wxAccount == null)
                                {
                                    resultModel.Msg = "该订单所属商家未设置账户信息";
                                    return resultModel;
                                }
                                //获取商家账户信息
                                string WeixinAppId = wxAccount.Number;
                                string WeixinMchId = wxAccount.MerchantNo;
                                string WeixinPayKey = wxAccount.AccountKey;
                                //申请退款
                                string result = ApplyRefund(order, WeixinAppId, WeixinMchId, WeixinPayKey);
                                //如果请求失败
                                if (result == null)
                                {
                                    resultModel.Msg = APIMessage.WEIXIN_REFUND_FAIL;
                                    return resultModel;
                                }
                                //解析返回数据
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(result);
                                //如果返回成功
                                string return_code = doc.GetElementsByTagName("return_code")[0].InnerText;
                                if (return_code == "SUCCESS")
                                {
                                    string result_code = doc.GetElementsByTagName("result_code")[0].InnerText;
                                    if (result_code == "SUCCESS")
                                    {
                                        order.OrderStatus = model.OrderStatus;
                                        order.RecedeType = 2;
                                        resultModel.result = "订单退款申请成功";
                                    }
                                    else
                                    {
                                        resultModel.Msg = APIMessage.WEIXIN_REFUND_FAIL;
                                        return resultModel;
                                    }
                                }
                                else
                                {
                                    resultModel.Msg = APIMessage.WEIXIN_REFUND_FAIL;
                                    return resultModel;
                                }

                            }
                            //支付宝支付退款
                            else if (order.PayWay == 2)
                            {
                            }
                            else
                            {
                                order.OrderStatus = model.OrderStatus;
                                order.RecedeType = 0;
                            }
                        }
                        else if (order.OrderStatus == ConstantParam.OrderStatus_CONFIRM)
                        {
                            order.OrderStatus = model.OrderStatus;
                            order.RecedeType = 0;
                        }
                        order.RecedeTime = DateTime.Now;
                        alert = "您在" + order.Shop.ShopName + "提交的订单已被商家退单";

                        //如果订单修改成功
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
                    }
                    else
                    {
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
                }
                else
                {
                    resultModel.Msg = APIMessage.ORDER_NOEXIST;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
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

            string cert = HttpContext.Current.Server.MapPath("/App_Data/apiclient_cert.p12");
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
        /// 删除订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteOrder(DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                var user = shopUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user == null)
                {
                    resultModel.Msg = APIMessage.NO_USER;
                    return resultModel;
                }
                //如果验证Token不通过或已过期
                if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                {
                    resultModel.Msg = APIMessage.TOKEN_INVALID;
                    return resultModel;
                }
                //更新最近登录时间和Token失效时间
                user.LatelyLoginTime = DateTime.Now;
                user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                shopUserBll.Update(user);

                //获取指定订单
                IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsStoreHided == ConstantParam.DEL_FLAG_DEFAULT);

                if (order != null)
                {
                    if (order.OrderStatus == ConstantParam.OrderStatus_FINISH || order.OrderStatus == ConstantParam.OrderStatus_EXIT || order.OrderStatus == ConstantParam.OrderStatus_CLOSE)
                    {
                        order.IsStoreHided = ConstantParam.DEL_FLAG_DELETE;
                        //删除订单
                        if (!orderBll.Update(order))
                        {
                            resultModel.Msg = "订单删除失败";
                        }
                    }
                    else
                    {
                        resultModel.Msg = "当前状态的订单不能被删除";
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.ORDER_NOEXIST;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }
    }
}
