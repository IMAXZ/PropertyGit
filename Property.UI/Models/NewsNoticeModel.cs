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
    /// 新闻公告模型
    /// </summary>
    public class NewsNoticeModel
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
        /// 提交人
        /// </summary>
        public int SubmitUserId { get; set; }

        /// <summary>
        /// 公告提交的服务器时间
        /// </summary>
        public string SubmitTime { get; set; }

        /// <summary>
        /// 公告发布的服务器时间
        /// </summary>
        public string PuslishedTime { get; set; }

        /// <summary>
        /// 提交人名称
        /// </summary>
        public string SubmitUser { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区名称
        /// </summary>
        public string PropertyPlaceName { get; set; }

        /// <summary>
        /// 提交人头像
        /// </summary>
        public string SubmitUserHeadPath { get; set; }

        /// <summary>
        /// 发布标识  0:未发布 1:已发布
        /// </summary>
        public bool PublishedFlag { get; set; }
    }


    /// <summary>
    /// 新闻公告搜索模型
    /// </summary>
    public class NewsNoticeSearchModel : SearchModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 发布标识  0:未发布 1:已发布
        /// </summary>
        public int? PublishedFlag { get; set; }

        /// <summary>
        /// 公告状态下拉列表
        /// </summary>
        public List<SelectListItem> StatueList { get; set; }

        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public PagedList<T_Post> PostList { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int? PropertyPlaceId { get; set; }
        public List<SelectListItem> PropertyPlaceList { get; set; }

        /// <summary>
        /// 所属物业小区名称
        /// </summary>
        public string PropertyPlaceName { get; set; }

    }
}