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
    /// 巡检点表实体类
    /// </summary>
    public class T_InspectionPoint
    {
        public T_InspectionPoint() 
        {
            this.PlanPoints = new HashSet<R_PlanPoint>();
        }

        /// <summary>
        /// 巡检点ID：主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PointName { get; set; }

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
        /// 所属巡检类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 所属巡检类别 关联表对象
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual T_InspectionCategory InspectionCategory { get; set; }

        /// <summary>
        /// 包含该巡检点的所有巡检任务关联
        /// </summary>
        public virtual ICollection<R_PlanPoint> PlanPoints { get; set; }
    }
}
