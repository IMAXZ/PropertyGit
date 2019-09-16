using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 商家APP共通 接口控制器
    /// </summary>
    public class StoreMobileController : ApiController
    {
        /// <summary>
        /// 商家客户端登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel Login(OwnerLoginModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户名查找用户
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                var user = shopUserBll.GetEntity(u => u.UserName == model.UserName && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //1.判断用户名是否正确
                if (user == null)
                {
                    resultModel.Msg = APIMessage.NAME_ERROR;
                    return resultModel;
                }

                //2.判断密码是否正确
                string md5Str = PropertyUtils.GetMD5Str(model.Password);
                if (user.Password != md5Str)
                {
                    resultModel.Msg = APIMessage.PWD_ERROR;
                    return resultModel;
                }

                //产生随机令牌
                var token = System.Guid.NewGuid().ToString("N");
                //更新用户令牌和最近登录时间及Token失效时间
                user.Token = token;
                user.LatelyLoginTime = DateTime.Now;
                user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                shopUserBll.Update(user);

                //返回登录用户的ID和用户名以及令牌
                resultModel.result = new
                {
                    token = token,
                    userId = user.Id,
                    userName = user.UserName,
                    IsCreateShop = user.Shops.Count > 0,
                    ShopId = user.Shops.FirstOrDefault() == null ? -1 : user.Shops.FirstOrDefault().Id,
                    IsGreenType = user.Shops.FirstOrDefault() == null ? false : user.Shops.FirstOrDefault().Type.Contains(ConstantParam.SHOP_TYPE_0.ToString())
                };

                //推送设备管理
                IShopUserPushBLL userPushBll = BLLFactory<IShopUserPushBLL>.GetBLL("ShopUserPushBLL");
                var userPush = userPushBll.GetEntity(p => p.UserId == user.Id);
                var userPush1 = userPushBll.GetEntity(p => p.RegistrationId == model.RegistrationId);
                if (userPush != null)
                {
                    userPush.RegistrationId = model.RegistrationId;
                    userPushBll.Update(userPush);
                }
                else if (userPush1 != null)
                {
                    userPush1.UserId = user.Id;
                    userPushBll.Update(userPush1);
                }
                else
                {
                    userPush = new T_ShopUserPush()
                    {
                        UserId = user.Id,
                        RegistrationId = model.RegistrationId
                    };
                    userPushBll.Save(userPush);
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model">密码修改模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel ChangePassword(OwnerChangePasswordModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要修改密码的商家用户
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

                    string OldMd5Pwd = PropertyUtils.GetMD5Str(model.OldPwd);
                    //如果输入的旧密码与数据库中不一致
                    if (OldMd5Pwd != user.Password)
                    {
                        resultModel.Msg = APIMessage.OLD_PWD_ERROR;
                    }
                    else
                    {
                        //修改密码并保存
                        user.Password = PropertyUtils.GetMD5Str(model.NewPwd);
                        shopUserBll.Update(user);
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
        /// 获取最新版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetVersionInfo([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
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

                    //调用版本信息BLL层获取最新的版本信息
                    IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
                    var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_SHOP, "VersionCode", false);
                    //如果版本信息不为空
                    if (Versions != null && Versions.Count() > 0)
                    {
                        var highestVersion = Versions.First();
                        if (highestVersion != null)
                        {
                            resultModel.result = new
                            {
                                VersionCode = highestVersion.VersionCode,
                                VersionName = highestVersion.VersionName,
                                Desc = highestVersion.Desc,
                                ApkFilePath = highestVersion.ApkFilePath
                            };
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.NO_APP;
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
        /// 获取商家商品销售统计总量数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetSellStatisticalALLData([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
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

                    //获取当前用户的门店
                    var Shop = user.Shops.FirstOrDefault();
                    if (Shop != null)
                    {
                        //获取交易完成的订单
                        var Orders = Shop.Orders.Where(o => o.OrderStatus == ConstantParam.OrderStatus_FINISH);
                        //年统计
                        int[] yearOrderData = new int[DateTime.Today.Month];
                        double[] yearInComeData = new double[DateTime.Today.Month];
                        for (int i = 0; i < yearOrderData.Count(); i++)
                        {
                            DateTime d = DateTime.Today.AddMonths(i + 1 - (int)DateTime.Today.Month);
                            DateTime start = d.AddDays(1 - d.Day);
                            DateTime end = start.AddMonths(1);
                            yearOrderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                            yearInComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                        }
                        //月统计
                        int[] monthOrderData = new int[DateTime.Today.Day];
                        double[] monthInComeData = new double[DateTime.Today.Day];
                        for (int i = 0; i < monthOrderData.Count(); i++)
                        {
                            DateTime start = DateTime.Today.AddDays(i + 1 - (int)DateTime.Today.Day);
                            DateTime end = start.AddDays(1);
                            monthOrderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                            monthInComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                        }
                        //周统计
                        int index = 0;
                        if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                        {
                            index = 7;
                        }
                        else
                        {
                            index = (int)DateTime.Today.DayOfWeek;
                        }
                        int[] weekOrderData = new int[index];
                        double[] weekInComeData = new double[index];
                        for (int i = 0; i < weekOrderData.Count(); i++)
                        {
                            DateTime start = DateTime.Today.AddDays(i + 1 - index);
                            DateTime end = start.AddDays(1);
                            weekOrderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                            weekInComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                        }
                        resultModel.result = new
                        {
                            OrderCount = Orders.Count(),//订单总量
                            InCome = Convert.ToDouble(Convert.ToDecimal(Orders.Select(o => o.OrderPrice).Sum())), //总收入
                            YearOrderData = yearOrderData,  //年订单统计
                            YearInComeData = yearInComeData,//年收入统计
                            MonthOrderData = monthOrderData,//月订单统计
                            MonthInComeData = monthInComeData,//月收入统计
                            WeekOrderData = weekOrderData,//周订单统计
                            WeekInComeData = weekInComeData //周收入统计
                        };
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
        /// 获取商家商品销售统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetSellStatisticalData([FromUri]SellStatisticalDataModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
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

                    //获取当前用户的门店
                    var Shop = user.Shops.FirstOrDefault();
                    if (Shop != null)
                    {
                        //获取交易完成的订单
                        var Orders = Shop.Orders.Where(o => o.OrderStatus == ConstantParam.OrderStatus_FINISH);
                        if (model.Flag == 0)
                        {
                            int[] orderData = new int[DateTime.Today.Month];
                            double[] inComeData = new double[DateTime.Today.Month];
                            for (int i = 0; i < orderData.Count(); i++)
                            {
                                DateTime d = DateTime.Today.AddMonths(i + 1 - (int)DateTime.Today.Month);
                                DateTime start = d.AddDays(1 - d.Day);
                                DateTime end = start.AddMonths(1);
                                orderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                                inComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                            }
                            resultModel.result = new
                            {
                                OrderCount = Orders.Count(),//订单总量
                                InCome = Convert.ToDouble(Convert.ToDecimal(Orders.Select(o => o.OrderPrice).Sum())), //总收入
                                OrderData = orderData, //订单数列表（年统计）
                                InComeData = inComeData //收入列表（年统计）
                            };
                        }
                        else if (model.Flag == 1)
                        {
                            int[] orderData = new int[DateTime.Today.Day];
                            double[] inComeData = new double[DateTime.Today.Day];
                            for (int i = 0; i < orderData.Count(); i++)
                            {
                                DateTime start = DateTime.Today.AddDays(i + 1 - (int)DateTime.Today.Day);
                                DateTime end = start.AddDays(1);
                                orderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                                inComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                            }
                            resultModel.result = new
                            {
                                OrderCount = Orders.Count(),//订单总量
                                InCome = Convert.ToDouble(Convert.ToDecimal(Orders.Select(o => o.OrderPrice).Sum())), //总收入
                                OrderData = orderData, //订单数列表（年统计）
                                InComeData = inComeData //收入列表（年统计）
                            };
                        }
                        else if (model.Flag == 2) 
                        {
                            int index = 0;
                            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                            {
                                index = 7;
                            }
                            else 
                            {
                                index = (int)DateTime.Today.DayOfWeek;
                            }

                            int[] orderData = new int[index];
                            double[] inComeData = new double[index];
                            for (int i = 0; i < orderData.Count(); i++)
                            {
                                DateTime start = DateTime.Today.AddDays(i + 1 - index);
                                DateTime end = start.AddDays(1);
                                orderData[i] = Orders.Count(o => o.CompleteTime >= start && o.CompleteTime < end);
                                inComeData[i] = Convert.ToDouble(Convert.ToDecimal(Orders.Where(o => o.CompleteTime >= start && o.CompleteTime < end).Select(o => o.OrderPrice).Sum()));
                            }
                            resultModel.result = new
                            {
                                OrderCount = Orders.Count(),//订单总量
                                InCome = Convert.ToDouble(Convert.ToDecimal(Orders.Select(o => o.OrderPrice).Sum())), //总收入
                                OrderData = orderData, //订单数列表（年统计）
                                InComeData = inComeData //收入列表（年统计）
                            };
                        }
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
