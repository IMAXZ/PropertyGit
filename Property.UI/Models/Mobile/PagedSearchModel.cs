using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 分页查询模型
    /// </summary>
    public class PagedSearchModel : TokenModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
    }
}