using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class TopicFloorDetailModel
    {
        /// <summary>
        /// 当前楼层Id(也是是一级回复的Id)
        /// </summary>
        public int FloorId { get; set; }
        /// <summary>
        /// 当前楼层号
        /// </summary>
        public string FloorNo { get; set; }
        /// <summary>
        /// 当前一级发布人Id
        /// </summary>
        public int PostUserId { get; set; }
        /// <summary>
        /// 当前一级发布人用户名
        /// </summary>
        public string PostUserName { get; set; }
        /// <summary>
        /// 当前一级发布人头像
        /// </summary>
        public string PostUserHeadPath { get; set; }
        /// <summary>
        /// 当前一级发布的日期
        /// </summary>
        public string PostDate { get; set; }
        /// <summary>
        /// 当前一级发布的内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 当前一级的图片列表
        /// </summary>
        public string ImgPath { get; set; }
        /// <summary>
        /// 当前一级的缩略图列表
        /// </summary>
        public string ImgThumbnail { get; set; }
        /// <summary>
        /// 当前一级下面所有二级回复的数
        /// </summary>
        public int LevelTwoReplyCount { get; set; }
        /// <summary>
        /// 小区Id
        /// </summary>
        public int PropertyPlaceId { get; set; }
        /// <summary>
        /// 主题Id
        /// </summary>
        public int TopicId { get; set; }
        /// <summary>
        /// 准备要回复人的Id
        /// </summary>
        public int ReplyId { get; set; }
        /// <summary>
        /// 准备要回复人的用户名
        /// </summary>
        public string ReplidUserName { get; set; }
        /// <summary>
        /// 准备要回复人的内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 当前的登录用户Id
        /// </summary>
        public int CurrentUserId { get; set; }
        /// <summary>
        /// 能否删除
        /// </summary>
        public bool CanDelete { get; set; }
    }
}