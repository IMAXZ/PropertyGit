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
    /// <summary>
    /// 公司平台巡检模型
    /// </summary>
    public class CompanyInspectionController : BaseController
    {
        /// <summary>
        /// 巡检任务列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检任务一览")]
        public ActionResult PlanList(InspectionPlanSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.BeforeDate == null)
                model.BeforeDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            model.PlaceList = GetPlaceList();
            model.TypeList = GetInspectionTypeList();

            int currentCompanyId = GetSessionModel().CompanyId.Value;
            DateTime endDate = model.EndDate.Value.AddDays(1);
            //2.查询条件：未删除、巡检开始日期范围、已发布、当前总公司
            Expression<Func<T_InspectionPlan, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PublishedFlag == ConstantParam.PUBLISHED_TRUE
                && p.PropertyPlace.CompanyId == currentCompanyId && p.BeginDate < endDate && p.EndDate >= model.BeforeDate;
            //根据所属小区查询
            if (model.PlaceId != null)
            {
                where = PredicateBuilder.And(where, p => p.PropertyPlaceId == model.PlaceId.Value);
            }
            //根据巡检任务名称模糊查询
            if (!string.IsNullOrEmpty(model.PlanName))
            {
                where = PredicateBuilder.And(where, p => p.PlanName.Contains(model.PlanName));
            }
            //根据巡检类型查询
            if (model.Type != null)
            {
                where = PredicateBuilder.And(where, p => p.Type == model.Type.Value);
            }

            //3.根据查询条件调用BLL层 获取分页数据
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            var sortModel = this.SettingSorting("Id", false);
            model.DataList = planBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_InspectionPlan>;

            return View(model);
        }

        /// <summary>
        /// 巡检任务详细查看
        /// </summary>
        /// <param name="id">巡检任务ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "查看任务详细")]
        public ActionResult PlanDetail(int id)
        {
            //调用BLL层获取要查看详细的巡检任务
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            var plan = planBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (plan != null)
            {
                var PointCategorys = plan.PlanPoints.Select(p => new PointCategoryModel
                {
                    CategoryName = p.InspectionPoint.InspectionCategory.CategoryName,
                    PointName = p.InspectionPoint.PointName,
                    PointMemo = p.InspectionPoint.Memo
                }).ToList();

                List<string> CategoryList = PointCategorys.Select(p => p.CategoryName).Distinct().ToList();
                ViewBag.PointCategorys = PointCategorys;
                ViewBag.CategoryList = CategoryList;
                return View(plan);
            }
            else
            {
                return RedirectToAction("PlanList");
            }
        }
        /// <summary>
        /// 巡检实施监控结果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检实施监控")]
        public ActionResult ResultMonitorList()
        {
            ViewBag.PlaceList = GetPlaceList();
            return View();
        }

        /// <summary>
        /// 获取月巡检实施监控的日历数据
        /// </summary>
        /// <param name="Year">日历查询的年</param>
        /// <param name="Month">日历查询的月</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMonthPlanResultMonitorData(int PlaceId,int Year, int Month)
        {
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var SelectStartDate = Convert.ToDateTime(Year + " - " + Month);
            var SelectEndDate = SelectStartDate.AddMonths(1);
            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == PlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_MONTH && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    //月巡检
                    foreach (var monthPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH))
                    {
                        int EndDayNum = monthPlan.EndNum;
                        int[] smalls = new int[] { 4, 6, 9, 11 };
                        //如果是小月
                        if (smalls.Contains(Month))
                        {
                            if (monthPlan.BeginNum > 30)
                            {
                                continue;
                            }
                        }
                        else if (Month == 2)
                        {
                            //闰年
                            if ((Year % 4 == 0 && Year / 100 != 0) || (Year % 400 == 0))
                            {
                                if (monthPlan.BeginNum > 29)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (monthPlan.BeginNum > 28)
                                {
                                    continue;
                                }
                            }
                        }
                        DateTime start = SelectStartDate.AddDays(monthPlan.BeginNum - 1);

                        //如果不在巡检时间段内
                        if (start > plan.EndDate || start < plan.BeginDate)
                        {
                            continue;
                        }
                        DateTime end = start.AddDays(1);

                        string StatusColor = "";

                        var resultList = monthPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (count == plan.PlanPoints.Count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + monthPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    //月巡检
                    foreach (var monthPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectEndDate;

                        //如果不在巡检时间段内
                        if (end < plan.BeginDate || start > plan.EndDate)
                        {
                            continue;
                        }
                        if (start < plan.BeginDate)
                        {
                            start = plan.BeginDate;
                        }
                        if (end > plan.EndDate)
                        {
                            end = plan.EndDate.AddDays(1);
                        }

                        string StatusColor = "";

                        var resultList = monthPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + monthPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + monthPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取周巡检实施监控的日历数据
        /// </summary>
        /// <param name="SelectDate">日历周开始日期（周一）</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetWeekPlanResultMonitorData(int PlaceId, string SelectDate)
        {
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var d = Convert.ToDateTime(SelectDate);
            var SelectStartDate = d.AddDays(1 - (int)d.DayOfWeek);
            var SelectEndDate = SelectStartDate.AddDays(7);

            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == PlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_WEEK && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    foreach (var weekPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK))
                    {
                        DateTime start = SelectStartDate.AddDays(weekPlan.BeginNum - 1);

                        //如果不在巡检时间段内
                        if (start > plan.EndDate || start < plan.BeginDate)
                        {
                            continue;
                        }
                        DateTime end = start.AddDays(1);

                        string StatusColor = "";

                        var resultList = weekPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + weekPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    foreach (var weekPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectEndDate;

                        //如果不在巡检时间段内
                        if (end < plan.BeginDate || start > plan.EndDate)
                        {
                            continue;
                        }
                        if (start < plan.BeginDate)
                        {
                            start = plan.BeginDate;
                        }
                        if (end > plan.EndDate)
                        {
                            end = plan.EndDate.AddDays(1);
                        }

                        string StatusColor = "";

                        var resultList = weekPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (SelectEndDate < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + weekPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + weekPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取日巡检实施监控的日历数据
        /// </summary>
        /// <param name="Date">日期</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDayPlanResultMonitorData(int PlaceId, string Date)
        {
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var SelectStartDate = Convert.ToDateTime(Date);
            var SelectEndDate = SelectStartDate.AddDays(1);

            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == PlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_DAY && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    foreach (var dayPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY))
                    {
                        DateTime start = SelectStartDate.AddHours(dayPlan.BeginNum);
                        DateTime end = SelectStartDate.AddHours(dayPlan.EndNum);

                        string StatusColor = "";

                        var resultList = dayPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end.Date.AddDays(1) < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd HH"),
                            end = end.ToString("yyyy-MM-dd HH"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + dayPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.AddDays(1).ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    foreach (var dayPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectStartDate.AddDays(1);

                        string StatusColor = "";

                        var resultList = dayPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + dayPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/CompanyInspection/ResultDetail?PlanTimeId=" + dayPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看巡检结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检结果查看")]
        public ActionResult ResultDetail(InspenctionResultSearchModel model)
        {
            IInspectionTimePlanBLL planTimeBll = BLLFactory<IInspectionTimePlanBLL>.GetBLL("InspectionTimePlanBLL");
            var planTime = planTimeBll.GetEntity(p => p.Id == model.PlanTimeId);
            if (planTime != null)
            {
                InspectionResultModel res = new InspectionResultModel()
                {
                    PlanName = planTime.InspectionPlan.PlanName,
                    PointList = planTime.InspectionPlan.PlanPoints.Select(p => p.InspectionPoint).ToList(),
                    ResultList = planTime.InspectionResults.Where(r => r.PlanDate >= model.StartDate && r.PlanDate < model.EndDate).ToList()
                };
                return View(res);
            }
            else
            {
                return RedirectToAction("ResultMonitorList");
            }
        }

        /// <summary>
        /// 巡检异常一览
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检异常一览")]
        public ActionResult ExceptionList(InspenctionExceptionSearchModel model)
        {
            //获取当前物业公司ID
            int currentCompanyId = GetSessionModel().CompanyId.Value;

            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;
            model.StatusList = GetStatusList();
            model.PlaceList = GetPlaceList();

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);

            //查询条件：当前公司 异常状态
            Expression<Func<T_InspectionResult, bool>> where = r => r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && r.InspectionTimePlan.InspectionPlan.PropertyPlace.CompanyId == currentCompanyId && r.Status == ConstantParam.EXCEPTION
                && r.PlanDate >= model.StartTime.Value && r.PlanDate < endTime;
            //根据小区查询
            if (model.PlaceId != null)
            {
                where = PredicateBuilder.And(where, r => r.InspectionTimePlan.InspectionPlan.PropertyPlaceId == model.PlaceId.Value);
            }
            //根据处理状态名称查询
            if (model.DisposeStatus != null)
            {
                where = PredicateBuilder.And(where, u => u.DisposeStatus == model.DisposeStatus);
            }

            //根据巡检点名称模糊查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, r => r.InspectionPoint.PointName.Contains(model.Kword));
            }
            //排序
            var sortModel = this.SettingSorting("id", false);

            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            model.DataList = resultBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_InspectionResult>;
            return View(model);
        }

        /// <summary>
        /// 巡检异常详细查看
        /// </summary>
        /// <param name="id">巡检异常结果ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检异常详细")]
        public ActionResult ExceptionDetail(int id)
        {
            //获取要查看详细的异常信息
            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            var result = resultBll.GetEntity(r => r.Id == id && r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return RedirectToAction("ExceptionList");
            }
        }

        /// <summary>
        /// 获取物业小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetPlaceList()
        {
            int CurrentCompanyId = GetSessionModel().CompanyId.Value;
            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CompanyId == CurrentCompanyId);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 巡检类型下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetInspectionTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "日巡检",
                Value = ConstantParam.INSPECTION_TYPE_DAY.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "周巡检",
                Value = ConstantParam.INSPECTION_TYPE_WEEK.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "月巡检",
                Value = ConstantParam.INSPECTION_TYPE_MONTH.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 处理状态下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetStatusList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "未处理",
                Value = ConstantParam.NO_DISPOSE.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已处理",
                Value = ConstantParam.DISPOSED.ToString(),
                Selected = false
            });
            return list;
        }
    }
}
