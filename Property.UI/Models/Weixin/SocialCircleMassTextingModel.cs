using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 群发消息模型
    /// </summary>
    public class SocialCircleMassTextingModel
    {
        /// <summary>
        /// 群发消息ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 群发内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 群发图片
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 群发时间
        /// </summary>
        public string ChatTime { get; set; }

        /// <summary>
        /// 上次群发时间
        /// </summary>
        public string LastTime { get; set; }

        /// <summary>
        /// 群发人的姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 群发人头像
        /// </summary>
        public string HeadPath { get; set; }

        /// <summary>
        /// 所选群发成员
        /// </summary>
        public string MemberIds { get; set; }

        /// <summary>
        /// 群发人员名称集合
        /// </summary>
        public string MemberNames { get; set; }

        /// <summary>
        /// 群发成员个数
        /// </summary>
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// 群发
    /// </summary>
    public class UserListSocialCircleMassTextingModel
    {
        /// <summary>
        /// 圈子Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 圈子创建人姓名
        /// </summary>
        public string CreaterName { get; set; }

        /// <summary>
        /// 圈子创建人头像路径
        /// </summary>
        public string CreaterHeadPath { get; set; }

        /// <summary>
        /// 群发成员个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 所选群发成员
        /// </summary>
        public string MemberIds { get; set; }

        /// <summary>
        /// 群发人员
        /// </summary>
        public string NameList { get; set; }
    }
}