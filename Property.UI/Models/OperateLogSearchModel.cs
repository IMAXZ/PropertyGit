using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 操作日志搜索模型
    /// </summary>
    public class OperateLogSearchModel : SearchModel
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 平台日志数据集合
        /// </summary>
        public PagedList<T_PlatformOpreateLog> PlatformLogList { get; set; }

        /// <summary>
        /// 物业日志数据集合
        /// </summary>
        public PagedList<T_PropertyOpreateLog> PropertyLogList { get; set; }

        /// <summary>
        /// 物业总公司日志数据集合
        /// </summary>
        public PagedList<T_CompanyOpreateLog> CompanyLogList { get; set; }
    }
}