using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class TopicDetailModel
    {
        /// <summary>
        /// 主题Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发主题的用户Id
        /// </summary>
        public int PostUserId { get; set; }
        /// <summary>
        /// 发主题的用户名
        /// </summary>
        public string PostUserName { get; set; }
        /// <summary>
        /// 发主题的用户头像
        /// </summary>
        public string PostUserHeadPath { get; set; }
        /// <summary>
        /// 显示主题的时间
        /// </summary>
        public string PostDate { get; set; }
        /// <summary>
        /// 此主题用到的图片列表
        /// </summary>
        public string TopicImgPath { get; set; }
        /// <summary>
        /// 小区Id
        /// </summary>
        public int PropertyPlaceId { get; set; }
        /// <summary>
        /// 一级回复的统计
        /// </summary>
        public int LevelOneReplyCount { get; set; }
        /// <summary>
        /// 当前用户Id
        /// </summary>
        public int CurrentUserId { get; set; }
        /// <summary>
        /// 回复主题的内容（主题的一级回复）
        /// </summary>
        public string ReplyTopicContent { get; set; }
        /// <summary>
        /// 回复主题的图片列表（主题的一级回复）
        /// </summary>
        public string ReplyTopicImgList { get; set; }
        /// <summary>
        /// 0表示所有楼层都显示 其它Id代表具体那楼层Id。
        /// </summary>
        public int FloorId { get; set; }
        /// <summary>
        /// 0：不置顶，1：置顶。
        /// </summary>
        public int IsTop { get; set; }
    }
}