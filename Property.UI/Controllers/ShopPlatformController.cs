using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Filter;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 门店平台首页控制器
    /// </summary>
    public class ShopPlatformController : ShopBaseController
    {
        /// <summary>
        /// 门店平台首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "门店平台首页")]
        public ActionResult Index()
        {
            //获取登录用户的门店
            int UserId = GetSessionModel().UserID;
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var Shop = ShopBll.GetEntity(s => s.ShopUserId == UserId);
            if (Shop != null)
            {
                var placeNameList = Shop.ShopPlaces.Select(p => p.PropertyPlace.Name).ToList();
                var placeNames = "";
                if (placeNameList != null && placeNameList.Count > 0)
                {
                    for (int i = 0; i < placeNameList.Count; i++)
                    {
                        placeNames += placeNameList[i] + ",";
                    }
                    placeNames = placeNames.Substring(0, placeNames.Length - 1);
                }
                ViewBag.PlaceNames = placeNames;
                return View(Shop);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 编辑门店
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑商家信息")]
        [HttpGet]
        public ActionResult EditShop(int id)
        {
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shopInfo = ShopBll.GetEntity(index => index.Id == id);

            if (shopInfo != null)
            {
                ShopPlatformModel model = new ShopPlatformModel();
                model.Id = shopInfo.Id;
                model.ShopName = shopInfo.ShopName;
                model.MainSale = shopInfo.MainSale;
                model.Tel = shopInfo.Phone;
                model.ProvinceId = shopInfo.ProvinceId;
                model.ProvinceList = GetProvinceList();
                model.CityId = shopInfo.CityId;
                model.CityList = base.GetCityList(shopInfo.ProvinceId);
                model.CountyId = shopInfo.CountyId;
                model.CountyList = base.GetCountyList(shopInfo.CityId);
                model.Address = shopInfo.Address;
                model.Content = shopInfo.Content;
                model.StartBusinessTime = shopInfo.StartBusinessTime;
                model.EndBusinessTime = shopInfo.EndBusinessTime;
                model.TimeList = GetBusinessTimeList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 编辑商家信息提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShop(ShopPlatformModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                T_Shop shop = ShopBll.GetEntity(m => m.Id == model.Id);
                if (shop != null)
                {
                    shop.ShopName = model.ShopName;
                    shop.Phone = model.Tel;
                    shop.Address = model.Address;
                    shop.MainSale = model.MainSale;
                    shop.Content = model.Content;
                    shop.StartBusinessTime = model.StartBusinessTime;
                    shop.EndBusinessTime = model.EndBusinessTime;
                    shop.UpdateTime = DateTime.Now;

                    //修改保存到数据库
                    if (ShopBll.Update(shop))
                    {
                        //日志记录
                        jm.Content = PropertyUtils.ModelToJsonString(model);
                    }
                    else
                    {
                        jm.Msg = "编辑失败";
                    }
                }
                else
                {
                    jm.Msg = "该门店不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取营业时间列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetBusinessTimeList()
        {
            // 开始列表
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i <= 24; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = i + " : 00",
                    Value = i.ToString(),
                    Selected = false
                });
            }
            return list;
        }

        /// <summary>
        /// 设置封面图片
        /// </summary>
        [BreadCrumb(Label = "设置店铺封面")]
        [HttpGet]
        public ActionResult SetCoverImg(int id) 
        {
            //门店BLL
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shop = shopBll.GetEntity(index => index.Id == id);
            if (shop != null)
            {
                CoverImgUploadModel model = new CoverImgUploadModel();
                model.ShopId = id;
                model.ImgPath = shop.ImgPath;
                return View(model);
            }
            else 
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// 设置封面图片提交
        /// </summary>
        /// <param name="model">封面图片上传模型</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetCoverImg(CoverImgUploadModel model)
        {
            JsonModel jm = new JsonModel();

            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shop = shopBll.GetEntity(index => index.Id == model.ShopId);
            try
            {
                //存入文件的路径
                string directory = Server.MapPath(ConstantParam.SHOP_IMG_DIR);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                //获取上传文件的扩展名
                string fileEx = Path.GetExtension(Path.GetFileName(model.CoverImgFile.FileName));
                //保存数据文件
                string fileName = DateTime.Now.ToFileTime().ToString() + fileEx;
                string savePath = Path.Combine(directory, fileName);
                model.CoverImgFile.SaveAs(savePath);
                shop.ImgPath = ConstantParam.SHOP_IMG_DIR + fileName;

                //判断缩略图路径是否存在
                if (!Directory.Exists(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR)))
                {
                    Directory.CreateDirectory(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR));
                }
                //生成缩略图
                string thumpFile = DateTime.Now.ToFileTime() + ".jpg";

                var thumpPath = Path.Combine(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR), thumpFile);
                if (PropertyUtils.getThumImage(savePath, 18, 3, thumpPath))
                {
                    shop.ImgThumbnail = ConstantParam.SHOP_THUM_IMG_DIR + thumpFile;
                }
                shopBll.Update(shop);
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 门店图片上传
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "门店图片上传")]
        [HttpGet]
        public ActionResult UploadPic(int id)
        {
            //门店BLL
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shopInfo = shopBll.GetEntity(index => index.Id == id);

            ShopImgModel shopImgModel = new ShopImgModel();
            shopImgModel.Id = id;
            if (shopInfo.ImgPath != null && shopInfo.ImgThumbnail != null)
            {
                List<string> imgPaths = shopInfo.ImgPath.Split(new char[] { ';' }).ToList();
                List<string> thumbImgPaths = shopInfo.ImgThumbnail.Split(new char[] { ';' }).ToList();
                shopImgModel.ImgPathArray = imgPaths;
                shopImgModel.ImgThumbPathArray = thumbImgPaths;
            }
            return View(shopImgModel);
        }


        /// <summary>
        /// 门店图片上传
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPic(ShopImgModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //门店BLL
                IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                var shopInfo = shopBll.GetEntity(index => index.Id == model.Id);

                //图片路径
                string PathList = shopInfo.ImgPath;
                //缩略图路径
                string ThumPathList = shopInfo.ImgThumbnail;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    if (file != null)
                    {
                        var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
                        //判断图片路径是否存在
                        if (!Directory.Exists(Server.MapPath(ConstantParam.SHOP_IMG_DIR)))
                        {
                            Directory.CreateDirectory(Server.MapPath(ConstantParam.SHOP_IMG_DIR));
                        }
                        //判断缩略图路径是否存在
                        if (!Directory.Exists(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR)))
                        {
                            Directory.CreateDirectory(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR));
                        }
                        //保存图片
                        var path = Path.Combine(Server.MapPath(ConstantParam.SHOP_IMG_DIR), fileName);
                        file.SaveAs(path);
                        //生成缩略图
                        string thumpFile = DateTime.Now.ToFileTime() + ".jpg";

                        var thumpPath = Path.Combine(Server.MapPath(ConstantParam.SHOP_THUM_IMG_DIR), thumpFile);
                        PropertyUtils.getThumImage(path, 18, 3, thumpPath);
                        PathList += ConstantParam.SHOP_IMG_DIR + fileName + ";";
                        ThumPathList += ConstantParam.SHOP_THUM_IMG_DIR + thumpFile + ";";
                    }
                }
                // 更新图片
                shopInfo.ImgPath = PathList;
                shopInfo.ImgThumbnail = ThumPathList;
                shopInfo.UpdateTime = DateTime.Now;
                // 保存到数据库
                shopBll.Update(shopInfo);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            catch (Exception e)
            {
                jm.Msg = e.Message;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除门店图片
        /// </summary>
        /// <param name="id">门店id</param>
        /// <param name="href">缩略图相对路径</param>
        /// <param name="thum">大图相对路径</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelShopImg(int id, string href, string thum)
        {
            JsonModel Jm = new JsonModel();
            try
            {
                IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                T_Shop Shop = shopBLL.GetEntity(a => a.Id == id);
                //删除缩略图路径
                int start = Shop.ImgThumbnail.IndexOf(href + ";");
                Shop.ImgThumbnail = Shop.ImgThumbnail.Remove(start, href.Length + 1);
                //获取缩略图片路径
                string path = Server.MapPath(href);
                //删除缩略图片
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                //获取大图片路径
                int thumStart = Shop.ImgPath.IndexOf(thum + ";");
                Shop.ImgPath = Shop.ImgPath.Remove(thumStart, thum.Length + 1);
                string thumPath = Server.MapPath(thum);
                if (System.IO.File.Exists(thumPath))
                {
                    try
                    {
                        //删除原图片
                        System.IO.File.Delete(thumPath);
                    }
                    catch { }
                }
                shopBLL.Update(Shop);
                //记录log
                Jm.Content = "删除门店图片" + href;
            }
            catch
            {
                Jm.Msg = "删除出现异常";
            }
            return Json(Jm);
        }

        /// <summary>
        /// 设置商家支付类型
        /// </summary> 
        /// <returns></returns>
        [BreadCrumb(Label = "设置商家支付类型")]
        [HttpGet]
        public ActionResult SetupPayType(int id)
        {
            //根据商家ID，获取商家信息
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            T_Shop shop = shopBll.GetEntity(s => s.Id == id);

            //是否是绿色直供
            bool isGreenSupplied = shop.Type.Contains(Property.Common.ConstantParam.SHOP_TYPE_0.ToString());
            ShopPayTypeModel paytypeModel = new ShopPayTypeModel();

            //获取商家信息
            paytypeModel.Shop = shop;
            //获取支付类型  绿色直供包含5种
            var PayTypeList = new List<ShopPaymentType>();

            if (isGreenSupplied)
            {
                PayTypeList.Add(new ShopPaymentType() { TypeId = 1, TypeName = "微信在线支付" });
                PayTypeList.Add(new ShopPaymentType() { TypeId = 2, TypeName = "支付宝在线支付" });
            }

            PayTypeList.Add(new ShopPaymentType() { TypeId = 3, TypeName = "货到现金付款" });
            PayTypeList.Add(new ShopPaymentType() { TypeId = 4, TypeName = "货到微信付款" });
            PayTypeList.Add(new ShopPaymentType() { TypeId = 5, TypeName = "货到支付宝付款" });

            paytypeModel.PayTypeList = PayTypeList;
            //已经分配的支付类型
            paytypeModel.PayTypeIds = shop.ShopPaymentManagements.Select(s => s.PayTypeId).ToList();

            return View(paytypeModel);
        }

        /// <summary>
        /// 设置商家支付类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetupPayType(ShopPayTypeSetupModel model)
        {
            JsonModel jm = new JsonModel();
            //根据商家ID，获取商家信息
            IShopBLL shopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            T_Shop shop = shopBll.GetEntity(s => s.Id == model.shopId);

            // 新建用户角色关联表
            List<T_ShopPaymentManagement> payTypes = new List<T_ShopPaymentManagement>();

            if (model.ids != null)
            {
                foreach (var id in model.ids)
                {
                    T_ShopPaymentManagement item = new T_ShopPaymentManagement() { ShopId = model.shopId, PayTypeId = id };
                    payTypes.Add(item);
                }
            }

            //修改商家支付类型集合
            if (shopBll.SetupPayTypes(shop, payTypes))
            {
                jm.Content = "商家 " + shop.ShopName + " 设置支付类型";
            }
            else
            {
                jm.Msg = "设置支付类型失败";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置订单运费
        /// </summary> 
        /// <returns></returns>
        [BreadCrumb(Label = "设置订单运费")]
        [HttpGet]
        public ActionResult SetupShippingCost()
        {
            ShopShippingCostModel model = new ShopShippingCostModel();

            if (GetCurrentShopId().HasValue)
            {
                int shopId = GetCurrentShopId().Value;
                IShopShippingCostBLL shippingCostBLL = BLLFactory<IShopShippingCostBLL>.GetBLL("ShopShippingCostBLL");
                var shippingCost = shippingCostBLL.GetEntity(s => s.ShopId == shopId);

                if (shippingCost != null)
                {
                    model.OrderExpense = shippingCost.OrderExpense;
                    model.Price = shippingCost.Price;
                    model.Id = shippingCost.Id;
                    model.IsFree = shippingCost.IsFree == 1 ? true : false;
                }

                model.ShopId = shopId;

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 设置订单运费提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetupShippingCost(ShopShippingCostModel model)
        {
            JsonModel Jm = new JsonModel();

            if (ModelState.IsValid)
            {
                IShopShippingCostBLL shippingCostBLL = BLLFactory<IShopShippingCostBLL>.GetBLL("ShopShippingCostBLL");

                //如果存在更新，否则添加新的
                if (model.Id.HasValue)
                {
                    var shippingCost = shippingCostBLL.GetEntity(s => s.Id == model.Id);

                    shippingCost.OrderExpense = model.OrderExpense;
                    shippingCost.Price = model.Price;
                    shippingCost.IsFree = model.IsFree ? 1 : 0;

                    shippingCostBLL.Update(shippingCost);
                }
                else
                {
                    T_ShopShippingCost shippingCost = new T_ShopShippingCost();

                    shippingCost.ShopId = model.ShopId;
                    shippingCost.OrderExpense = model.OrderExpense;
                    shippingCost.Price = model.Price;
                    shippingCost.IsFree = model.IsFree ? 1 : 0;

                    shippingCostBLL.Save(shippingCost);
                }

                Jm.Content = Property.Common.PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                Jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }

            return Json(Jm, JsonRequestBehavior.AllowGet);
        }
    }
}
