using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 圈子聊天模型
    /// </summary>
    public class SocialCircleChatModel
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 圈子名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 圈子成员个数
        /// </summary>
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// 聊天记录模型
    /// </summary>
    public class SocialCircleChatRecordModel 
    {
        /// <summary>
        /// 聊天人
        /// </summary>
        public string ChatUser { get; set; }

        /// <summary>
        /// 聊天用户头像
        /// </summary>
        public string ChatUserHeadImg { get; set; }

        /// <summary>
        /// 聊天时间
        /// </summary>
        public string ChatTime { get; set; }

        /// <summary>
        /// 上次聊天时间
        /// </summary>
        public string LastChatTime { get; set; }

        /// <summary>
        /// 聊天内容
        /// </summary>
        public string ChatContent { get; set; }

        /// <summary>
        /// 聊天图路径
        /// </summary>
        public string ChatImg { get; set; }

        /// <summary>
        /// 是否自己
        /// </summary>
        public bool IsMySelf { get; set; }
    }
}