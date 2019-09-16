using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    public class ShopInfoMobileController : ApiController
    {
        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopInfo([FromUri]ShopInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);
                    //如果商家信息不为空
                    if (shop != null)
                    {
                        resultModel.result = new
                        {
                            ShopName = shop.ShopName,
                            ShopContent = string.Format("MobilePage/StoreIntroduce?ShopId={0}", shop.Id),
                            ShopMainSale = shop.MainSale,
                            StartBusinessTime = shop.StartBusinessTime,
                            EndBusinessTime = shop.EndBusinessTime,
                            ShopAddress = shop.Address,
                            Phone = shop.Phone,
                            ShopPicList = shop.ImgPath
                        };
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
        /// 设置商家信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetShopInfo(ShopInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);

                    //设置主营内容
                    if (shop != null)
                    {
                        shop.ShopName = model.ShopName;
                        //shop.Content = model.ShopContent;
                        shop.MainSale = model.MainSale;
                        shop.StartBusinessTime = model.StartBusinessTime;
                        shop.EndBusinessTime = model.EndBusinessTime;
                        shop.Address = model.Address;
                        shop.Phone = model.Phone;
                        shop.UpdateTime = DateTime.Now;
                        shopBll.Update(shop);
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
        /// 获取商家支付种类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopPayType([FromUri]ShopInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);
                    //如果商家信息不为空
                    if (shop != null)
                    {
                        var payTypeIds = shop.ShopPaymentManagements.Select(s => s.PayTypeId);
                        //是否是绿色直供
                        bool isGreenSupplied = shop.Type.Contains(Property.Common.ConstantParam.SHOP_TYPE_0.ToString());

                        //获取支付类型  绿色直供包含5种
                        var PayTypeList = new List<ShopPaymentType>();

                        if (isGreenSupplied)
                        {
                            PayTypeList.Add(new ShopPaymentType() { TypeId = 1, TypeName = "微信在线支付", TypeFlag = 1 });
                            PayTypeList.Add(new ShopPaymentType() { TypeId = 2, TypeName = "支付宝在线支付", TypeFlag = 2 });
                        }

                        PayTypeList.Add(new ShopPaymentType() { TypeId = 3, TypeName = "货到现金付款", TypeFlag = 3 });
                        PayTypeList.Add(new ShopPaymentType() { TypeId = 4, TypeName = "货到微信付款", TypeFlag = 1 });
                        PayTypeList.Add(new ShopPaymentType() { TypeId = 5, TypeName = "货到支付宝付款", TypeFlag = 2 });

                        var List = PayTypeList.Select(t => new { TypeId = t.TypeId, TypeName = t.TypeName, TypeFlag = t.TypeFlag, IsCheck = payTypeIds.Contains(t.TypeId) });
                        resultModel.result = List;
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
        /// 设置商家支付种类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetShopPayType(Property.UI.Models.Mobile.ShopPayTypeModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);

                    //设置商家支付
                    if (shop != null)
                    {
                        // 新建用户角色关联表
                        List<T_ShopPaymentManagement> payTypes = new List<T_ShopPaymentManagement>();

                        if (!string.IsNullOrEmpty(model.PayTypeIds))
                        {
                            foreach (var id in model.PayTypeIds.Split(';'))
                            {
                                T_ShopPaymentManagement item = new T_ShopPaymentManagement() { ShopId = model.ShopId, PayTypeId = int.Parse(id) };
                                payTypes.Add(item);
                            }

                            shopBll.SetupPayTypes(shop, payTypes);
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
        /// <summary>
        /// 获取商家运费
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopShippingCost([FromUri]ShopInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopShippingCostBLL shippingCostBLL = BLLFactory<IShopShippingCostBLL>.GetBLL("ShopShippingCostBLL");
                    var shippingCost = shippingCostBLL.GetEntity(s => s.ShopId == model.ShopId);
                    //如果商家信息不为空
                    if (shippingCost != null)
                    {
                        resultModel.result = new
                        {
                            Id = shippingCost.Id,
                            OrderExpense = shippingCost.OrderExpense.HasValue ? shippingCost.OrderExpense.ToString() : "",
                            Price = shippingCost.Price,
                            IsFree = shippingCost.IsFree
                        };
                    }
                    else
                    {
                        resultModel.result = new
                        {
                            Id = "",
                            OrderExpense = "",
                            Price = "",
                            IsFree = ""
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
        /// 设置商家运费
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetShopShippingCost(Property.UI.Models.Mobile.ShopShippingCostModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    IShopShippingCostBLL shippingCostBLL = BLLFactory<IShopShippingCostBLL>.GetBLL("ShopShippingCostBLL");

                    //如果存在更新，否则添加新的
                    if (model.Id.HasValue)
                    {
                        var shippingCost = shippingCostBLL.GetEntity(s => s.Id == model.Id);

                        shippingCost.OrderExpense = model.OrderExpense;
                        shippingCost.Price = model.Price;
                        shippingCost.IsFree = model.IsFree;

                        shippingCostBLL.Update(shippingCost);
                    }
                    else
                    {
                        T_ShopShippingCost shippingCost = new T_ShopShippingCost();

                        shippingCost.ShopId = model.ShopId;
                        shippingCost.OrderExpense = model.OrderExpense;
                        shippingCost.Price = model.Price;
                        shippingCost.IsFree = model.IsFree;

                        shippingCostBLL.Save(shippingCost);
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
        /// 上传商家封面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel UploadShopImage(ShopImageModel model)
        {
            //string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "YST.txt";
            //StreamWriter sw = new StreamWriter(filePath, true);
            //sw.Write("测试开始。。。");
            //sw.Close();
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取要设置基本信息的物业用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    userBll.Update(user);

                    //调用商家BLL层获取商家信息
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.ShopId);

                    if (shop != null)
                    {
                        //设定头像路径及文件名
                        string dir = HttpContext.Current.Server.MapPath(ConstantParam.SHOP_IMG_DIR);

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        //设置商家封面
                        if (!string.IsNullOrEmpty(model.ShopImage))
                        {
                            var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                            string filepath = Path.Combine(dir, fileName);

                            using (FileStream fs = new FileStream(filepath, FileMode.Create))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    byte[] datas = Convert.FromBase64String(model.ShopImage);
                                    bw.Write(datas);
                                    bw.Close();
                                }
                            }

                            //封面路径保存
                            shop.ImgPath = PropertyUtils.UnZip(filepath, dir, ConstantParam.SHOP_IMG_DIR);

                            StringBuilder imgsSB = new StringBuilder();
                            //生成缩略图保存
                            foreach (var path in shop.ImgPath.Split(';'))
                            {
                                string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
                                string thumpPath = Path.Combine(HttpContext.Current.Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR), thumpFile);
                                PropertyUtils.getThumImage(Path.Combine(HttpContext.Current.Server.MapPath(path)), 18, 3, thumpPath);
                                imgsSB.Append(ConstantParam.SHOP_THUM_IMG_DIR + thumpFile + ";");
                            }

                            shop.ImgThumbnail = imgsSB.ToString();
                            shop.ImgThumbnail = shop.ImgThumbnail.Substring(0, shop.ImgThumbnail.Length - 1);
                        }

                        shopBll.Update(shop);
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
        /// 获取商家用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopUserInfo([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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
                    userBll.Update(user);

                    var shop = user.Shops.FirstOrDefault();
                    //如果商家信息不为空
                    if (shop != null)
                    {
                        resultModel.result = new
                        {
                            ShopUserName = user.UserName,
                            ShopUserImage = user.HeadPath,
                            ShopName = shop.ShopName,
                            TrueName = user.TrueName,
                            UserGender = user.Gender.HasValue ? user.Gender.Value == 1 ? "男" : "女" : "",
                            PhoneNumber = user.Phone,
                            Email = user.Email
                        };
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.SHOP_NOEXIST;
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
        /// 设置商家用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetShopUserInfo(ShopUserInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取要设置基本信息的物业用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    if (!string.IsNullOrEmpty(model.Gender))
                    {
                        user.Gender = int.Parse(model.Gender);
                    }

                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.TrueName = model.TrueName;
                    //设定头像路径及文件名
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.ShOPFORM_USER_HEAD_DIR);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //设置商家用户头像
                    if (!string.IsNullOrEmpty(model.UserImg))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.UserImg);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        user.HeadPath = ConstantParam.ShOPFORM_USER_HEAD_DIR + fileName;
                    }
                    userBll.Update(user);
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