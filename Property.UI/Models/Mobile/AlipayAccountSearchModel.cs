using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 支付宝账户查询模型
    /// </summary>
    public class AlipayAccountSearchModel : DetailSearchModel
    {
        /// <summary>
        /// 类型 0:物业账户 1:商家账户
        /// </summary>
        public int Type { get; set; }
    }
}