using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 收货地址模型
    /// </summary>
    public class ShippingAddressModel : TokenModel
    {
        /// <summary>
        /// 收货地址ID
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ShipperName { get; set; }

        /// <summary>
        /// 性别: 0男 1女
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 收货人联系方式
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 县区ID
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetails { get; set; }
    }

    /// <summary>
    /// 收货地址根据ID操作模型
    /// </summary>
    public class ShippingAddressIDModel : TokenModel
    {
        /// <summary>
        /// 收货地址ID
        /// </summary>
        public int AddressId { get; set; }
    }
}