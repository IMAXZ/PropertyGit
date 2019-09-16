using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers.Weixin
{
    public class WeixinPropertyNoticeController : WeixinBaseController
    {
        /// <summary>
        /// 物业公告列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PropertyNoticeList()
        {
            var owner = GetCurrentUser();
            var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            ViewBag.PropertyNoticeCount = postBll.Count(m => placeList.Contains(m.PropertyPlaceId) && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.PublishedFlag == ConstantParam.PUBLISHED_TRUE);
            return View();
        }

        /// <summary>
        /// 物业公告Json方式获取
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult PropertyNoticeJsonList(int pageIndex)
        {
            PageResultModel model = new PageResultModel();
            var owner = GetCurrentUser();
            var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);

            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");

            var list = postBll.GetPageList(m => placeList.Contains(m.PropertyPlaceId) && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.PublishedFlag == ConstantParam.PUBLISHED_TRUE, "PublishedTime", false, pageIndex).Select(m => new
            {
                PropertyName = m.PropertyPlace.Name,
                PropertyImg = string.IsNullOrEmpty(m.PropertyPlace.ImgThumbnail) ? "/Images/news_item_default.png" : m.PropertyPlace.ImgThumbnail,
                Id = m.Id,
                Title = m.Title,
                PublishedTime = m.PublishedTime,
                strPublishedTime = m.PublishedTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            model.Total = postBll.Count(m => placeList.Contains(m.PropertyPlaceId) && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.PublishedFlag == ConstantParam.PUBLISHED_TRUE);
            model.Result = list;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 物业公告详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PropertyNoticeDetail(int id)
        {
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var post = postBll.GetEntity(m => m.Id == id);
            var model = new PropertyNoticeDetailModel()
            {
                Id = post.Id,
                PlaceName = post.PropertyPlace.Name,
                Content = post.Content,
                Title = post.Title,
                PublishedTime = post.PublishedTime.HasValue ? post.PublishedTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
            };

            return View(model);
        }
    }
}
