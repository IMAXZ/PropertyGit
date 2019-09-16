using MvcBreadCrumbs;
using Property.Common;
using Property.DAL;
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

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业信息平台首页
    /// </summary>
    public class PropertyController : BaseController
    {
        /// <summary>
        /// 物业平台首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "物业平台首页")]
        public ActionResult Index()
        {
            //构造首页数据模型
            PropertyIndexModel model = new PropertyIndexModel();

            //获取物业小区
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId ?? 0;

            //获取app用户统计数据
            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            int appUserCount = userBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.UserPlaces.Any(up => up.PropertyPlaceId == CurrentPlaceId));
            model.AppUserCount = appUserCount;

            //获取物业用户统计数据
            IPropertyUserBLL pUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            int pUserCount = pUserBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlaceId == CurrentPlaceId);
            model.PropertyUserCount = pUserCount;

            //获取新闻发布个数
            IPostBLL postBLL = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            int postCount = postBLL.Count(u => u.PublishedFlag == ConstantParam.PUBLISHED_TRUE && u.PropertyPlaceId == CurrentPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            model.NoticeCount = postCount;


            //获取业主上报问题个数
            IQuestionBLL questionBLL = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            int questionCount = questionBLL.Count(u => u.PropertyPlaceId == CurrentPlaceId);
            model.QuestionCount = questionCount;

            //获取业主上报本月未处理问题个数
            DateTime monthBegin = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            DateTime monthEnd = DateTime.Now.Date.AddDays(1);
            int notHandleQuestionCount = questionBLL.Count(u => u.Status == ConstantParam.NO_DISPOSE && u.UploadTime > monthBegin &&
                u.UploadTime < monthEnd && u.PropertyPlaceId == CurrentPlaceId);
            model.NotHandleQuestionCount = notHandleQuestionCount;

            //获取本月巡检未处理问题个数
            IInspectionResultBLL resultBLL = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            int notHandleExCount = resultBLL.Count(u => u.Status == ConstantParam.EXCEPTION && u.PlanDate >= monthBegin &&
                u.PlanDate < monthEnd
                && (u.DisposeStatus == null || u.DisposeStatus == ConstantParam.NO_DISPOSE) && u.InspectionTimePlan.InspectionPlan.PropertyPlaceId == CurrentPlaceId);
            model.NotHandleExceptionCount = notHandleExCount;


            //获取最新的为题列表
            var list = questionBLL.GetPageList(q => q.PropertyPlaceId == CurrentPlaceId, "UploadTime", false, 1, 5);
            model.LatestQuestionList = list;


            //获取当前小区所属公司ID
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = placeBll.GetEntity(p => p.Id == CurrentPlaceId);
            int currentCompanyId = place.CompanyId;

            //查询条件初始化
            Expression<Func<T_CompanyPost, bool>> where = u => u.CompanyId == currentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PublishStatus == ConstantParam.PUBLISHED_TRUE && u.IsOpen == ConstantParam.PUBLISHED_TRUE;

            //获取最新的5条总公司新闻公告
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            model.LatestCompanyPostList = postBll.GetPageList(where, "PublishedTime", false, 1, 5);

            return View(model);
        }
    }
}
