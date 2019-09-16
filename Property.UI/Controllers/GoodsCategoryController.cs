using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    public class GoodsCategoryController : ShopBaseController
    {
        //
        // GET: /GoodsCategory/
        /// <summary>
        /// 促销商品列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "商品类别列表")]
        [HttpGet]
        public ActionResult GoodsCategoryList(GoodsCategoryModel model)
        {
            IGoodsCategoryBLL goodsCategoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
            var shopId = GetCurrentShopId().Value;
            Expression<Func<T_GoodsCategory, bool>> where = u => (string.IsNullOrEmpty(model.Name) ? true : u.Name.Contains(model.Name))&&u.ShopId==shopId;
            //查询条件
            //排序
            var sortModel = this.SettingSorting("Id", false);

            //将查询到的数据赋值传到页面
            model.DataList = goodsCategoryBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_GoodsCategory>;
            return View(model);
        }
        /// <summary>
        /// 促销产品新增
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增商品类别")]
        [HttpGet]
        public ActionResult AddGoodsCategory() 
        {
            return View();
        }
        /// <summary>
        /// 促销商品新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGoodsCategory(GoodsCategoryModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IGoodsCategoryBLL goodsCategoryTypeBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
                T_GoodsCategory goodsCategory = new T_GoodsCategory()
                {
                    Name = model.Name,
                    ShopId=GetCurrentShopId().Value
                };
                // 保存
                goodsCategoryTypeBll.Save(goodsCategory);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 编辑促销商品分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑商品类别")]
        [HttpGet]
        public ActionResult EditGoodsCategory(int id)
        {
            IGoodsCategoryBLL goodsCategoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
            var shopId = GetCurrentShopId().Value;
            //获取要编辑的物业账户
            T_GoodsCategory goodsCategory = goodsCategoryBll.GetEntity(m => m.Id == id&&m.ShopId==shopId);

            if (goodsCategory != null)
            {
                //初始化返回页面的模型
                GoodsCategoryModel model = new GoodsCategoryModel()
                {
                    Id=goodsCategory.Id,
                    Name = goodsCategory.Name
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("GoodsCategoryList");
            }
        }
        /// <summary>
        /// 编辑促销商品分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditGoodsCategory(GoodsCategoryModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IGoodsCategoryBLL goodsCategoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");

                T_GoodsCategory goodsCategory = goodsCategoryBll.GetEntity(m => m.Id == model.Id);
                if (goodsCategory != null)
                {
                    goodsCategory.Name = model.Name;
                    //保存到数据库
                    if (goodsCategoryBll.Update(goodsCategory))
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
                    jm.Msg = "该类别不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除促销商品分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "删除商品类别")]
        [HttpPost]
        public ActionResult DeleteGoodsCategory(int id)
        {
            JsonModel jm = new JsonModel();
            IGoodsCategoryBLL goodsCategoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
            //获取要删除的类别
            T_GoodsCategory goodsCategory = goodsCategoryBll.GetEntity(m => m.Id == id);
            if (goodsCategory == null)
            {
                jm.Msg = "该类别不存在";
            }
            else if (goodsCategory.ShopSales.Count()> 0)
            {
                jm.Msg = "已有该类型的商品，无法删除";
            }
            else
            {
                if (goodsCategoryBll.Delete(goodsCategory))
                {
                    //操作日志
                    jm.Content = "删除商品类别 " + goodsCategory.Name;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 远程验证商品类别是否存在
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult RemoteCheckExist(int Id, string Name)
        {
            
            IGoodsCategoryBLL goodsCategoryBll = BLLFactory<IGoodsCategoryBLL>.GetBLL("GoodsCategoryBLL");
            var ShopId = GetCurrentShopId().Value;
            if (goodsCategoryBll.Exist(m => m.Name == Name && m.Id != Id&&m.ShopId ==ShopId))
            {
                // 校验不通过
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
    }
}
