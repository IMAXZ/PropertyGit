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
    /// 巡检时间安排
    /// </summary>
    public class T_InspectionTimePlan
    {
        public T_InspectionTimePlan() 
        {
            this.InspectionResults = new HashSet<T_InspectionResult>();
        }

        /// <summary>
        /// 巡检时间安排ID：主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 巡检任务ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// 关联的巡检任务对象
        /// </summary>
        [ForeignKey("PlanId")]
        public virtual T_InspectionPlan InspectionPlan { get; set; }

        /// <summary>
        /// 次号，第几次
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 开始天
        /// </summary>
        public int BeginNum { get; set; }

        /// <summary>
        /// 结束天
        /// </summary>
        public int EndNum { get; set; }

        /// <summary>
        /// 该巡检安排的所有巡检结果
        /// </summary>
        public virtual ICollection<T_InspectionResult> InspectionResults { get; set; }
    }
}
