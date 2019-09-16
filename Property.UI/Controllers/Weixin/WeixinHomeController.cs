using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 微信首页控制器
    /// </summary>
    public class WeixinHomeController : WeixinBaseController
    {
        /// <summary>
        /// 公众号首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            HomeDataModel model = new HomeDataModel();
            var owner = GetCurrentUser();
            var PlaceIds = GetVerifiedPlaceIds();
            //初始化查询条件
            var DoorIds = owner.PropertyIdentityVerification.Where(v => v.DoorId != null && v.IsVerified == 1).Select(m => m.DoorId);
            var CompanyIds = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.IsVerified == 1).Select(m => m.BuildCompanyId);
            Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => u.IsPayed == ConstantParam.PAYED_FALSE && (DoorIds.Contains(u.BuildDoorId) || CompanyIds.Contains(u.BuildCompanyId));
            // 获取当前用户对应业主的缴费记录
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            model.ExpenseList = expenseDetailsBLL.GetPageList(where, "CreateDate", false, 1, 1).Select(e => new ExpenseNoticeModel
            {
                ExpenseType = e.PropertyExpenseType.Name,
                PlaceName = e.BuildCompanyId == null ? e.BuildDoor.BuildUnit.Build.PropertyPlace.Name : e.BuildCompany.PropertyPlace.Name,
                OwnerDoor = e.BuildCompanyId == null ? (e.BuildDoor.BuildUnit.Build.BuildName + e.BuildDoor.BuildUnit.UnitName + e.BuildDoor.DoorName) : e.BuildCompany.Name,
                ExpenseDateDes = e.ExpenseDateDes,
                Cost = e.Expense + "元"
            }).ToList();

            // 获取用户关联小区的公告列表
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);
            Expression<Func<T_Post, bool>> where1 = u => placeList.Contains(u.PropertyPlaceId) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PublishedFlag == ConstantParam.PUBLISHED_TRUE;
            model.NewsList = postBll.GetPageList(where1, "PublishedTime", false, 1, 3).Select(p => new NewsModel
            {
                Id = p.Id,
                PlaceName = p.PropertyPlace.Name,
                propertyPic = string.IsNullOrEmpty(p.PropertyPlace.ImgThumbnail) ? "/Images/news_item_default.png" : p.PropertyPlace.ImgThumbnail,
                PublishTime = p.PublishedTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = p.Title
            }).ToList();

            //获取最新上报的问题
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            Expression<Func<T_Question, bool>> where2 = u => u.UploadUserId == owner.Id && PlaceIds.Contains(u.PropertyPlaceId);
            model.QuestionList = questionBll.GetPageList(where2, "Id", false, 1, 2).ToList().Select(q => new QuestionModel
            {
                Id = q.Id,
                PlaceName = q.PropertyPlace.Name,
                Title = q.Title,
                Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                Status = q.Status,
                UploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[] { } : q.Imgs.Split(';'),
                AudioPath = q.AudioPath,
                VoiceDuration = q.VoiceDuration
            }).ToList();

            return View(model);
        }

    }
}
