using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Property.Entity;

namespace Property.UI.Models
{

    #region 巡检内容（类别）
    /// <summary>
    /// 巡检内容（类别）表单模型
    /// </summary>
    public class InspectionCategoryModel
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(1000)]
        public string Memo { get; set; }

    }

    /// <summary>
    /// 巡检内容查询模型
    /// </summary>
    public class InspectionCategorySearchModel : SearchModel
    {
        /// <summary>
        /// 巡检内容名称
        /// </summary>
        public string CategoryName { get; set; }
    }
    #endregion

    #region 巡检点模型

    /// <summary>
    /// 巡检点表单模型
    /// </summary>
    public class InspectionPointModel
    {
        /// <summary>
        /// 巡检点ID
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PointName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Memo { get; set; }

        /// <summary>
        /// 所属巡检类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 巡检内容（类别）下拉列表
        /// </summary>
        public List<SelectListItem> CategoryList { get; set; }
    }

    /// <summary>
    /// 巡检点查询模型
    /// </summary>
    public class InspectionPointSearchModel : SearchModel
    {
        /// <summary>
        /// 巡检点名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 所属巡检类别ID
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 巡检内容（类别）下拉列表
        /// </summary>
        public List<SelectListItem> CategoryList { get; set; }

        /// <summary>
        /// 查询到的巡检点数据列表
        /// </summary>
        public PagedList<T_InspectionPoint> DataList { get; set; }
    }
    #endregion

    #region 巡检任务

    /// <summary>
    /// 巡检任务表单模型
    /// </summary>
    public class InspectionPlanModel
    {
        public InspectionPlanModel()
        {
            this.StartHourNums = new int?[10];
            this.EndHourNums = new int?[10];
            this.StartWeekNums = new int?[7];
            this.EndWeekNums = new int?[7];
            this.StartDayNums = new int?[10];
            this.EndDayNums = new int?[10];

            this.StartHourNumLists = new List<List<SelectListItem>>();
            this.EndHourNumLists = new List<List<SelectListItem>>();
            this.StartWeekNumLists = new List<List<SelectListItem>>();
            this.EndWeekNumLists = new List<List<SelectListItem>>();
            this.StartDayNumLists = new List<List<SelectListItem>>();
            this.EndDayNumLists = new List<List<SelectListItem>>();
        }

        /// <summary>
        /// 巡检任务ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// 巡检任务名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PlanName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 巡检类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 是否随机巡检
        /// </summary>
        public int IsRandom { get; set; }

        /// <summary>
        /// 随机巡检次数
        /// </summary>
        public int? RandomNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Memo { get; set; }

        /// <summary>
        /// 开始时间集合
        /// </summary>
        public int?[] StartHourNums { get; set; }

        /// <summary>
        /// 结束时间集合
        /// </summary>
        public int?[] EndHourNums { get; set; }

        /// <summary>
        /// 小时下拉选择列表
        /// </summary>
        public List<SelectListItem> HourNumList { get; set; }

        /// <summary>
        /// 所有的开始小时数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> StartHourNumLists { get; set; }

        /// <summary>
        /// 所有的结束小时数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> EndHourNumLists { get; set; }

        /// <summary>
        /// 所有的开始周数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> StartWeekNumLists { get; set; }

        /// <summary>
        /// 所有的结束周数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> EndWeekNumLists { get; set; }

        /// <summary>
        /// 所有的开始天数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> StartDayNumLists { get; set; }

        /// <summary>
        /// 所有的结束天数下拉选择列表
        /// </summary>
        public List<List<SelectListItem>> EndDayNumLists { get; set; }

        /// <summary>
        /// 开始周数集合
        /// </summary>
        public int?[] StartWeekNums { get; set; }

        /// <summary>
        /// 结束周几集合
        /// </summary>
        public int?[] EndWeekNums { get; set; }

        /// <summary>
        /// 周下拉选择列表
        /// </summary>
        public List<SelectListItem> WeekNumList { get; set; }

        /// <summary>
        /// 开始天集合
        /// </summary>
        public int?[] StartDayNums { get; set; }

        /// <summary>
        /// 结束天集合
        /// </summary>
        public int?[] EndDayNums { get; set; }

        /// <summary>
        /// 小时下拉选择列表
        /// </summary>
        public List<SelectListItem> DayNumList { get; set; }

        /// <summary>
        /// 巡检点id
        /// </summary>
        public int[] PointIds { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// 巡检点列表
        /// </summary>
        public List<SelectListItem> PointList { get; set; }

        /// <summary>
        /// 巡检类型下拉列表
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// 是否随机列表
        /// </summary>
        public List<SelectListItem> IsRandomList { get; set; }
    }

    /// <summary>
    /// 巡检任务查询模型
    /// </summary>
    public class InspectionPlanSearchModel : SearchModel
    {
        /// <summary>
        /// 小区ID
        /// </summary>
        public int? PlaceId { get; set; }

        /// <summary>
        /// 巡检任务名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? BeforeDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 巡检类型
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 巡检类型列表
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// 小区下拉列表
        /// </summary>
        public List<SelectListItem> PlaceList { get; set; }

        /// <summary>
        /// 查询到的巡检任务列表
        /// </summary>
        public PagedList<T_InspectionPlan> DataList { get; set; }
    }

    /// <summary>
    /// 巡检点 巡检内容（类别）模型
    /// </summary>
    public class PointCategoryModel
    {
        /// <summary>
        /// 巡检内容类别
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 巡检点描述
        /// </summary>
        public string PointMemo { get; set; }
    }

    #endregion

    #region 巡检结果模型

    /// <summary>
    /// 巡检实施监控（状态）模型（日历展示）
    /// </summary>
    public class InspectionMonitorModel
    {
        /// <summary>
        /// 巡检实施标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 巡检实施开始日期
        /// </summary>
        public string start { get; set; }

        /// <summary>
        /// 巡检实施结束日期
        /// </summary>
        public string end { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 点击后跳转的地址
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// 巡检结果查询模型
    /// </summary>
    public class InspenctionResultSearchModel
    {
        /// <summary>
        /// 巡检时间安排Id
        /// </summary>
        public int PlanTimeId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// 巡检结果模型
    /// </summary>
    public class InspectionResultModel
    {
        /// <summary>
        /// 巡检计划名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 巡检点列表
        /// </summary>
        public List<T_InspectionPoint> PointList { get; set; }

        /// <summary>
        /// 巡检结果列表
        /// </summary>
        public List<T_InspectionResult> ResultList { get; set; }
    }

    #endregion

    #region 巡检异常

    /// <summary>
    /// 巡检异常查询模型
    /// </summary>
    public class InspenctionExceptionSearchModel : SearchModel
    {
        /// <summary>
        /// 所属小区ID
        /// </summary>
        public int? PlaceId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 查询到的异常结果列表
        /// </summary>
        public PagedList<T_InspectionResult> DataList { get; set; }

        /// <summary>
        /// 处理状态  0:未处理  1:已处理
        /// </summary>
        public int? DisposeStatus { get; set; }

        /// <summary>
        /// 状态下拉列表
        /// </summary>
        public List<SelectListItem> StatusList { get; set; }

        /// <summary>
        /// 小区下拉列表
        /// </summary>
        public List<SelectListItem> PlaceList { get; set; }
    }

    /// <summary>
    /// 巡检异常处理模型
    /// </summary>
    public class DisposeExceptionModel
    {
        /// <summary>
        /// 巡检异常结果ID
        /// </summary>
        public int ResultId { get; set; }

        /// <summary>
        /// 巡检任务名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 处理描述
        /// </summary>
        public string DisposeDesc { get; set; }
    }

    /// <summary>
    /// 指派处理人模型
    /// </summary>
    public class SetDisposerModel
    {
        /// <summary>
        /// 巡检异常结果ID
        /// </summary>
        public int ResultId { get; set; }

        /// <summary>
        /// 巡检任务名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string ExceptionDesc { get; set; }

        /// <summary>
        /// 处理人ID
        /// </summary>
        public int DisposerId { get; set; }

        /// <summary>
        /// 物业用户列表
        /// </summary>
        public List<SelectListItem> UserList { get; set; }
    }
    #endregion
}
