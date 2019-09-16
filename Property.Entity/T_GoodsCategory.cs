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
    /// 促销商品分类表 实体类
    /// </summary>
    public class T_GoodsCategory
    {
        public T_GoodsCategory() 
        {
            this.ShopSales = new HashSet<T_ShopSale>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 所属商家 
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 所属商家 关联表对象
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual T_Shop Shop { get; set; }

        /// <summary>
        /// 该商品类别下的所有商品
        /// </summary>
        public virtual ICollection<T_ShopSale> ShopSales { get; set; }
    }
}
