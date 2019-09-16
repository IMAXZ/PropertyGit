using Microsoft.Ajax.Utilities;
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
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 门店商品促销管理控制器
    /// </summary>
    public class ShopSalesController : ShopBaseController
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "商品列表")]
        public ActionResult SaleList(ShopSaleSearchModel model)
        {
            //如果门店已创建
            if (GetCurrentShopId() != null)
            {
                int ShopId = GetCurrentShopId().Value;
                //促销BLL
                IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                //条件
                Expression<Func<T_ShopSale, bool>> whereLambda = a => a.GoodsCategory.ShopId == ShopId;
                if (!string.IsNullOrEmpty(model.Kword))
                {
                    whereLambda = PredicateBuilder.And<T_ShopSale>(whereLambda, a => a.Title.Contains(model.Kword));
                }
                if (model.InSale != null)
                {
                    whereLambda = PredicateBuilder.And(whereLambda, u => u.InSales == model.InSale.Value);
                }
                if (model.GoodsCategoryId != null)
                {
                    whereLambda = PredicateBuilder.And<T_ShopSale>(whereLambda, a => a.GoodsCategoryId == model.GoodsCategoryId.Value);
                }
                var sortModel = SettingSorting("Id", false);
                //查询数据
                model.ResultList = SaleBLL.GetPageList(whereLambda, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_ShopSale>;
                model.GoodsCategoryList = GetGoodsCategoryList(ShopId);
                model.GoodsStateList = GetGoodsGoodsStateList(null);
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult RemoveShopSales(int id)
        {
            JsonModel Jm = new JsonModel();
            int ShopId = GetCurrentShopId().Value;//当前门店
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == id);
            if (ShopSale != null)
            {
                ShopSale.UnShelveTime = DateTime.Now;
                ShopSale.InSales = 0;
            }
            else
            {
                Jm.Msg = "该商品已经下架";
            }
            SaleBLL.Update(ShopSale);
            return Json(Jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 重新上架
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReAddShopSales(int id)
        {
            JsonModel Jm = new JsonModel();
            int ShopId = GetCurrentShopId().Value;//当前门店
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == id);
            if (ShopSale != null)
            {
                ShopSale.CreateTime = DateTime.Now;
                ShopSale.InSales = 1;
            }
            else
            {
                Jm.Msg = "该商品已经上架";
            }
            SaleBLL.Update(ShopSale);
            return Json(Jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 商品上架
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "商品上架")]
        public ActionResult AddShopSales()
        {
            ShopSaleModel model = new ShopSaleModel();
            int UserId = GetSessionModel().UserID;
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var Shop = ShopBll.GetEntity(s => s.ShopUserId == UserId);
            if (Shop != null)
            {
                //如果门店类型是绿色直供
                if (Shop.Type.Contains(ConstantParam.SHOP_TYPE_0.ToString()))
                {
                    ViewBag.IsHasPush = true;
                }
                else
                {
                    ViewBag.IsHasPush = false;
                }
                model.GoodsCategoryList = GetGoodsCategoryList(Shop.Id);
                return View(model);
            }
            else
            {
                return RedirectToAction("SaleList");
            }
        }
        /// <summary>
        /// 商品上架提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddShopSales(ShopSaleModel model)
        {
            JsonModel Jm = new JsonModel();
            if (ModelState.IsValid)
            {
                var currentShopId = GetCurrentShopId();
                //如果门店已创建
                if (currentShopId != null)
                {
                    T_ShopSale ShopSale = new T_ShopSale();
                    ShopSale.Title = model.Title;
                    ShopSale.Phone = model.Phone;
                    ShopSale.Content = model.Content;
                    ShopSale.GoodsCategoryId = model.GoodsCategoryId.Value;
                    ShopSale.RemainingAmout = model.RemainingAmout;
                    ShopSale.Price = model.Price;
                    ShopSale.CreateTime = DateTime.Now;
                    ShopSale.InSales = 1;
                    //促销BLL
                    IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    //保存
                    SaleBLL.Save(ShopSale);

                    //绿色直供推送
                    if (model.IsPush)
                    {
                        IShopBLL shopBLL = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                        string shopName = shopBLL.GetEntity(s => s.Id == currentShopId).ShopName;
                        
                        //推送给业主客户端
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var registrationIds = userPushBLL.GetList(p => !string.IsNullOrEmpty(p.RegistrationId)).Select(p => p.RegistrationId).ToArray();

                        string alert = shopName + "的商品" + model.Title + "上架了";
                        bool flag = PropertyUtils.SendPush("商品上架", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);
                        if (!flag)
                        {
                            Jm.Msg = "推送发生异常";
                        }
                    }
                    //记录 Log
                    Jm.Content = Property.Common.PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    Jm.Msg = "门店还未创建";
                }
            }
            else
            {
                Jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(Jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelShopSales(int id)
        {
            JsonModel Jm = new JsonModel();
            try
            {
                //促销BLL
                IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == id);
                //图片集合
                List<string> Path = new List<string>();
                //缩略图集合
                List<string> ThumPath = new List<string>();
                if (!string.IsNullOrEmpty(ShopSale.ImgPath) && !string.IsNullOrEmpty(ShopSale.ImgThumbnail))
                {
                    Path = ShopSale.ImgPath.Split(new char[] { ';' }).ToList();
                    ThumPath = ShopSale.ImgThumbnail.Split(new char[] { ';' }).ToList();
                }
                //删除文件
                for (int i = 0; i < ThumPath.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Path[i].Trim()))
                    {
                        //删除缩略图
                        DelFile(ThumPath[i]);
                        //删除图片
                        //DelFile(Path[i]);
                    }
                }
                if (ShopSale.OrderDetails.Count > 0)
                {
                    Jm.Msg = "该商品已被订购,无法删除";
                }
                else
                {
                    //执行删除
                    SaleBLL.Delete(ShopSale);
                }
            }
            catch
            {
                Jm.Msg = "删除失败";
            }
            return Json(Jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除文件方法
        /// </summary>
        /// <param name="file"></param>
        public void DelFile(string file)
        {
            string filestr = Server.MapPath(file);
            if (System.IO.File.Exists(filestr))
            {
                System.IO.File.Delete(filestr);
            }
        }

        /// <summary>
        /// 编辑商品信息
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "编辑商品信息")]
        public ActionResult EditShopSales(int id)
        {
            //初始化模型
            ShopSaleModel model = new ShopSaleModel();
            //促销BLL
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            var lst = SaleBLL.GetEntity(a => a.Id == id);
            if (lst != null)
            {
                model.Id = lst.Id;
                model.Title = lst.Title;
                model.Content = lst.Content;
                model.Phone = lst.Phone;
                model.RemainingAmout = lst.RemainingAmout;
                model.Price = lst.Price;
                model.GoodsCategoryId = lst.GoodsCategoryId;
                model.GoodsCategoryList = GetGoodsCategoryList(lst.GoodsCategory.ShopId);
                return View(model);
            }
            else
            {
                return RedirectToAction("SaleList");
            }
        }

        /// <summary>
        /// 编辑商品信息提交
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditShopSales(ShopSaleModel Model)
        {
            JsonModel Jm = new JsonModel();
            if (ModelState.IsValid)
            {
                //促销BLL
                IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == Model.Id);
                if (ShopSale != null)
                {
                    ShopSale.Title = Model.Title;
                    ShopSale.Phone = Model.Phone;
                    ShopSale.Content = Model.Content;
                    ShopSale.GoodsCategoryId = Model.GoodsCategoryId.Value;
                    ShopSale.RemainingAmout = Model.RemainingAmout;
                    ShopSale.Price = Model.Price;
                    //保存修改
                    SaleBLL.Update(ShopSale);
                }
                else
                {
                    Jm.Msg = "该商品不存在";
                }
                //记录日志
                Jm.Content = Property.Common.PropertyUtils.ModelToJsonString(Model);
            }
            else
            {
                Jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(Jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看商品信息
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "查看商品详细")]
        [HttpGet]
        public ActionResult ShopSalesDetail(int id = 0)
        {
            //促销BLL
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            var lst = SaleBLL.GetEntity(a => a.Id == id);
            if (lst != null)
            {
                return View(lst);
            }
            else
            {
                return RedirectToAction("SaleList");
            }
        }

        /// <summary>
        /// 验证标题是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="Title">标题</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ShopSalesValidate(int id, string Title)
        {
            bool result = true;
            //促销BLL
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            result = (SaleBLL.GetEntity(a => a.Id != id && a.Title.Equals(Title)) == null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传商品图片
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "商品图片管理")]
        [HttpGet]
        public ActionResult Upload(int id)
        {
            //门店BLL
            IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            T_ShopSale Model = SaleBLL.GetEntity(a => a.Id == id);
            ShopSaleModel List = new ShopSaleModel();
            List.Id = Model.Id;
            if (Model.ImgPath != null && Model.ImgThumbnail != null)
            {
                List<string> pathstr = Model.ImgPath.Split(new char[] { ';' }).ToList();
                List<string> ThumPath = Model.ImgThumbnail.Split(new char[] { ';' }).ToList();
                List.PathList = pathstr;
                List.ThumPathList = ThumPath;
            }
            return View(List);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Upload(ShopSaleModel Model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //促销BLL
                IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == Model.Id);
                //图片路径
                string PathList = string.IsNullOrEmpty(ShopSale.ImgPath) ? "" : ShopSale.ImgPath + ";";
                //缩略图路径
                string ThumPathList = string.IsNullOrEmpty(ShopSale.ImgThumbnail) ? "" : ShopSale.ImgThumbnail + ";";
                int currentImgCount = PathList.Split(';').Count() - 1;
                if (Request.Files.Count + currentImgCount > 6)
                {
                    jm.Msg = "最多只能上传6张商品图片";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    if (file != null)
                    {
                        var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
                        //判断图片路径是否存在
                        if (!System.IO.Directory.Exists(Server.MapPath(ConstantParam.SHOP_Sales)))
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(ConstantParam.SHOP_Sales));
                        }
                        //判断缩略图路径是否存在
                        if (!System.IO.Directory.Exists(Server.MapPath(ConstantParam.SHOP_Sales_ThumIMG)))
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(ConstantParam.SHOP_Sales_ThumIMG));
                        }
                        //保存图片
                        var path = Path.Combine(Server.MapPath(ConstantParam.SHOP_Sales), fileName);
                        file.SaveAs(path);
                        //生成缩略图
                        string thumpFile = DateTime.Now.ToFileTime() + ".jpg";

                        var thumpPath = Path.Combine(Server.MapPath(ConstantParam.SHOP_Sales_ThumIMG), thumpFile);
                        PropertyUtils.getThumImage(path, 18, 3, thumpPath);
                        PathList += ConstantParam.SHOP_Sales + "/" + fileName + ";";
                        ThumPathList += ConstantParam.SHOP_Sales_ThumIMG + "/" + thumpFile + ";";
                    }
                }
                ShopSale.ImgPath = PathList.Substring(0, PathList.Length - 1);
                ShopSale.ImgThumbnail = ThumPathList.Substring(0, ThumPathList.Length - 1); ;

                //保存
                SaleBLL.Update(ShopSale);
                jm.Content = "商品：" + Model.Title + "上传图片";
            }
            catch (Exception e)
            {
                jm.Msg = e.Message;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelFile(int id, string href, string thum)
        {
            JsonModel Jm = new JsonModel();
            try
            {
                //促销BLL
                IShopSaleBLL SaleBLL = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                T_ShopSale ShopSale = SaleBLL.GetEntity(a => a.Id == id);

                //删除缩略图路径
                List<string> ImgThumbnailList = ShopSale.ImgThumbnail.Split(';').ToList();
                ImgThumbnailList.Remove(href);

                string newImgThumbnails = "";
                foreach (var item in ImgThumbnailList)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        newImgThumbnails = newImgThumbnails + item + ";";
                    }
                }
                if (!string.IsNullOrEmpty(newImgThumbnails)) 
                {
                    newImgThumbnails = newImgThumbnails.Substring(0, newImgThumbnails.Length - 1);
                }
                ShopSale.ImgThumbnail = newImgThumbnails;
                //获取缩略图片文件
                string path = Server.MapPath(href);
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch { }
                }

                //删除大图路径
                List<string> ImgPathList = ShopSale.ImgPath.Split(';').ToList();
                ImgPathList.Remove(thum);

                string newImgPaths = "";
                foreach (var item in ImgPathList)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        newImgPaths = newImgPaths + item + ";";
                    }
                }
                if (!string.IsNullOrEmpty(newImgPaths))
                {
                    newImgPaths = newImgPaths.Substring(0, newImgPaths.Length - 1);
                }
                ShopSale.ImgPath = newImgPaths;
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
                SaleBLL.Update(ShopSale);
                //记录log
                Jm.Content = "删除商品图片" + href;
            }
            catch
            {
                Jm.Msg = "删除出现异常";
            }
            return Json(Jm);
        }
        /// <summary>
        /// 获取商品状态列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetGoodsGoodsStateList(int ? Sale=0)
        {
            List<SelectListItem> GoodsState = new List<SelectListItem>();
            GoodsState.Add(new SelectListItem()
            {
                Text = "已下架",
                Value = ConstantParam.SHOPSALED.ToString(),
                Selected = Sale == ConstantParam.SHOPSALED
            });
            GoodsState.Add(new SelectListItem()
            {
                Text = "出售中",
                Value = ConstantParam.SHOPSALING.ToString(),
                Selected = Sale == ConstantParam.SHOPSALING
            });
            return GoodsState;
        }

        /// <summary>
        /// 获取当前门店商品分类列表
        /// </summary>
        private List<SelectListItem> GetGoodsCategoryList(int ShopId)
        {
            IGoodsCategoryBLL goodsCategoryBLL = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = goodsCategoryBLL.GetList(u => u.ShopId == ShopId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }
    }
}
