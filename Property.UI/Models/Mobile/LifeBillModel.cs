using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 记账明细模型
    /// </summary>
    public class LifeBillModel : TokenModel
    {
        /// <summary>
        /// 记账Id
        /// </summary>
        public int Id { get; set; }
    }

    /// <summary>
    /// 添加记账模型
    /// </summary>
    public class AddLifeBillModel : TokenModel
    {
        /// <summary>
        /// 分类Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 支付方式Id
        /// </summary>
        public int PayId { get; set; }

        /// <summary>
        /// 记账日期
        /// </summary>
        public string PayDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string DateStr { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
    }

    /// <summary>
    /// 编辑记账模型
    /// </summary>
    public class EditLifeBillModel : AddLifeBillModel
    {
        /// <summary>
        /// 记账Id
        /// </summary>
        public int Id { get; set; }
    }
}