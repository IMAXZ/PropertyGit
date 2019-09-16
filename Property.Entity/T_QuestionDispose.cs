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
    /// 处理问题表 实体类
    /// </summary>
    public class T_QuestionDispose
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
        /// 问题Id
        /// </summary>
        public int QuestionId { get; set; } 

        /// <summary>
        /// 外键 问题表
        /// </summary>
        [ForeignKey("QuestionId")]
        public virtual T_Question Question { get; set; } 

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
