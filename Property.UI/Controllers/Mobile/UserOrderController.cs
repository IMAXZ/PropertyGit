using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Web.Http;
using System.Xml;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 业主用户订单支付接口控制器
    /// </summary>
    public class UserOrderController : ApiController
    {

        /// <summary>
        /// 获取我的订单分页列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel OrderList([FromUri]OrderPagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取订单列表数据
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    Expression<Func<T_Order, bool>> where = o => o.AppUserId == owner.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT;
                    if (model.OrderStatus != null)
                    {
                        where = PredicateBuilder.And(where, o => o.OrderStatus == model.OrderStatus);
                    }
                    resultModel.result = orderBll.GetPageList(where, "OrderDate", false, model.PageIndex).Select(o => new
                    {
                        OrderId = o.Id,
                        OrderNo = o.OrderNo,
                        ShopId = o.Shop.Id,
                        ShopName = o.Shop.ShopName,
                        ShopImg = string.IsNullOrEmpty(o.Shop.ImgThumbnail) ? "" : o.Shop.ImgThumbnail.Split(';')[0],
                        OrderTime = o.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        OrderStatus = o.OrderStatus,
                        RecedeType = o.RecedeType,
                        RemainingTime = o.PayDate == null || o.PayDate.Value.AddHours(2) < DateTime.Now ? "0小时0分" : (o.PayDate.Value.AddHours(2) - DateTime.Now).Hours + "小时" + (o.PayDate.Value.AddHours(2) - DateTime.Now).Minutes + "分",
                        ExitOrderReason = o.Reason,
                        OrderPrice = o.OrderPrice,
                        PayWay = o.PayWay,
                        RefundStatus = GetRefundResult(o.Id),
                        PayTradeNo = o.PayTradeNo,
                        RecedeTime = o.RecedeTime != null ? o.RecedeTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        Memo = string.IsNullOrEmpty(o.Memo) ? "" : o.Memo,
                    });
                    resultModel.Total = orderBll.Count(where);
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
        /// 我的订单详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel OrderDetail([FromUri]DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取指定订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);
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
                            Memo = order.Memo,
                            RemainingTime = order.PayDate == null || order.PayDate.Value.AddHours(2) < DateTime.Now ? "0小时0分" : (order.PayDate.Value.AddHours(2) - DateTime.Now).Hours + "小时" + (order.PayDate.Value.AddHours(2) - DateTime.Now).Minutes + "分",
                            ExitOrderReason = order.Reason,
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
        /// 提交订单
        /// </summary>
        /// <param name="model">订单模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SubmitOrder([FromBody]JObject value)
        {
            //{\"OrderPrice\":8.88,\"ShopId\":7,\"Memo\":null,\"ShipAddressId\":1,\"OrderGoods\":[{\"SaleId\":103,\"SaledAmount\":1}],\"UserId\":56,\"Token\":\"65e196f05198447b8f30742e24e173d9\"}
            var model = JsonConvert.DeserializeObject<OrderModel>(value.ToString());
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    //生成唯一订单号
                    Random r = new Random();
                    string orderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(100, 1000);
                    while (true)
                    {
                        if (!orderBll.Exist(o => o.OrderNo == orderNo))
                        {
                            break;
                        }
                        else
                        {
                            orderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(100, 1000);
                        }
                    }
                    //添加订单
                    T_Order order = new T_Order()
                    {
                        OrderNo = orderNo,
                        AppUserId = model.UserId,
                        ShopId = model.ShopId,
                        ShipAddressId = model.ShipAddressId,
                        OrderDate = DateTime.Now,
                        OrderPrice = model.OrderPrice,
                        Memo = model.Memo,
                        OrderStatus = ConstantParam.OrderStatus_NOPAY
                    };

                    IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    //订单添加商品详细
                    foreach (var Goods in model.OrderGoods)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.SaleId);
                        if (sale != null)
                        {
                            order.OrderDetails.Add(new T_OrderDetails()
                            {
                                ShopSaleId = Goods.SaleId,
                                SaledAmount = Goods.SaledAmount,
                                Price = sale.Price * Goods.SaledAmount
                            });
                        }
                    }
                    orderBll.Save(order);

                    var newOrder = orderBll.GetEntity(o => o.OrderNo == orderNo);
                    if (newOrder != null)
                    {
                        resultModel.result = newOrder.Id;

                    }

                    IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var ShopUserId = shopBLL.GetEntity(s => s.Id == model.ShopId).ShopUserId;

                    //推送给订单所属商家
                    IShopUserPushBLL userPushBLL = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                    var userPush = userPushBLL.GetEntity(p => p.UserId == ShopUserId);
                    if (userPush != null)
                    {
                        string registrationId = userPush.RegistrationId;
                        string alert = "您有订单号为" + order.OrderNo + "的新订单，点击查看详情";
                        //通知信息
                        PropertyUtils.SendPush("新订单来啦", alert, ConstantParam.MOBILE_TYPE_SHOP, registrationId);
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
        /// 提交订单
        /// </summary>
        /// <param name="model">订单模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SubmitOrder2([FromBody]JObject value)
        {
            //{\"OrderPrice\":8.88,\"ShopId\":7,\"Memo\":null,\"ShipAddressId\":1,\"OrderGoods\":[{\"SaleId\":103,\"SaledAmount\":1}],\"UserId\":56,\"Token\":\"65e196f05198447b8f30742e24e173d9\"}
            var model = JsonConvert.DeserializeObject<OrderModel>(value.ToString());
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //判断订单中的商品是否还在出售中，价格是否有改动
                    var goodsPrices = 0.0;

                    IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    foreach (var Goods in model.OrderGoods)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.SaleId);
                        if (sale == null || sale.InSales == 0 || sale.RemainingAmout < Goods.SaledAmount)
                        {
                            resultModel.Msg = "订单中有商品已售完或已下架，请重新选购";
                            return resultModel;
                        }
                        goodsPrices += sale.Price * Goods.SaledAmount;
                    }

                    var costPrice = 0.0;
                    //获取要查询的门店运费
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);
                    if (shop != null)
                    {
                        var cost = shop.ShopShippingCosts.FirstOrDefault();
                        if (cost != null && cost.IsFree == 0 && (cost.OrderExpense == null || goodsPrices < cost.OrderExpense.Value))
                        {
                            costPrice = cost.Price.Value;
                        }
                    }

                    if (model.OrderPrice != goodsPrices + costPrice)
                    {
                        resultModel.Msg = "订单中有商品价格发生变化，请重新选购";
                        return resultModel;
                    }
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    //生成唯一订单号
                    Random r = new Random();
                    string orderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(100, 1000);
                    while (true)
                    {
                        if (!orderBll.Exist(o => o.OrderNo == orderNo))
                        {
                            break;
                        }
                        else
                        {
                            orderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(100, 1000);
                        }
                    }
                    //添加订单
                    T_Order order = new T_Order()
                    {
                        OrderNo = orderNo,
                        AppUserId = model.UserId,
                        ShopId = model.ShopId,
                        ShipAddressId = model.ShipAddressId,
                        OrderDate = DateTime.Now,
                        OrderPrice = model.OrderPrice,
                        Memo = model.Memo,
                        OrderStatus = ConstantParam.OrderStatus_NOPAY
                    };

                    //订单添加商品详细
                    foreach (var Goods in model.OrderGoods)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.SaleId);
                        order.OrderDetails.Add(new T_OrderDetails()
                        {
                            ShopSaleId = Goods.SaleId,
                            SaledAmount = Goods.SaledAmount,
                            Price = sale.Price * Goods.SaledAmount
                        });
                    }
                    //如果订单提交成功
                    if (orderBll.SubmitOrder(order))
                    {
                        var newOrder = orderBll.GetEntity(o => o.OrderNo == orderNo);
                        if (newOrder != null)
                        {
                            resultModel.result = new
                            {
                                OrderId = newOrder.Id,
                                OrderNo = newOrder.OrderNo
                            };
                        }

                        IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                        var ShopUserId = shopBLL.GetEntity(s => s.Id == model.ShopId).ShopUserId;

                        //推送给订单所属商家
                        IShopUserPushBLL userPushBLL = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                        var userPush = userPushBLL.GetEntity(p => p.UserId == ShopUserId);
                        if (userPush != null)
                        {
                            string registrationId = userPush.RegistrationId;
                            string alert = "您有订单号为" + order.OrderNo + "的新订单，点击查看详情";
                            //通知信息
                            PropertyUtils.SendPush("新订单来啦", alert, ConstantParam.MOBILE_TYPE_SHOP, registrationId);
                        }
                    }
                    else
                    {
                        resultModel.Msg = "订单提交失败";
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
        /// 生成微信支付预订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel CreateWeixinPayTrade(DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //生成微信预支付订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(e => e.Id == model.Id);
                    if (order != null)
                    {
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

                        string result = CreateTradePost(order, WeixinAppId, WeixinMchId, WeixinPayKey);
                        //如果请求失败
                        if (result == null)
                        {
                            resultModel.Msg = APIMessage.WEIXIN_YUORDER_FAIL;
                            return resultModel;
                        }
                        //解析返回结果
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(result);
                        string return_code = doc.GetElementsByTagName("return_code")[0].InnerText;
                        //如果返回成功
                        if (return_code == "SUCCESS")
                        {
                            string result_code = doc.GetElementsByTagName("result_code")[0].InnerText;
                            if (result_code == "SUCCESS")
                            {
                                //预支付交易会话标识
                                string prepayid = doc.GetElementsByTagName("prepay_id")[0].InnerText;

                                //随机字符串
                                string str = "1234567890abcdefghijklmnopqrstuvwxyz";
                                Random r = new Random();
                                string PayRandomStr = "";
                                for (int i = 0; i < 32; i++)
                                {
                                    PayRandomStr += str[r.Next(str.Length)].ToString();
                                }
                                //时间戳
                                var timestamp = Convert.ToInt64(DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds / 10000) * 10;
                                //签名字符串
                                string PaySignStr = "appid=" + WeixinAppId + "&noncestr=" + PayRandomStr + "&package=Sign=WXPay&partnerid="
                                    + WeixinMchId + "&prepayid=" + prepayid + "&timestamp=" + timestamp + "&key=" + WeixinPayKey;
                                resultModel.result = new
                                {
                                    appid = WeixinAppId,
                                    partnerid = WeixinMchId,
                                    package = "Sign=WXPay",
                                    prepayid = prepayid,
                                    noncestr = PayRandomStr,
                                    timestamp = timestamp,
                                    sign = PropertyUtils.GetMD5Str(PaySignStr.ToString()).ToUpper()
                                };
                            }
                            else
                            {
                                string err_code_des = doc.GetElementsByTagName("err_code_des")[0].InnerText;
                                resultModel.Msg = err_code_des;
                            }
                        }
                        else
                        {
                            string return_msg = doc.GetElementsByTagName("return_msg")[0].InnerText;
                            resultModel.Msg = return_msg;
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ORDER_NOEXIST;
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
        /// Post请求 生成微信预订单
        /// </summary>
        /// <returns></returns>
        private string CreateTradePost(T_Order order, string WeixinAppId, string WeixinMchId, string WeixinPayKey)
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
            //商品描述
            string body = order.Shop.ShopName + "（" + order.OrderNo + "）";
            varBody.Append("<body>" + body + "</body>");
            signStr.Append("body=" + body + "&");
            //商品描述
            string detail = order.OrderDetails.Select(od => od.ShopSale.Title).ToArray().ToString();
            varBody.Append("<detail>" + detail + "</detail>");
            signStr.Append("detail=" + detail + "&");
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
            //通知地址
            string notifyUrl = PropertyUtils.GetConfigParamValue("HostUrl") + "/Common/WeixinPayNotifyUrl";
            varBody.Append("<notify_url>" + notifyUrl + "</notify_url>");
            signStr.Append("notify_url=" + notifyUrl + "&");
            //商户订单号
            string no = DateTime.Now.ToFileTime().ToString() + new Random().Next(1000);
            varBody.Append("<out_trade_no>" + no + "</out_trade_no>");
            signStr.Append("out_trade_no=" + no + "&");
            //编辑订单中的微信支付订单号
            order.PayTradeNo = no;
            IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
            orderBll.Update(order);
            //终端IP
            varBody.Append("<spbill_create_ip>218.58.55.130</spbill_create_ip>");
            signStr.Append("spbill_create_ip=" + "218.58.55.130" + "&");
            //总金额
            int fee = Convert.ToInt32(order.OrderPrice * 100);
            varBody.Append("<total_fee>" + fee + "</total_fee>");
            signStr.Append("total_fee=" + fee + "&");
            //交易类型
            varBody.Append("<trade_type>APP</trade_type>");
            signStr.Append("trade_type=APP&");
            //签名
            signStr.Append("key=" + WeixinPayKey);
            varBody.Append("<sign>" + PropertyUtils.GetMD5Str(signStr.ToString()).ToUpper() + "</sign>");
            varBody.Append("</xml>");
            #endregion

            //发送HTTP POST请求
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";
            // 信任所有证书
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
            return result;
        }

        /// <summary>
        /// 信任所有证书。
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 获取订单是否已支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel IsPayedOrder([FromUri]DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取指定订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);
                    if (order != null)
                    {
                        resultModel.result = order.OrderStatus > 0;
                    }
                    else
                    {
                        resultModel.Msg = "订单已不存在";
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
        /// 支付订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel PayOrder(OrderPayModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.Id == model.OrderId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);
                    if (order != null)
                    {
                        //如果订单状态为待付款
                        if (order.OrderStatus == ConstantParam.OrderStatus_NOPAY)
                        {
                            //如果是绿色直供在线支付方式
                            if (order.Shop.Type.Contains(ConstantParam.SHOP_TYPE_0.ToString()))
                            {
                                order.OrderStatus = ConstantParam.OrderStatus_RECEIPT;
                            }
                            else
                            {
                                order.OrderStatus = ConstantParam.OrderStatus_CONFIRM;
                            }
                            order.PayWay = model.PayWay;
                            order.PayDate = DateTime.Now;
                            //如果支付成功
                            if (orderBll.Update(order))
                            {
                                //返回订单信息
                                resultModel.result = new
                                {
                                    OrderNo = order.OrderNo,
                                    ShopName = order.Shop.ShopName,
                                    TradeTime = order.PayDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                    PayWay = order.PayWay,
                                    TotalPrice = order.OrderPrice
                                };

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
                            else
                            {
                                resultModel.Msg = "订单支付失败";
                            }
                        }
                        else
                        {
                            if (order.PayWay == 1)
                            {
                                //返回订单信息
                                resultModel.result = new
                                {
                                    OrderNo = order.OrderNo,
                                    ShopName = order.Shop.ShopName,
                                    TradeTime = order.PayDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                    PayWay = order.PayWay,
                                    TotalPrice = order.OrderPrice
                                };
                            }
                            else
                            {
                                resultModel.Msg = APIMessage.ORDER_NOPAYING;
                            }
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ORDER_NOEXIST;
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
        /// 编辑订单
        /// </summary>
        /// <param name="value">Json参数</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel EditOrder([FromBody]JObject value)
        {
            //{\"OrderPrice\":8.88,\"OrderId\":1,\"Memo\":null,\"ShipAddressId\":1,\"OrderGoods\":[{\"SaleId\":103,\"SaledAmount\":1}],\"UserId\":56,\"Token\":\"65e196f05198447b8f30742e24e173d9\"}
            var model = JsonConvert.DeserializeObject<OrderEditModel>(value.ToString());
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取要编辑的订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    T_Order order = orderBll.GetEntity(o => o.Id == model.OrderId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);
                    if (order != null)
                    {
                        if (order.OrderStatus < ConstantParam.OrderStatus_RECEIPT)
                        {
                            order.ShipAddressId = model.ShipAddressId;
                            order.OrderPrice = model.OrderPrice;
                            order.Memo = model.Memo;
                            //编辑订单
                            IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                            Dictionary<int, int> goodsDic = new Dictionary<int, int>();
                            foreach (var goods in model.OrderGoods)
                            {
                                var sale = saleBll.GetEntity(s => s.Id == goods.SaleId);
                                if (sale == null || sale.InSales == 0 || sale.RemainingAmout < goods.SaledAmount)
                                {
                                    resultModel.Msg = "订单中有商品已下架";
                                    return resultModel;
                                }
                                goodsDic.Add(goods.SaleId, goods.SaledAmount);
                            }
                            //如果编辑失败
                            if (!orderBll.UpdateOrder(order, goodsDic))
                            {
                                resultModel.Msg = "订单编辑失败";
                            }

                            IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                            var ShopUserId = shopBLL.GetEntity(s => s.Id == order.ShopId).ShopUserId;

                            //推送给订单所属商家
                            IShopUserPushBLL userPushBLL = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                            var userPush = userPushBLL.GetEntity(p => p.UserId == ShopUserId);
                            if (userPush != null)
                            {
                                string registrationId = userPush.RegistrationId;
                                string alert = "用户修改了订单号为" + order.OrderNo + "的订单，点击查看详情";
                                //通知信息
                                PropertyUtils.SendPush("订单修改通知", alert, ConstantParam.MOBILE_TYPE_SHOP, registrationId);
                            }
                        }
                        else
                        {
                            resultModel.Msg = "该订单不能编辑";
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ORDER_NOEXIST;
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
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel CancelOrder(DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取指定订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);
                    if (order != null)
                    {
                        if (order.OrderStatus < ConstantParam.OrderStatus_RECEIPT)
                        {
                            order.OrderStatus = ConstantParam.OrderStatus_CLOSE;
                            if (!orderBll.CancelOrder(order))
                            {
                                resultModel.Msg = "订单取消失败";
                            }
                            IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                            var ShopUserId = shopBLL.GetEntity(s => s.Id == order.ShopId).ShopUserId;

                            //推送给订单所属商家
                            IShopUserPushBLL userPushBLL = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                            var userPush = userPushBLL.GetEntity(p => p.UserId == ShopUserId);
                            if (userPush != null)
                            {
                                string registrationId = userPush.RegistrationId;
                                string alert = "用户取消了订单号为" + order.OrderNo + "的订单，点击查看详情";
                                //通知信息
                                PropertyUtils.SendPush("订单取消通知", alert, ConstantParam.MOBILE_TYPE_SHOP, registrationId);
                            }
                        }
                        else
                        {
                            resultModel.Msg = "该订单不能取消";
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ORDER_NOEXIST;
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
        /// 删除订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteOrder(DetailSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取指定订单
                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                    var order = orderBll.GetEntity(o => o.Id == model.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && o.IsUserHided == ConstantParam.DEL_FLAG_DEFAULT);

                    if (order != null)
                    {
                        if (order.OrderStatus == ConstantParam.OrderStatus_FINISH || order.OrderStatus == ConstantParam.OrderStatus_EXIT || order.OrderStatus == ConstantParam.OrderStatus_CLOSE)
                        {
                            order.IsUserHided = ConstantParam.DEL_FLAG_DELETE;
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

    }
}
