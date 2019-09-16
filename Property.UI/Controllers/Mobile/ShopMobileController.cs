using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 门店相关接口
    /// </summary>
    public class ShopMobileController : ApiController
    {
        /// <summary>
        /// 门店列表（根据类型分页查询）
        /// </summary>
        /// <param name="model">查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel ShopList([FromUri]ShopSearchModel model)
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

                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    Expression<Func<T_Shop, bool>> where = u => u.Type.Contains(model.Type.ToString());
                    //如果是生活小卖店或五金店
                    if (model.Type == 2 || model.Type == 3)
                    {
                        var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);
                        where = PredicateBuilder.And(where, u => u.ShopPlaces.Count(p => placeList.Contains(p.PropertyPlaceId)) > 0);
                    }
                    resultModel.Total = shopBll.Count(where);
                    resultModel.result = shopBll.GetPageList(where, "Id", false, model.PageIndex).ToList().Select(s => new
                    {
                        Id = s.Id,
                        ShopName = s.ShopName,
                        Content = s.MainSale,
                        Phone = string.IsNullOrEmpty(s.Phone) ? "" : s.Phone,
                        Img = string.IsNullOrEmpty(s.ImgThumbnail) ? "" : s.ImgThumbnail.Split(';')[0]
                    });
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
        /// 门店详细页面接口
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel ShopDetail([FromUri]DetailSearchModel model)
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

                    resultModel.result = "MobilePage/StoreDetail?ShopId=" + model.Id;
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
        /// 二期门店详细页面接口
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel ShopDetail2([FromUri]DetailSearchModel model)
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

                    //获取要查看详细的门店实体对象
                    IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    T_Shop Shop = ShopBll.GetEntity(s => s.Id == model.Id);
                    if(Shop != null)
                    {
                        resultModel.result = new 
                        {
                            ShopImg = Shop.ImgPath,
                            ShopInfoPath = "MobilePage/StoreDetail2?ShopId=" + model.Id,
                        };
                    }else
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
        /// 商品类别列表
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetGoodsCategoryList([FromUri]DetailSearchModel model)
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

                    //该门店的商品类别
                    Expression<Func<T_GoodsCategory, bool>> where = s => s.ShopId == model.Id;
                    //获取指定门店的类别列表
                    IGoodsCategoryBLL categoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
                    resultModel.result = categoryBll.GetList(where).Select(s => new
                    {
                        CategoryId = s.Id,
                        CategoryName = s.Name
                    });
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
        /// 商品列表（类别）
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetSaleList([FromUri]GoodsSearchModel model)
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

                    //该门店销售中的商品
                    Expression<Func<T_ShopSale, bool>> where = s => s.GoodsCategory.ShopId == model.ShopId && s.InSales == 1;
                    //如果选择了商品分类
                    if (model.GoodsCategoryId > 0)
                    {
                        where = PredicateBuilder.And(where, s => s.GoodsCategoryId == model.GoodsCategoryId);
                    }
                    //获取指定门店指定类别的商品列表
                    IShopSaleBLL SaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    resultModel.Total = SaleBll.Count(where);
                    resultModel.result = SaleBll.GetPageList(where, "Id", false, model.PageIndex).ToList().Select(s => new
                    {
                        GoodsId = s.Id,
                        GoodsName = s.Title,
                        GoodsDesc = s.Content,
                        RemainingAmout = s.RemainingAmout,
                        SellAmout = s.OrderDetails.Where(od => od.Order.OrderStatus == ConstantParam.OrderStatus_FINISH).Select(od => od.SaledAmount).ToArray().Sum(),
                        GoodsCoverImg = string.IsNullOrEmpty(s.ImgThumbnail) ? "" : s.ImgThumbnail.Split(';')[0],
                        GoodsOtherImg = string.IsNullOrEmpty(s.ImgPath) ? "" : s.ImgPath,
                        Price = s.Price
                    });
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
        /// 获取门店运费
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopFreight([FromUri]ShopFreightSearchModel model)
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

                    //获取要查询的门店
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.Id);
                    if (shop != null) 
                    {
                        var cost = shop.ShopShippingCosts.FirstOrDefault();
                        if (cost != null && cost.IsFree == 0 && (cost.OrderExpense == null || model.TotalPrice < cost.OrderExpense.Value))
                        {
                            resultModel.result = cost.Price.Value;
                        }
                        else 
                        {
                            resultModel.result = 0;
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
        /// 获取支持的付款方式
        /// </summary>
        /// <param name="model">详细模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShopPayWay([FromUri]DetailSearchModel model)
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

                    //获取要查询的门店
                    IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                    var shop = shopBll.GetEntity(s => s.Id == model.Id);
                    if (shop != null)
                    {
                        var PayWays = shop.ShopPaymentManagements.Select(p => p.PayTypeId).ToList();
                        if (PayWays.Count > 0)
                        {
                            resultModel.result = PayWays;
                        }
                        else 
                        {
                            resultModel.result = new List<int>();
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
