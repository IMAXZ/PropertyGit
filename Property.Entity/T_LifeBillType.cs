using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 生活记账分类表 实体类
    /// </summary>
    public class T_LifeBillType
    {
        public T_LifeBillType()
        {
            this.LifeBills = new HashSet<T_LifeBill>();
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
        /// 该分类下的所有生活记账
        /// </summary>
        public virtual ICollection<T_LifeBill> LifeBills { get; set; }
    }
}
