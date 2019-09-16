using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 总公司新闻公告模型
    /// </summary>
    public class CompanyNoticeModel
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [AllowHtml]
        public string Content { get; set; }

        /// <summary>
        /// 发布标识
        /// </summary>
        public bool PublishedFlag { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsOpen { get; set; }
    }

    /// <summary>
    /// 总公司新闻公告搜索模型
    /// </summary>
    public class CompanyNewsNoticeSearchModel : SearchModel
    {

        /// <summary>
        /// 发布标识  0:未发布 1:已发布
        /// </summary>
        public int? PublishedFlag { get; set; }

        /// <summary>
        /// 公告状态下拉列表
        /// </summary>
        public List<SelectListItem> StatusList { get; set; }

        /// <summary>
        /// 是否公开  0:已公开 1:未公开
        /// </summary>
        public int? IsOpen { get; set; }

        /// <summary>
        /// 是否公开下拉列表
        /// </summary>
        public List<SelectListItem> IsOpenList { get; set; }

        /// <summary>
        /// 查询到的新闻公告列表
        /// </summary>
        public PagedList<T_CompanyPost> PostList { get; set; }
    }
}