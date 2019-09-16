using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 生活记账支付方式表 实体类
    /// </summary>
    public class T_LifePayType
    {
        public T_LifePayType()
        {
            this.LifeBills = new HashSet<T_LifeBill>();
        }

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Img { get; set; }

        /// <summary>
        /// 删除标示 0.未删除 1.删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 改支付方式下的所有记账
        /// </summary>
        public virtual ICollection<T_LifeBill> LifeBills { get; set; }
    }
}
