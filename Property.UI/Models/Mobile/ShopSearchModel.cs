using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 门店查询模型
    /// </summary>
    public class ShopSearchModel : PagedSearchModel
    {
        /// <summary>
        /// 门店类型 0：绿色直供 1：开锁服务 2：生活小卖店 3：五金用品
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 商家运费查询模型
    /// </summary>
    public class ShopFreightSearchModel : DetailSearchModel
    {
        /// <summary>
        /// 商品总价
        /// </summary>
        public double TotalPrice { get; set; }
    }
}