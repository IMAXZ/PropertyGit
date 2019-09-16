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
    /// 巡检异常处理实体类
    /// </summary>
    public class T_InspectionExceptionDispose
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 处理描述
        /// </summary>
        [MaxLength(1000)]
        public string DisposeDesc { get; set; }

        /// <summary>
        /// 巡检异常结果ID
        /// </summary>
        public int ExceptionResultId { get; set; }

        /// <summary>
        /// 外键 巡检结果表
        /// </summary>
        [ForeignKey("ExceptionResultId")]
        public virtual T_InspectionResult Result { get; set; }

        /// <summary>
        /// 处理人Id
        /// </summary>
        public int DisposeUserId { get; set; }

        /// <summary>
        /// 外键 物业员工表
        /// </summary>
        [ForeignKey("DisposeUserId")]
        public virtual T_PropertyUser DisposeUser { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime DisposeTime { get; set; }
    }
}
