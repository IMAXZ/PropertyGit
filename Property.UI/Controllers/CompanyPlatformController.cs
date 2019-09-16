using MvcBreadCrumbs;
using Property.Common;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 总公司平台首页
    /// </summary>
    public class CompanyPlatformController : BaseController
    {
        /// <summary>
        /// 跳转到首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "总公司平台首页")]
        public ActionResult Index(int id = 1)
        {
            CompanyPlatformIndexModel model = new CompanyPlatformIndexModel();
            int CurrentCompanyId = GetSessionModel().CompanyId.Value;

            //获取当前公司物业小区个数并赋值
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            model.PlaceCount = placeBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.CompanyId == CurrentCompanyId);

            //获取当前公司下属小区物业人员总个数
            IPropertyUserBLL placeUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            model.PlaceUserCount = placeUserBll.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //住宅业主个数
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            model.HouseUserCount = houseUserBll.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            //办公楼业主个数
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            model.BuildCompanyCount = buildCompanyBll.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取物业公告个数
            IPostBLL postBLL = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            int postCount = postBLL.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId && u.PublishedFlag == ConstantParam.PUBLISHED_TRUE && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            model.PlacePostCount = postCount;


            //获取业主上报问题个数
            IQuestionBLL questionBLL = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            int QuestionCount = questionBLL.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId);

            //获取业主上报问题已处理个数
            int DisposedQuestionCount = questionBLL.Count(u => u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.PropertyPlace.CompanyId == CurrentCompanyId && u.Status == ConstantParam.DISPOSED);

            //设置上报问题处理率
            model.QuestionDisposedRate = Convert.ToDouble(DisposedQuestionCount) / QuestionCount;


            //获取巡检异常个数
            IInspectionResultBLL resultBLL = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            int InspectionExceptionCount = resultBLL.Count(u => u.InspectionTimePlan.InspectionPlan.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.InspectionTimePlan.InspectionPlan.PropertyPlace.CompanyId == CurrentCompanyId
                && u.Status == ConstantParam.EXCEPTION && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取巡检异常已处理个数
            int DisposedInspectionExceptionCount = resultBLL.Count(u => u.InspectionTimePlan.InspectionPlan.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && u.InspectionTimePlan.InspectionPlan.PropertyPlace.CompanyId == CurrentCompanyId && u.Status == ConstantParam.EXCEPTION
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.DisposeStatus == ConstantParam.DISPOSED);

            //设置巡检异常处理率
            model.InspectionExceptionDisposedRate = Convert.ToDouble(DisposedInspectionExceptionCount) / InspectionExceptionCount;


            var dataList = placeBll.GetList(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.CompanyId == CurrentCompanyId).ToList();
            var ExpenseCountList = new List<ExpenseCountModel>();
            foreach (var item in dataList)
            {
                var m = new ExpenseCountModel();
                m.PlaceName = item.Name;
                //如果是住宅小区
                if (item.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
                {
                    IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
                    m.DoorCount = doorBll.Count(d => d.BuildUnit.Build.PropertyPlaceId == item.Id);

                    int ExpensedDoorCount = doorBll.Count(d => d.BuildUnit.Build.PropertyPlaceId == item.Id && d.HouseUserExpenseDetails.Count > 0 &&
                        d.HouseUserExpenseDetails.OrderByDescending(e => e.CreateDate).FirstOrDefault().IsPayed == ConstantParam.PAYED_TRUE);
                    m.ExpensedRate = Convert.ToDouble(ExpensedDoorCount) / m.DoorCount;
                }
                //如果是办公楼小区
                else if (item.PlaceType == ConstantParam.PLACE_TYPE_COMPANY)
                {
                    m.DoorCount = item.BuildCompanys.Count(c => c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                    int ExpensedDoorCount = item.BuildCompanys.Count(c => c.HouseUserExpenseDetails.Count > 0 && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                        && c.HouseUserExpenseDetails.OrderByDescending(d => d.CreateDate).FirstOrDefault().IsPayed == ConstantParam.PAYED_TRUE);
                    m.ExpensedRate = Convert.ToDouble(ExpensedDoorCount) / m.DoorCount;
                }
                ExpenseCountList.Add(m);
            }
            model.ExpenseCountList = ExpenseCountList.OrderByDescending(m => m.ExpensedRate).ToPagedList(id, ConstantParam.PAGE_SIZE);
            return View(model);
        }
    }
}
