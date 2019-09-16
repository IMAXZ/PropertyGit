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
    /// 意见反馈表 实体类
    /// </summary>
    public class T_Feedback
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 提报时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [MaxLength(200)]
        public string Img { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 所属业主用户Id关联表对象
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User User { get; set; }
    }
}
