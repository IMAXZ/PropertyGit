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
    /// 物业小区缴费分类表对应实体类
    /// </summary>
    public class T_PropertyExpenseType
    {
        public T_PropertyExpenseType()
        {
            this.PropertyExpenseNos = new HashSet<T_PropertyExpenseNo>();
            this.HouseUserExpenseTemplates = new HashSet<T_HouseUserExpenseTemplate>();
            this.HouseUserExpenseDetails = new HashSet<T_HouseUserExpenseDetails>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 是否固定 0:非固定缴费类型 1:固定缴费类型
        /// </summary>
        public int IsFixed { get; set; }

        /// <summary>
        /// 所属物业小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区关联对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 该缴费分类下的所有缴费编号关联对象
        /// </summary>
        public virtual ICollection<T_PropertyExpenseNo> PropertyExpenseNos { get; set; }

        /// <summary>
        /// 该缴费分类下的所有缴费模板关联对象
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseTemplate> HouseUserExpenseTemplates { get; set; }

        /// <summary>
        /// 该缴费分类下的所有缴费明细关联对象
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseDetails> HouseUserExpenseDetails { get; set; }
    }
}
