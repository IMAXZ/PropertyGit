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
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 手机页面页面
    /// </summary>
    public class MobilePageController : Controller
    {
        /// <summary>
        /// 公告详细页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewsDetail(int id)
        {
            JsonModel jm = new JsonModel();
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var post = postBll.GetEntity(p => p.Id == id && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT 
                && p.PublishedFlag == ConstantParam.PUBLISHED_TRUE);

            NewsNoticeModel postModel = new NewsNoticeModel();
            postModel.Title = post.Title;
            postModel.Content = post.Content;
            postModel.PuslishedTime = post.PublishedTime.Value.ToShortDateString().Replace("/", ".");
            return View(postModel);
        }

        /// <summary>
        /// 公告详细页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CompanyNewsDetail(int id)
        {
            JsonModel jm = new JsonModel();
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var post = postBll.GetEntity(p => p.Id == id && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            NewsNoticeModel postModel = new NewsNoticeModel();
            postModel.Title = post.Title;
            postModel.Content = post.Content;
            postModel.PuslishedTime = post.PublishedTime.Value.ToShortDateString().Replace("/", ".");
            return View(postModel);
        }
        
        /// <summary>
        /// 门店详细页面
        /// </summary>
        /// <param name="ShopId">门店ID</param>
        /// <returns></returns>
        public ActionResult StoreDetail(int ShopId)
        {
            //获取要查看详细的门店实体对象
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            T_Shop Shop = ShopBll.GetEntity(s => s.Id == ShopId);
            return View(Shop);
        }

        /// <summary>
        /// 二期门店详细页面
        /// </summary>
        /// <param name="ShopId">门店ID</param>
        /// <returns></returns>
        public ActionResult StoreDetail2(int ShopId)
        {
            //获取要查看详细的门店实体对象
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            T_Shop Shop = ShopBll.GetEntity(s => s.Id == ShopId);
            return View(Shop);
        }

        /// <summary>
        /// 二期商家结束HTML页面
        /// </summary>
        /// <param name="ShopId">门店ID</param>
        /// <returns></returns>
        public ActionResult StoreIntroduce(int ShopId)
        {
            //获取要查看详细的门店实体对象
            IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
            T_Shop Shop = ShopBll.GetEntity(s => s.Id == ShopId);
            return View(Shop);
        }

        /// <summary>
        /// 获取指定门店促销分页列表数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="ShopId"></param>
        /// <returns></returns>
        public JsonResult ShopSaleList(int pageIndex, int shopId)
        {
            IShopSaleBLL SaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            var list = SaleBll.GetPageList(s => s.GoodsCategory.ShopId == shopId, "Id", false, pageIndex).Select(s => new
            {
                Id = s.Id,
                Title = s.Title,
                Content = s.Content.Length > 30 ? s.Content.Substring(0, 30) : s.Content,
                SaleImg = string.IsNullOrEmpty(s.ImgThumbnail) ? "" : s.ImgThumbnail.Split(';')[0]
            }).ToList();

            ApiPageResultModel model = new ApiPageResultModel();
            model.Total = SaleBll.Count(s => s.GoodsCategory.ShopId == shopId);
            model.result = list;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 促销详细页面
        /// </summary>
        /// <param name="id">促销ID</param>
        /// <returns></returns>
        public ActionResult SaleDetail(int id)
        {
            //获取要查看详细的门店实体对象
            IShopSaleBLL SaleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
            T_ShopSale Sale = SaleBll.GetEntity(s => s.Id == id);
            return View(Sale);
        }

        /// <summary>
        /// 办事首页
        /// </summary>
        /// <returns></returns>
        public ActionResult BanshiIndex()
        {
            return View();
        }

        /// <summary>
        /// 物业产品页
        /// </summary>
        /// <returns></returns>
        public ActionResult PropertyProduct() 
        {
            return View();
        }

        /// <summary>
        /// 下载业主APK
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public void DownOwnerApk()
        {
            //调用版本信息BLL层获取最新的版本信息
            IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_OWNER, "VersionCode", false);
            //如果版本信息不为空
            if (Versions != null && Versions.Count() > 0)
            {
                var highestVersion = Versions.First();
                if (highestVersion != null)
                {
                    string filePath = Server.MapPath(highestVersion.ApkFilePath);//路径
                    FileInfo fileInfo = new FileInfo(filePath);
                    string fileName = fileInfo.Name;
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.AddHeader("Content-Transfer-Encoding", "binary");
                    Response.ContentType = "application/octet-stream";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Response.WriteFile(fileInfo.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 下载物业APK
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public void DownPropertyApk()
        {
            //调用版本信息BLL层获取最新的版本信息
            IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_PROPERTY, "VersionCode", false);
            //如果版本信息不为空
            if (Versions != null && Versions.Count() > 0)
            {
                var highestVersion = Versions.First();
                if (highestVersion != null)
                {
                    string filePath = Server.MapPath(highestVersion.ApkFilePath);//路径
                    FileInfo fileInfo = new FileInfo(filePath);
                    string fileName = fileInfo.Name;
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.AddHeader("Content-Transfer-Encoding", "binary");
                    Response.ContentType = "application/octet-stream";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Response.WriteFile(fileInfo.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 下载商家APK
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public void DownStoreApk()
        {
            //调用版本信息BLL层获取最新的版本信息
            IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_SHOP, "VersionCode", false);
            //如果版本信息不为空
            if (Versions != null && Versions.Count() > 0)
            {
                var highestVersion = Versions.First();
                if (highestVersion != null)
                {
                    string filePath = Server.MapPath(highestVersion.ApkFilePath);//路径
                    FileInfo fileInfo = new FileInfo(filePath);
                    string fileName = fileInfo.Name;
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.AddHeader("Content-Transfer-Encoding", "binary");
                    Response.ContentType = "application/octet-stream";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Response.WriteFile(fileInfo.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}
