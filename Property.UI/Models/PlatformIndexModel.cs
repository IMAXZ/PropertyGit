using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 平台管理首页数据模型
    /// </summary>
    public class PlatformIndexModel
    {
        /// <summary>
        /// 平台用户个数
        /// </summary>
        public int PlatformUserCount { get; set; }

        /// <summary>
        /// 物业公司个数
        /// </summary>
        public int CompanyCount { get; set; }

        /// <summary>
        /// 物业小区个数
        /// </summary>
        public int PlaceCount { get; set; }

        /// <summary>
        /// 业主个数
        /// </summary>
        public int OwnerCount { get; set; }

        /// <summary>
        /// 一周内业主访问量
        /// </summary>
        public int OwnerWeekLoginCount { get; set; }

        /// <summary>
        /// 一月内业主访问量
        /// </summary>
        public int OwnerMonthLoginCount { get; set; }

        /// <summary>
        /// 柱状图表Json数据
        /// </summary>
        public string BarJsonData { get; set; }

        /// <summary>
        /// 业主统计分页数据
        /// </summary>
        public PagedList<OwnerDataModel> OwnerDatas { get; set; }
    }

    /// <summary>
    /// 柱状图数据模型
    /// </summary>
    public class BarDataModel 
    {
        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司物业小区个数
        /// </summary>
        public int PlaceCount { get; set; }

        /// <summary>
        /// 该公司业主个数
        /// </summary>
        public int OwnerCount { get; set; }
    }

    /// <summary>
    /// 业主统计数据模型
    /// </summary>
    public class OwnerDataModel 
    {
        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 物业小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 业主个数
        /// </summary>
        public int OwnerCount { get; set; }
    }
}