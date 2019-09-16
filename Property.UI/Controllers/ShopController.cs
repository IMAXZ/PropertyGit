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
    /// 门店信息管理控制器
    /// </summary>
    public class ShopController : BaseController
    {
        /// <summary>
        /// 门店一览
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "门店一览")]
        [HttpGet]
        public ActionResult ShopList(ShopSearchModel model)
        {
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            Expression<Func<T_Shop, bool>> where = u => (string.IsNullOrEmpty(model.ShopName) ? true : u.ShopName.Contains(model.ShopName));
            if (model.Type != null)
            {
                where = PredicateBuilder.And<T_Shop>(where, u => u.Type.Contains(model.Type.Value.ToString()));
            }
            var sortModel = SettingSorting("Id", false);

            model.List = ShopBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_Shop>;
            model.TypeList = GetTypeList();
            return View(model);
        }

        /// <summary>
        /// 编辑门店
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑门店")]
        [HttpGet]
        public ActionResult EditShop(int id)
        {
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            var shopInfo = ShopBll.GetEntity(index => index.Id == id);

            if (shopInfo != null)
            {
                ShopModel model = new ShopModel();
                model.Id = shopInfo.Id;
                model.ShopUserName = shopInfo.ShopUser.UserName;
                model.Types = shopInfo.Type;
                model.TypeList = GetTypeList();
                model.ShopName = shopInfo.ShopName;
                model.MainSale = shopInfo.MainSale;
                model.Tel = shopInfo.Phone;
                model.Address = shopInfo.Address;
                model.ProvinceId = shopInfo.ProvinceId;
                model.ProvinceList = GetProvinceList();
                model.CityId = shopInfo.CityId;
                model.CityList = base.GetCityList(shopInfo.ProvinceId);
                model.CountyId = shopInfo.CountyId;
                model.CountyList = base.GetCountyList(shopInfo.CityId);
                model.Content = shopInfo.Content;
                model.StartBusinessTime = shopInfo.StartBusinessTime;
                model.EndBusinessTime = shopInfo.EndBusinessTime;
                model.TimeList = GetBusinessTimeList();
                //model.IsDelivery = shopInfo.IsDelivery == null ? false : (shopInfo.IsDelivery == 1 ? true : false);

                var placeList = shopInfo.ShopPlaces.Select(p => new
                {
                    PlaceId = p.PropertyPlaceId,
                    PlaceName = p.PropertyPlace.Name
                }).ToList();

                model.PlaceIds = "";
                model.placeNames = "";
                if (placeList != null && placeList.Count > 0)
                {
                    for (int i = 0; i < placeList.Count; i++)
                    {
                        model.PlaceIds += placeList[i].PlaceId + ",";
                        model.placeNames += placeList[i].PlaceName + ",";
                    }
                    model.PlaceIds = model.PlaceIds.Substring(0, model.PlaceIds.Length - 1);
                    model.placeNames = model.placeNames.Substring(0, model.placeNames.Length - 1);
                    model.PlaceList = GetPlaceList(shopInfo.CityId, placeList.Select(p => p.PlaceId).ToList());
                }
                else
                {
                    model.PlaceList = GetPlaceList(shopInfo.CityId, null);
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("ShopList");
            }
        }

        /// <summary>
        /// 编辑门店提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShop(ShopModel model)
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
                    shop.ProvinceId = model.ProvinceId;
                    shop.CityId = model.CityId;
                    shop.CountyId = model.CountyId;
                    shop.Address = model.Address;
                    shop.MainSale = model.MainSale;
                    shop.Content = model.Content;
                    shop.StartBusinessTime = model.StartBusinessTime;
                    shop.EndBusinessTime = model.EndBusinessTime;
                    //shop.IsDelivery = model.IsDelivery ? 1 : 0;
                    shop.Type = model.Types;
                    shop.UpdateTime = DateTime.Now;

                    //修改保存到数据库
                    if (ShopBll.UpdateShop(shop, model.PlaceIds))
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
        /// 获取本门店所在市的小区
        /// </summary>
        /// <param name="cityId">市ID</param>
        /// <returns></returns>
        public List<SelectListItem> GetPlaceList(int cityId, List<int> PlaceIds)
        {
            List<SelectListItem> placeList = new List<SelectListItem>();

            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(m => m.CityId == cityId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList();
            foreach (var item in list)
            {
                placeList.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected = PlaceIds == null ? false : PlaceIds.Contains(item.Id)
                });
            }
            return placeList;
        }

        /// <summary>
        /// 获取指定城市内的服务小区
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPropertyPlaceList(int cityId)
        {
            List<object> list = new List<object>();
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            foreach (var item in placeBll.GetList(m => m.CityId == cityId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.Name });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取门店类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetTypeList()
        {
            // 类型列表
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_0,
                Value = ConstantParam.SHOP_TYPE_0.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_1,
                Value = ConstantParam.SHOP_TYPE_1.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_2,
                Value = ConstantParam.SHOP_TYPE_2.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_3,
                Value = ConstantParam.SHOP_TYPE_3.ToString(),
                Selected = false
            });
            return typeList;
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
        /// 删除指定门店
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteShop(int id)
        {
            JsonModel jm = new JsonModel();
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            // 根据指定id值获取实体对象
            var shopInfo = ShopBll.GetEntity(s => s.Id == id);
            if (shopInfo != null)
            {
                //删除门店
                if (ShopBll.Delete(shopInfo))
                {
                    // 删除门店图片
                    if (!string.IsNullOrEmpty(shopInfo.ImgPath))
                    {
                        //删除文件图片
                        string[] imgPaths = shopInfo.ImgPath.Split(';');
                        foreach (var imgPath in imgPaths)
                        {
                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                FileInfo f = new FileInfo(Server.MapPath(imgPath));
                                if (f.Exists)
                                    f.Delete();
                            }
                        }
                    }

                    // 删除门店缩略图片
                    if (!string.IsNullOrEmpty(shopInfo.ImgThumbnail))
                    {
                        //删除文件图片
                        string[] imgThumbnailPaths = shopInfo.ImgThumbnail.Split(';');
                        foreach (var imgPath in imgThumbnailPaths)
                        {
                            if (!string.IsNullOrEmpty(imgPath))
                            {
                                FileInfo f = new FileInfo(Server.MapPath(imgPath));
                                if (f.Exists)
                                    f.Delete();
                            }
                        }
                    }

                    jm.Content = "删除门店 " + shopInfo.ShopName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            else
            {
                jm.Msg = "该门店不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
