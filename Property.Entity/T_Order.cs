using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 订单表 实体类
    /// </summary>
    public class T_Order
    {
        public T_Order()
        {
            this.OrderDetails = new HashSet<T_OrderDetails>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单价格
        /// </summary>
        public double OrderPrice { get; set; }

        /// <summary>
        /// 下订单者
        /// </summary>
        public int AppUserId { get; set; }

        /// <summary>
        /// 下订单者 关联表对象
        /// </summary>
        [ForeignKey("AppUserId")]
        public virtual T_User User { get; set; }

        /// <summary>
        /// 商家Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 商家Id 关联表对象
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual T_Shop Shop { get; set; }

        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        [MaxLength(64)]
        public string PayTradeNo { get; set; }

        /// <summary>
        /// 订单状态 0.待付款 1.待确认 2.待收货 3.已退单 4.交易完毕 5.交易关闭
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 退单种类 0：商家退单 1：系统退单 2：退单退款
        /// </summary>
        public Nullable<int> RecedeType { get; set; }

        /// <summary>
        /// 支付宝退单结果
        /// </summary>
        [MaxLength(50)]
        public string RefundResult { get; set; }

        /// <summary>
        /// 退单时间
        /// </summary>
        public Nullable<DateTime> RecedeTime { get; set; }

        /// <summary>
        /// 交易完成时间
        /// </summary>
        public Nullable<DateTime> CompleteTime { get; set; }

        /// <summary>
        /// 商家退单原因
        /// </summary>
        [MaxLength(100)]
        public string Reason { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        public Nullable<DateTime> PayDate { get; set; }

        /// <summary>
        /// 支付方式 0：默认未支付 1:微信在线支付 2:支付宝在线支付 3:货到现金付款 4:货到微信付款 5:货到支付宝付款
        /// </summary>
        public int PayWay { get; set; }

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
        /// 删除标识 0:未删除 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 是否用户删除 0:未删除 1:删除
        /// </summary>
        public int IsUserHided { get; set; }

        /// <summary>
        /// 是否商家删除 0:未删除 1:删除
        /// </summary>
        public int IsStoreHided { get; set; }

        /// <summary>
        /// 收货地址ID 关联表对象
        /// </summary>
        [ForeignKey("ShipAddressId")]
        public virtual T_AppUserShippingAddress ShippingAddress { get; set; }

        /// <summary>
        /// 该订单表的所有订单详细
        /// </summary>
        public virtual ICollection<T_OrderDetails> OrderDetails { get; set; }
    }
}
