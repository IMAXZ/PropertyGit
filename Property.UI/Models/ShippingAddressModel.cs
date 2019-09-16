using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// App注册用户收货地址查询模型
    /// </summary>
    public class ShippingAddressSearchModel : SearchModel
    {
        /// <summary>
        /// 查询 地址：0：全部 1：默认
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// 是否默认下拉列表
        /// </summary>
        public List<SelectListItem> IsDefaultList { get; set; }

        /// <summary>
        /// 获取到的数据结果列表
        /// </summary>
        public PagedList<T_AppUserShippingAddress> ResultList { get; set; }
    }
}