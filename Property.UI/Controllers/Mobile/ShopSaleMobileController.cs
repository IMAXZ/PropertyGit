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
using System.Text;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    public class ShopSaleMobileController : ApiController
    {
        /// <summary>
        /// 获取商家的商品分类列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetGoodsCategoryList([FromUri]GoodsCategorySearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主存在
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

                    IGoodsCategoryBLL goodsBLL = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
                    Expression<Func<T_GoodsCategory, bool>> where = g => g.ShopId == model.ShopId;
                    // 获取商家的商品分类
                    var list = goodsBLL.GetPageList(where, model.PageIndex).Select(
                        g => new
                        {
                            Id = g.Id,
                            Name = g.Name,
                            Count = g.ShopSales.Count()
                        }).ToList();

                    resultModel.result = list;
                    resultModel.Total = goodsBLL.Count(g => g.ShopId == model.ShopId);
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
        /// 添加商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddGoodsCategory(GoodsCategoryInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    IGoodsCategoryBLL goodsBLL = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");

                    if (goodsBLL.Exist(m => m.Name == model.Name && m.ShopId == model.ShopId))
                    {
                        resultModel.Msg = "该商品类别已经存在";
                    }
                    else
                    {
                        //商品分类实例化
                        T_GoodsCategory goods = new T_GoodsCategory()
                        {
                            Name = model.Name,
                            ShopId = model.ShopId
                        };
                        //保存商品分类

                        goodsBLL.Save(goods);
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
        /// 编辑商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel EditGoodsCategory(GoodsCategoryInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IGoodsCategoryBLL goodsBLL = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");

                        if (goodsBLL.Exist(m => m.Name == model.Name && m.Id != model.Id && m.ShopId == model.ShopId))
                        {
                            resultModel.Msg = "该商品类别已经存在";
                        }
                        else
                        {
                            var goodsCategory = goodsBLL.GetEntity(g => g.Id == model.Id.Value);
                            //修改商品分类
                            if (goodsCategory != null)
                            {
                                goodsCategory.Name = model.Name;
                                goodsBLL.Update(goodsCategory);
                            }
                            else
                            {
                                resultModel.Msg = "不存在当前商品分类";
                            }
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
        /// 删除商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DelGoodsCategory(GoodsCategoryInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IGoodsCategoryBLL goodsBLL = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
                        var goodsCategory = goodsBLL.GetEntity(g => g.Id == model.Id.Value);

                        if (goodsCategory == null)
                        {
                            resultModel.Msg = "该商品分类不存在";
                        }
                        else if (goodsCategory.ShopSales.Count() > 0)
                        {
                            resultModel.Msg = "已有该商品分类的商品，无法删除";
                        }
                        else
                        {
                            goodsBLL.Delete(goodsCategory);
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
        /// 获取商家商品列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetGoodsList([FromUri]GoodsSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //获取当前用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主存在
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

                    Expression<Func<T_ShopSale, bool>> where = s => s.InSales == model.InSales && s.GoodsCategory.ShopId == model.ShopId;

                    if (model.GoodsCategoryId > 0)
                    {
                        where = PredicateBuilder.And(where, s => s.GoodsCategoryId == model.GoodsCategoryId);
                    }

                    IOrderBLL orderBll = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");

                    IShopSaleBLL shopSaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    var list = shopSaleBLL.GetPageList(where, "CreateTime", false, model.PageIndex).Select(
                        s => new
                        {
                            Id = s.Id,
                            ImgThumbnail = string.IsNullOrEmpty(s.ImgThumbnail) ? "" : s.ImgThumbnail.Split(';').FirstOrDefault(),
                            Title = s.Title,
                            Price = s.Price,
                            RemainingAmout = s.RemainingAmout,
                            SaledCount = s.OrderDetails.Where(od => od.Order.OrderStatus == ConstantParam.OrderStatus_FINISH).Select(od => od.SaledAmount).ToArray().Sum(),
                            CreateDate = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            UnShelveTime = s.UnShelveTime.HasValue ? s.UnShelveTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            InSales = s.InSales
                        });

                    resultModel.result = list;
                    resultModel.Total = shopSaleBLL.Count(where);
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
        /// 获取商品信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetGoods([FromUri]GoodsInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IShopSaleBLL shopSaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                        var goods = shopSaleBll.GetEntity(g => g.Id == model.Id.Value);
                        //商品详情
                        if (goods != null)
                        {
                            resultModel.result = new
                            {
                                Name = goods.Title,
                                Content = goods.Content,
                                Price = goods.Price,
                                RemaintAmount = goods.RemainingAmout,
                                GoodsCategoryId = goods.GoodsCategoryId,
                                GoodsCategoryName = goods.GoodsCategory.Name,
                                ImgPath = goods.ImgPath,
                                ImgThumbnail = goods.ImgThumbnail
                            };
                        }
                        else
                        {
                            resultModel.Msg = "不存在当前商品";
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
        /// 添加商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddGoods(GoodsInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    //商品分类实例化
                    T_ShopSale goods = new T_ShopSale()
                    {
                        Title = model.Name,
                        Content = model.Content,
                        CreateTime = DateTime.Now,
                        GoodsCategoryId = model.GoodCategoryId,
                        Price = model.Price,
                        RemainingAmout = model.RemainingAmount,
                        InSales = 1
                    };

                    //话题文件资源保存目录
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.SHOP_Sales);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    //图片上传
                    if (!string.IsNullOrEmpty(model.PicList))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.PicList);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //图片集路径保存
                        goods.ImgPath = PropertyUtils.UnZip(filepath, dir, ConstantParam.SHOP_Sales);

                        StringBuilder imgsSB = new StringBuilder();
                        //生成缩略图保存
                        foreach (var path in goods.ImgPath.Split(';'))
                        {

                            string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
                            string thumpPath = Path.Combine(HttpContext.Current.Server.MapPath(ConstantParam.SHOP_Sales_ThumIMG), thumpFile);
                            PropertyUtils.getThumImage(Path.Combine(HttpContext.Current.Server.MapPath(path)), 18, 3, thumpPath);
                            imgsSB.Append(ConstantParam.SHOP_Sales_ThumIMG + "/" + thumpFile + ";");
                        }

                        goods.ImgThumbnail = imgsSB.ToString();
                        goods.ImgThumbnail = goods.ImgThumbnail.Substring(0, goods.ImgThumbnail.Length - 1);

                    }

                    //保存商品
                    IShopSaleBLL goodsBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    goodsBLL.Save(goods);

                    //绿色直供推送
                    if (model.IsPush == 1)
                    {
                        IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                        string shopName = shopBLL.GetEntity(s => s.Id == model.ShopId).ShopName;

                        //推送给业主客户端
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var registrationIds = userPushBLL.GetList(p => !string.IsNullOrEmpty(p.RegistrationId)).Select(p => p.RegistrationId).ToArray();

                        string alert = shopName + "的商品" + goods.Title + "上架了";
                        bool flag = PropertyUtils.SendPush("商品上架", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);
                        if (!flag)
                        {
                            resultModel.Msg = "推送发生异常";
                        }
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch(Exception ex)
            {
                PropertyUtils.WriteLogInfo("test");
                PropertyUtils.WriteLogError(ex);
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }
        /// <summary>
        /// 编辑商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel EditGoods(GoodsInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IShopSaleBLL shopSaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                        var goods = shopSaleBll.GetEntity(g => g.Id == model.Id.Value);

                        //修改商品分类
                        if (goods != null)
                        {
                            //基础信息
                            goods.Title = model.Name;
                            goods.Content = model.Content;
                            goods.Price = model.Price;
                            goods.RemainingAmout = model.RemainingAmount;
                            goods.GoodsCategoryId = model.GoodCategoryId;

                            //图片和压缩图
                            var sourceImgList = goods.ImgPath == null ? null : goods.ImgPath.Split(';').ToList();
                            var sourceImgThumbnailArray = goods.ImgThumbnail == null ? null : goods.ImgThumbnail.Split(';').ToArray();

                            var remainingImgList = goods.ImgPath == null ? new List<string>() : goods.ImgPath.Split(';').ToList();
                            var remainingImgThumbnailList = goods.ImgThumbnail == null ? new List<string>() : goods.ImgThumbnail.Split(';').ToList();


                            //要删除的缩略图列表
                            var delImgThumbnailList = new List<string>();

                            if (sourceImgList != null)
                            {
                                //对要删除的图片列表进行删除
                                if (!string.IsNullOrWhiteSpace(model.delPicList))
                                {
                                    foreach (var path in model.delPicList.Split(';'))
                                    {
                                        int index = sourceImgList.FindIndex(m => m == path);

                                        if (index < sourceImgThumbnailArray.Count())
                                        {
                                            delImgThumbnailList.Add(sourceImgThumbnailArray[index]);
                                        }

                                        remainingImgList.Remove(path);
                                        DelFile(path);
                                    }
                                }
                            }


                            foreach (var path in delImgThumbnailList)
                            {
                                remainingImgThumbnailList.Remove(path);
                                DelFile(path);
                            }

                            var strRemainedImg = "";

                            foreach (var item in remainingImgList)
                            {
                                if (!string.IsNullOrEmpty(item.Trim()))
                                {
                                    strRemainedImg = strRemainedImg + item + ";";
                                }
                            }

                            var strReminedThumbnailImg = "";

                            foreach (var item in remainingImgThumbnailList)
                            {
                                if (!string.IsNullOrEmpty(item.Trim()))
                                {
                                    strReminedThumbnailImg = strReminedThumbnailImg + item + ";";
                                }
                            }

                            //图片上传
                            if (!string.IsNullOrEmpty(model.PicList))
                            {
                                //文件资源保存目录
                                string dir = HttpContext.Current.Server.MapPath(ConstantParam.SHOP_Sales);

                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }

                                var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                                string filepath = Path.Combine(dir, fileName);

                                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                                {
                                    using (BinaryWriter bw = new BinaryWriter(fs))
                                    {
                                        byte[] datas = Convert.FromBase64String(model.PicList);
                                        bw.Write(datas);
                                        bw.Close();
                                    }
                                }

                                var imgZipParth = PropertyUtils.UnZip(filepath, dir, ConstantParam.SHOP_Sales);
                                //图片集路径保存
                                goods.ImgPath = strRemainedImg + imgZipParth;

                                StringBuilder imgsSB = new StringBuilder();
                                //生成缩略图保存
                                foreach (var path in imgZipParth.Split(';'))
                                {
                                    string thumpFile = DateTime.Now.ToFileTime().ToString() + ".jpg";
                                    string thumpPath = Path.Combine(HttpContext.Current.Server.MapPath(ConstantParam.SHOP_Sales_ThumIMG), thumpFile);
                                    PropertyUtils.getThumImage(Path.Combine(HttpContext.Current.Server.MapPath(path)), 18, 3, thumpPath);

                                    imgsSB.Append(ConstantParam.SHOP_Sales_ThumIMG + "/" + thumpFile + ";");
                                }

                                goods.ImgThumbnail = imgsSB.ToString();
                                goods.ImgThumbnail = strReminedThumbnailImg + goods.ImgThumbnail.Substring(0, goods.ImgThumbnail.Length - 1);
                            }
                            else
                            {
                                goods.ImgPath = strRemainedImg;
                                goods.ImgThumbnail = strReminedThumbnailImg;
                            }

                            shopSaleBll.Update(goods);
                        }
                        else
                        {
                            resultModel.Msg = "不存在当前商品";
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
        /// 删除商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DelGoods(GoodsInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IShopSaleBLL shopSaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                        var goods = shopSaleBll.GetEntity(g => g.Id == model.Id.Value);

                        if (goods == null)
                        {
                            resultModel.Msg = "该商品不存在";
                        }
                        else
                        {
                            if (goods.OrderDetails.Count > 0)
                            {
                                resultModel.Msg = "该商品已被订购,无法删除";
                            }
                            else
                            {
                                var imagePath = goods.ImgPath;
                                var imgThumbnail = goods.ImgThumbnail;

                                shopSaleBll.Delete(goods);

                                if (!string.IsNullOrWhiteSpace(imagePath))
                                {
                                    if (imagePath.Contains(";"))
                                    {
                                        foreach (var path in imagePath.Split(';'))
                                        {
                                            DelFile(path);
                                        }
                                    }
                                    else
                                    {
                                        DelFile(imagePath);
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(imgThumbnail))
                                {
                                    if (imgThumbnail.Contains(";"))
                                    {
                                        foreach (var path in imgThumbnail.Split(';'))
                                        {
                                            DelFile(path);
                                        }
                                    }
                                    else
                                    {
                                        DelFile(imgThumbnail);
                                    }
                                }
                            }
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
        /// 商品上/下架
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetOnSale(GoodsInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //获取当前商家用户
                IShopUserBLL userBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果商家用户存在
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

                    if (model.Id.HasValue)
                    {
                        IShopSaleBLL shopSaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                        var goods = shopSaleBll.GetEntity(g => g.Id == model.Id.Value);
                        //修改商品上/下架
                        if (goods != null)
                        {
                            goods.InSales = model.InSales;

                            if (goods.InSales == 0)
                            {
                                goods.UnShelveTime = DateTime.Now;
                            }
                            else
                            {
                                goods.CreateTime = DateTime.Now;
                            }

                            shopSaleBll.Update(goods);
                        }
                        else
                        {
                            resultModel.Msg = "不存在当前商品分类";
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
        /// 删除文件方法
        /// </summary>
        /// <param name="file"></param>
        private void DelFile(string file)
        {
            try
            {
                string filestr = HttpContext.Current.Server.MapPath(file);

                if (System.IO.File.Exists(filestr))
                {
                    System.IO.File.Delete(filestr);
                }
            }
            catch (Exception ex)
            {
                PropertyUtils.WriteLogError(ex);
            }
            finally
            {

            }
        }
    }
}