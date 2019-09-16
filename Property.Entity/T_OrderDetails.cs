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
    /// 订单详细表 实体类
    /// </summary>
    public class T_OrderDetails
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public int ShopSaleId { get; set; }

        /// <summary>
        /// 商品 关联表对象
        /// </summary>
        [ForeignKey("ShopSaleId")]
        public virtual T_ShopSale ShopSale { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int SaledAmount { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 所属订单
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 所属订单 关联表对象
        /// </summary>
        [ForeignKey("OrderId")]
        public virtual T_Order Order { get; set; }
    }
}
