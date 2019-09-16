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
    /// 任务巡检点关联表实体类
    /// </summary>
    public class R_PlanPoint
    {
        /// <summary>
        /// 主键ID 
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// 关联任务表
        /// </summary>
        [ForeignKey("PlanId")]
        public virtual T_InspectionPlan InspectionPlan { get; set; }

        /// <summary>
        /// 巡检点ID
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// 关联巡检点表
        /// </summary>
        [ForeignKey("PointId")]
        public virtual T_InspectionPoint InspectionPoint { get; set; }

    }
}
