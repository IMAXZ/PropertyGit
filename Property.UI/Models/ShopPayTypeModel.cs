using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    public class ShopPaymentType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        //1 微信支付 2 支付宝支付 3 现金支付
        public int TypeFlag { get; set; }
    }

    /// <summary>
    /// 商家支付类型模型
    /// </summary>
    public class ShopPayTypeModel
    {
        public int ShopId { get; set; }
        public T_Shop Shop { get; set; }
        public List<ShopPaymentType> PayTypeList { get; set; }
        public List<int> PayTypeIds { get; set; }
    }

    public class ShopPayTypeSetupModel
    {
        /// <summary>
        /// 商家id
        /// </summary>
        public int shopId { get; set; }

        /// <summary>
        /// 该商家要设置的支付类型
        /// </summary>
        public int[] ids { get; set; }
    }
}