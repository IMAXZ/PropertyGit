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
    public class CompanyController : BaseController
    {
        /// <summary>
        /// 物业总公司公告一览
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新闻公告列表")]
        [HttpGet]
        public ActionResult CompanyNewNoticeList(NewsNoticeSearchModel model)
        {
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            int propertyCompanyId = GetSessionModel().CompanyId.Value;
            Expression<Func<T_Post, bool>> where = u => u.PropertyPlace.CompanyId == propertyCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && (string.IsNullOrEmpty(model.Title) ? true : u.Title.Contains(model.Title)) && (u.PublishedFlag == Property.Common.ConstantParam.PUBLISHED_TRUE);
            if (model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == model.PropertyPlaceId.Value);
            }
            var sortModel = this.SettingSorting("Id", false);
            model.PostList = postBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_Post>;
            model.PropertyPlaceList = GetPropertyPlaceList();
            return View(model);
        }
        /// <summary>
        /// 获取物业小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetPropertyPlaceList()
        {
            int CompanyId = GetSessionModel().CompanyId.Value;
            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CompanyId == CompanyId);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }
        /// <summary>
        /// 物业总公司查看公告详细
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新闻公告详细")]
        [HttpGet]
        public ActionResult ReviewScanPost(int id)
        {
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                NewsNoticeModel postModel = new NewsNoticeModel();
                postModel.PostId = post.Id;
                postModel.Title = post.Title;
                postModel.Content = post.Content;
                postModel.SubmitUserId = post.SubmitUserId;
                postModel.SubmitUser = post.SubmitUser.TrueName;
                postModel.SubmitTime = post.SubmitTime.ToString();
                postModel.PuslishedTime = post.PublishedTime.ToString();
                postModel.PublishedFlag = post.PublishedFlag == 0 ? false : true;
                postModel.SubmitUserHeadPath = post.SubmitUser.HeadPath;
                return View(postModel);
            }
            else
            {
                return RedirectToAction("CompanyNewNoticeList");
            }
        }
        /// <summary>
        /// 总公司物业人员一览
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "物业用户列表")]
        [HttpGet]
        public ActionResult PropertyUserList(PropertyPlaceSearchModel model)
        {
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            int propertyCompanyId = GetSessionModel().CompanyId.Value;
            Expression<Func<T_PropertyUser, bool>> where = u => (string.IsNullOrEmpty(model.Kword) ? true : (u.TrueName.Contains(model.Kword) || u.UserName.Contains(model.Kword))) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlace.CompanyId == propertyCompanyId;
            if (model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == model.PropertyPlaceId.Value);
            }
            var sortModel = this.SettingSorting("Id", false);
            model.DataUserList = propertyUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PropertyUser>;
            model.PropertyPlaceList = GetPropertyPlaceList();
            return View(model);
        }
    }
}
