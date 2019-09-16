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
    /// 物业公告表对应实体类
    /// </summary>
    public class T_Post
    {
        /// <summary>
        /// 公告ID 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 公告提交的服务器时间
        /// </summary>
        public DateTime SubmitTime { get; set; }


        /// <summary>
        /// 公告发布的服务器时间
        /// </summary>
        public DateTime? PublishedTime { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public int SubmitUserId { get; set; }

        /// <summary>
        /// 提交人ID 关联表对象
        /// </summary>
        [ForeignKey("SubmitUserId")]
        public virtual T_PropertyUser SubmitUser { get; set; }

        /// <summary>
        /// 所属物业小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区ID 关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 发布标识  0:未发布 1:已发布
        /// </summary>
        public int PublishedFlag { get; set; }
    }
}
