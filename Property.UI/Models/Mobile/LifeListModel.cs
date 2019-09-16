using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 生活记账详细模型
    /// </summary>
    public class LifeBillDescModel
    {
        /// <summary>
        /// 记账Id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
    }

    /// <summary>
    /// 所有的生活记账模型
    /// </summary>
    public class AllLifeBillList
    {
        /// <summary>
        /// 生活记账详细列表
        /// </summary>
        public List<LifeBillDescModel> AccountList { get; set; }

        /// <summary>
        /// 记账时间
        /// </summary>
        public string Time { get; set; }
    }
}