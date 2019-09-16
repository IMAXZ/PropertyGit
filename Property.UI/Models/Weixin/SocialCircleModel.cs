using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 圈子模型
    /// </summary>
    public class SocialCircleModel
    {
        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 绑定的小区下拉列表
        /// </summary>
        public List<SelectListItem> PlaceList { get; set; }

        /// <summary>
        /// 圈子名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 圈子内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 圈子头像
        /// </summary>
        public string HeadImg { get; set; }
    }

    /// <summary>
    /// 业主圈子列表Item模型
    /// </summary>
    public class SocialCircleItemModel
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
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 圈子头像路径
        /// </summary>
        public string HeadPath { get; set; }

        /// <summary>
        /// 最新聊天时间
        /// </summary>
        public string NewestChatTime { get; set; }

        /// <summary>
        /// 是否申请过
        /// </summary>
        public bool IsApplyed { get; set; }
    }

    /// <summary>
    /// 群发消息圈子列表Item模型
    /// </summary>
    public class ReceiveMassTextSocialCircleItemModel : SocialCircleItemModel 
    {
        /// <summary>
        /// 该圈子未读消息个数
        /// </summary>
        public int NoReadMassCount { get; set; }

        /// <summary>
        /// 改圈子所有消息个数
        /// </summary>
        public int TotalMassCount { get; set; }
    }

    /// <summary>
    /// 我的圈子列表模型
    /// </summary>
    public class MySocialCircleListModel
    {
        /// <summary>
        /// 最新一条申请信息
        /// </summary>
        public ApplyInfoModel ApplyInfo { get; set; }

        /// <summary>
        /// 未处理申请个数
        /// </summary>
        public int NoDealCount { get; set; }

        /// <summary>
        /// 群发消息未读个数
        /// </summary>
        public int NoReadCount { get; set; }

        /// <summary>
        /// 最新一条群发消息说明
        /// </summary>
        public MassTextingModel NewsetMass { get; set; }

        /// <summary>
        /// 我创建的圈子
        /// </summary>
        public List<SocialCircleItemModel> CreateList { get; set; }

        /// <summary>
        /// 我加入的圈子
        /// </summary>
        public List<SocialCircleItemModel> JoinList { get; set; }

        /// <summary>
        /// 我创建的圈子个数
        /// </summary>
        public int CreateCount { get; set;}

        /// <summary>
        /// 我创建的圈子个数
        /// </summary>
        public int JoinCount { get; set; }
    }

    /// <summary>
    /// 最新一条申请信息
    /// </summary>
    public class ApplyInfoModel
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyUserName { get; set; }

        /// <summary>
        /// 要加入的圈子名称
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplyTime { get; set; }
    }

    /// <summary>
    /// 接收到的群发消息模型
    /// </summary>
    public class MassTextingModel 
    {
        /// <summary>
        /// 所属圈子名称
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// 群发消息发送时间
        /// </summary>
        public string MassSendTime { get; set; }
    }

    /// <summary>
    /// 编辑名称模型
    /// </summary>
    public class EditNameModel
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 圈子名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 编辑内容模型
    /// </summary>
    public class EditContentModel
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 圈子介绍
        /// </summary>
        public string Content { get; set; }
    }

    public class circleuserModel : CircleUserModel
    {

        /// <summary>
        /// 成员个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 创建人的用户名
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 创建人的头像
        /// </summary>
        public string HeadImg { get; set; }
    }

    /// <summary>
    /// 圈子成员列表
    /// </summary>
    public class CircleUserModel
    {
        /// <summary>
        /// 当前圈子的Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 成员Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 当前用户的Id
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        /// 创建人的Id
        /// </summary>
        public int CreateId { get; set; }

        /// <summary>
        /// 成员头像
        /// </summary>
        public string UserImg { get; set; }

        /// <summary>
        /// 成员用户名
        /// </summary>
        public string UserName { get; set; }
    }
}