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
    /// 物业总公司操作日志表 实体类
    /// </summary>
    public class T_CompanyOpreateLog
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OpreateTime { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int OpreaterId { get; set; }

        /// <summary>
        /// 物业用户关联表对象
        /// </summary>
        [ForeignKey("OpreaterId")]
        public virtual T_CompanyUser Opreater { get; set; }

        /// <summary>
        /// 执行的操作
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Action { get; set; }

        /// <summary>
        /// 具体描述
        /// </summary>
        [MaxLength(1000)]
        public string Desc { get; set; }
    }
}
