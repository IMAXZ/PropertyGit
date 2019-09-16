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
    /// 巡检分类表实体类
    /// </summary>
    public class T_InspectionCategory
    {
        public T_InspectionCategory() 
        {
            this.InspectionPoints = new HashSet<T_InspectionPoint>();
        }

        /// <summary>
        /// 类别ID：主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Memo { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 该巡检类别下所有巡检点
        /// </summary>
        public virtual ICollection<T_InspectionPoint> InspectionPoints { get; set; }
    }
}
