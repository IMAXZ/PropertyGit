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
    /// 门店促销消息表对应实体类
    /// </summary>
    public class T_ShopSale
    {
        public T_ShopSale()
        {
            this.OrderDetails = new HashSet<T_OrderDetails>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 促销图片路径
        /// </summary> 
        public string ImgPath { get; set; }

        /// <summary>
        /// 促销缩略图路径
        /// </summary> 
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 商品分类ID
        /// </summary>
        public int GoodsCategoryId { get; set; }

        /// <summary>
        /// 所属商品分类
        /// </summary>
        [ForeignKey("GoodsCategoryId")]
        public virtual T_GoodsCategory GoodsCategory { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainingAmout { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 是否销售中 0已经下架  1销售中
        /// </summary>
        public int InSales { get; set; }

        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime? UnShelveTime { get; set; }

        /// <summary>
        /// 拥有该促销商品的所有订单详细
        /// </summary>
        public virtual ICollection<T_OrderDetails> OrderDetails { get; set; }
    }
}
