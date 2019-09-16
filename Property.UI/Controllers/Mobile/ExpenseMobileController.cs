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
    /// 缴费相关API接口控制器
    /// </summary>
    public class ExpenseMobileController : ApiController
    {
        /// <summary>
        /// 获取待缴费记录列表
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel NoExpenseRecordList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
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

                    //初始化查询条件
                    var DoorIds = owner.PropertyIdentityVerification.Where(v => v.DoorId != null).Select(m => m.DoorId);
                    var CompanyIds = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null).Select(m => m.BuildCompanyId);
                    Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => u.IsPayed == ConstantParam.PAYED_FALSE && (DoorIds.Contains(u.BuildDoorId) || CompanyIds.Contains(u.BuildCompanyId));
                    // 获取当前用户对应业主的缴费记录
                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var list = expenseDetailsBLL.GetPageList(where, "CreateDate", false, model.PageIndex).Select(e => new
                    {
                        RecordId = e.Id,
                        ExpenseTypeName = e.PropertyExpenseType.Name,
                        ExpenseDateDes = e.ExpenseDateDes,
                        PlaceId = e.BuildCompanyId == null ? e.BuildDoor.BuildUnit.Build.PropertyPlace.Id : e.BuildCompany.PropertyPlace.Id,
                        PlaceName = e.BuildCompanyId == null ? e.BuildDoor.BuildUnit.Build.PropertyPlace.Name : e.BuildCompany.PropertyPlace.Name,
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = expenseDetailsBLL.GetList(where).Count();
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
        /// 设置记录为已在线支付
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetRecordToExpensed(DetailSearchModel model)
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

                    //设置缴费记录为已在线缴费
                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var record = expenseDetailsBLL.GetEntity(e => e.Id == model.Id);
                    if (record != null)
                    {
                        record.IsPayed = ConstantParam.PAYED_TRUE;
                        record.PaymentType = 2;
                        record.PayedDate = DateTime.Now;

                        expenseDetailsBLL.Update(record);
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.EXPENSE_RECORD_NOEXIST;
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
        /// 生成微信支付订单
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

                    //生成微信支付订单
                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var record = expenseDetailsBLL.GetEntity(e => e.Id == model.Id);
                    if (record != null)
                    {
                        //获取物业缴费账户信息
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
                        if (wxAccount == null)
                        {
                            resultModel.Msg = "该用户所属物业未设置账户信息";
                            return resultModel;
                        }
                        //获取物业账户信息
                        string WeixinAppId = wxAccount.Number;
                        string WeixinMchId = wxAccount.MerchantNo;
                        string WeixinPayKey = wxAccount.AccountKey;
                        //生成预订单
                        string result = CreateTradePost(record, WeixinAppId, WeixinMchId, WeixinPayKey);

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
                        resultModel.Msg = APIMessage.EXPENSE_RECORD_NOEXIST;
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
        /// Post请求 生成订单
        /// </summary>
        /// <returns></returns>
        private string CreateTradePost(T_HouseUserExpenseDetails record,string WeixinAppId, string WeixinMchId,string WeixinPayKey)
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
            string body = record.PropertyExpenseType.Name + "（" + record.ExpenseDateDes + "）";
            varBody.Append("<body>" + body + "</body>");
            signStr.Append("body=" + body + "&");
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
            string notifyUrl = PropertyUtils.GetConfigParamValue("HostUrl") + "/Common/WeixinExpenseNotifyUrl";
            varBody.Append("<notify_url>" + notifyUrl + "</notify_url>");
            signStr.Append("notify_url=" + notifyUrl + "&");
            //商户订单号
            string no = DateTime.Now.ToFileTime().ToString() + new Random().Next(1000);
            varBody.Append("<out_trade_no>" + no + "</out_trade_no>");
            signStr.Append("out_trade_no=" + no + "&");
            //保存订单号
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            record.PayTradeNo = no;
            expenseDetailsBLL.Update(record);

            //终端ID
            varBody.Append("<spbill_create_ip>218.58.55.130</spbill_create_ip>");
            signStr.Append("spbill_create_ip=" + "218.58.55.130" + "&");
            //总金额
            int fee = Convert.ToInt32(record.Expense * 100);
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
        /// 获取缴费历史记录列表
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel ExpensedRecordList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
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

                    //初始化查询条件
                    var DoorIds = owner.PropertyIdentityVerification.Where(v => v.DoorId != null).Select(m => m.DoorId);
                    var CompanyIds = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null).Select(m => m.BuildCompanyId);
                    Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => u.IsPayed == ConstantParam.PAYED_TRUE && (DoorIds.Contains(u.BuildDoorId) || CompanyIds.Contains(u.BuildCompanyId));
                    // 获取当前用户对应业主的缴费记录
                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var list = expenseDetailsBLL.GetPageList(where, "PayedDate", false, model.PageIndex).Select(e => new
                    {
                        RecordId = e.Id,
                        ExpenseTypeName = e.PropertyExpenseType.Name,
                        ExpenseDateDes = e.ExpenseDateDes,
                        PlaceName = e.BuildCompanyId == null ? e.BuildDoor.BuildUnit.Build.PropertyPlace.Name : e.BuildCompany.PropertyPlace.Name,
                        Expense = e.Expense,
                        PayedTime = e.PayedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = expenseDetailsBLL.GetList(where).Count();
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
        /// 获取缴费记录详细接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel ExpenseRecordDetail([FromUri]DetailSearchModel model)
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

                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var record = expenseDetailsBLL.GetEntity(e => e.Id == model.Id);
                    if (record != null)
                    {
                        //返回详细数据
                        resultModel.result = new
                        {
                            RecordId = record.Id,
                            ExpenseTypeName = record.PropertyExpenseType.Name,
                            ExpenseDateDes = record.ExpenseDateDes,
                            Expense = record.Expense,
                            PayedTime = record.PayedDate != null ? record.PayedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            PaymentType = record.PaymentType != null ? (record.PaymentType.Value == 1 ? "前台缴费" : "在线缴费") : "",
                            PlaceName = record.BuildCompanyId == null ? record.BuildDoor.BuildUnit.Build.PropertyPlace.Name : record.BuildCompany.PropertyPlace.Name,
                            Door = record.BuildCompanyId == null ? record.BuildDoor.BuildUnit.Build.BuildName + record.BuildDoor.BuildUnit.UnitName
                            + record.BuildDoor.DoorName : record.BuildCompany.Name,
                            OwnerName = record.BuildDoor != null && record.BuildDoor.HouseUsers.Count > 0 ? record.BuildDoor.HouseUsers.FirstOrDefault().Name : "",
                            Tel = record.BuildDoor != null ? (record.BuildDoor.HouseUsers.Count > 0 ? record.BuildDoor.HouseUsers.FirstOrDefault().Phone : "") : record.BuildCompany.Phone,
                        };
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.EXPENSE_RECORD_NOEXIST;
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
        /// 获取缴费订单是否已缴费
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel IsExpensed([FromUri]DetailSearchModel model)
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

                    //获取指定缴费订单
                    IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                    var record = expenseDetailsBLL.GetEntity(e => e.Id == model.Id);
                    if (record != null)
                    {
                        resultModel.result = record.IsPayed == ConstantParam.PAYED_TRUE;
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.EXPENSE_RECORD_NOEXIST;
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
