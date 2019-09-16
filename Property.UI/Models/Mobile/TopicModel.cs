using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    public class PostTopicModel : TokenModel
    {
        /// <summary>
        /// 物业小区Id
        /// </summary>
        public int PropertyPlaceId { get; set; }
        /// <summary>
        /// 话题类型Id
        /// </summary>
        public int TopicTypeId { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 图片列表
        /// </summary>
        public string PicList { get; set; }
    }

    public class ReplyTopicModel : TokenModel
    {
        /// <summary>
        /// 话题Id
        /// </summary>
        public int Topicid { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 图片列表
        /// </summary>
        public string PicList { get; set; }
        /// <summary>
        /// 回复人Id
        /// </summary>
        public int ReplyId { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 物业小区Id
        /// </summary>
        public int PropertyPlaceId { get; set; }
    }

    public class CollectTopicModel : TokenModel
    {
        public int TopicId { get; set; }
    }

    public class TopicDetailsModel : PagedSearchModel
    {
        public int TopicId { get; set; }
    }

    public class AllTopicPagedSearchModel : PagedSearchModel
    {
        public int PropertyPlaceId { get; set; }
        public int TopicTypeId { get; set; }
    }


    /// <summary>
    /// 帖子分类查询模型
    /// </summary>
    public class TopicSortSearchModel : TokenModel
    {
        /// <summary>
        /// 小区Id
        /// </summary>
        public int PropertyPlaceId { get; set; }
    }

    /// <summary>
    /// 回复的一二级模型
    /// </summary>
    public class ReplyOneTwoLevelModel : TokenModel
    {
        public int Id { get; set; }
    }

    public class Level2RepliedListModel : PagedSearchModel
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
    }

    public class DeleteTopicModel : TokenModel
    {
        public int TopicId { get; set; }
    }

    public class DeleteReplyModel : TokenModel
    {
        public int Id { get; set; }
    }
}