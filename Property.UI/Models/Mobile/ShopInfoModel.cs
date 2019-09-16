using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    public class ShopInfoModel : TokenModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopContent { get; set; }
        public string MainSale { get; set; }
        public int StartBusinessTime { get; set; }
        public int EndBusinessTime { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ShopImage { get; set; }
    }

    public class ShopImageModel : TokenModel
    {
        public int ShopId { get; set; }
        public string ShopImage { get; set; }
    }

    public class ShopUserImageModel : TokenModel
    {
        public string UserImage { get; set; }
    }

    public class ShopPayTypeModel : TokenModel
    {
        /// <summary>
        /// 商家id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 该商家要设置的支付类型
        /// </summary>
        public string PayTypeIds { get; set; }
    }

    public class ShopShippingCostModel : TokenModel
    {
        public int? Id { get; set; }
        public int ShopId { get; set; }
        public double? OrderExpense { get; set; }
        public double? Price { get; set; }
        public int IsFree { get; set; }
    }

    public class ShopUserInfoModel : TokenModel
    {
        public string TrueName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserImg { get; set; }
    }
}