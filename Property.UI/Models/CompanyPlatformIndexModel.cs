using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 公司平台首页数据模型
    /// </summary>
    public class CompanyPlatformIndexModel
    {
        /// <summary>
        /// 物业小区个数
        /// </summary>
        public int PlaceCount { get; set; }

        /// <summary>
        /// 物业用户个数
        /// </summary>
        public int PlaceUserCount { get; set; }

        /// <summary>
        /// 物业公告个数
        /// </summary>
        public int PlacePostCount { get; set; }

        /// <summary>
        /// 物业业主个数
        /// </summary>
        public int HouseUserCount { get; set; }

        /// <summary>
        /// 办公楼业主数
        /// </summary>
        public int BuildCompanyCount { get; set; }

        /// <summary>
        /// 上报问题处理率
        /// </summary>
        public double QuestionDisposedRate { get; set; }

        /// <summary>
        /// 巡检异常处理率
        /// </summary>
        public double InspectionExceptionDisposedRate { get; set; }

        /// <summary>
        /// 缴费统计列表数据
        /// </summary>
        public PagedList<ExpenseCountModel> ExpenseCountList { get; set; }
    }


    /// <summary>
    /// 首页缴费统计模型
    /// </summary>
    public class ExpenseCountModel
    {
        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 单元户个数（单位个数）
        /// </summary>
        public int DoorCount { get; set; }

        /// <summary>
        /// 缴费率
        /// </summary>
        public double ExpensedRate { get; set; }
    }
}