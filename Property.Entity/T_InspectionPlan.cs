using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 巡检任务计划表实体对象
    /// </summary>
    public class T_InspectionPlan
    {
        public T_InspectionPlan() 
        {
            this.PlanPoints = new HashSet<R_PlanPoint>();
            this.TimePlans = new HashSet<T_InspectionTimePlan>();
        }
        /// <summary>
        /// 巡检任务ID：主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 巡检任务名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PlanName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 巡检类型：0：日巡检 1：周巡检  2：月巡检
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 是否随机 0：不随机 1：随机
        /// </summary>
        public int IsRandom { get; set; }

        /// <summary>
        /// 巡检次数  日（月)最多10次  周：最多7次
        /// </summary>
        public int Number { get; set; }

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
        /// 发布标识  0:未发布 1:已发布
        /// </summary>
        public int PublishedFlag { get; set; }

        /// <summary>
        /// 该巡检计划所有巡检点关联
        /// </summary>
        public virtual ICollection<R_PlanPoint> PlanPoints { get; set; }

        /// <summary>
        /// 该巡检的巡检时间安排
        /// </summary>
        public virtual ICollection<T_InspectionTimePlan> TimePlans { get; set; }
    }
}
