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
    /// 商家运费设置表 实体类
    /// </summary>
    public class T_ShopShippingCost
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 订单消费金额
        /// </summary>
        public Nullable<double> OrderExpense { get; set; }

        /// <summary>
        /// 收费
        /// </summary>
        public Nullable<double> Price { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 是否免运费
        /// </summary>
        public int IsFree { get; set; }

        /// <summary>
        /// 所属商家 关联表对象
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual T_Shop Shop { get; set; }
    }
}
