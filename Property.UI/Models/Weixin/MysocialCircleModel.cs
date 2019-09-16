using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 我的圈子验证模型
    /// </summary>
    public class MySocialCircleModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 我的圈子名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 申请人头像
        /// </summary>
        public string HeadPath { get; set; }
        /// <summary>
        /// 申请人圈子id
        /// </summary>
        public int SocialCircleId { get; set; }

        /// <summary>
        /// 申请状态 0:未通过 1:通过 2:驳回 3：已退出
        /// </summary>
        public int ApplyStatus { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
    }
    public class MyListSocialCircleModel:SearchModel
    {
        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public List<MySocialCircleModel> MyListSocialCircle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PagedList<R_UserSocialCircle> DataList { get; set; }
    }
}