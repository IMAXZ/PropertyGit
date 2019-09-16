using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 意见反馈查询模型
    /// </summary>
    public class FeedbackSearchModel : SearchModel
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 查询到的意见反馈
        /// </summary>
        public PagedList<T_Feedback> DataList { get; set; }
    }
}