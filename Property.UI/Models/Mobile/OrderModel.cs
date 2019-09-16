using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 订单模型
    /// </summary>
    public class OrderModel : TokenModel
    {
        /// <summary>
        /// 订单价格
        /// </summary>
        public double OrderPrice { get; set; }

        /// <summary>
        /// 商家Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 收货地址ID
        /// </summary>
        public int ShipAddressId { get; set; }

        /// <summary>
        /// 订单添加的商品列表
        /// </summary>
        public List<OrderGoodsModel> OrderGoods { get; set; }
    }

    /// <summary>
    /// 订单模型
    /// </summary>
    public class OrderEditModel : TokenModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        public double OrderPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 收货地址ID
        /// </summary>
        public int ShipAddressId { get; set; }

        /// <summary>
        /// 订单添加的商品列表
        /// </summary>
        public List<OrderGoodsModel> OrderGoods { get; set; }
    }

    /// <summary>
    /// 订单中商品模型
    /// </summary>
    public class OrderGoodsModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int SaleId { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int SaledAmount { get; set; }
    }

    /// <summary>
    /// 订单分类分页查询模型
    /// </summary>
    public class OrderPagedSearchModel : PagedSearchModel
    {
        /// <summary>
        /// 如果为空：获取全部  0.待付款 1.待确认 2.待收货 3.已退单 4.交易完毕
        /// </summary>
        public int? OrderStatus { get; set; }
    }

    /// <summary>
    /// 订单分类分页查询模型
    /// </summary>
    public class OrderUpdateStatusModel : TokenModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 2.待收货 3.已退单 4.交易完毕
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 修改原因（退单）
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 订单支付模型
    /// </summary>
    public class OrderPayModel : TokenModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 支付方式 1:微信在线支付 2:支付宝在线支付 3:货到现金付款 4:货到微信付款 5:货到支付宝付款
        /// </summary>
        public int PayWay { get; set; }

        /// <summary>
        /// 支付宝交易订单号
        /// </summary>
        public string AlipayTradeNo { get; set; }
    }
}