using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 沟通列表搜索模型
    /// </summary>
    public class LinkupTypeSearchModel : SearchModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeforeDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 发帖人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 标题下拉列表
        /// </summary>
        public List<SelectListItem> TitleList { get; set; }

        /// <summary>
        /// 查询到的沟通列表
        /// </summary>
        public PagedList<T_PostBarTopic> DataList { get; set; }
    }

}