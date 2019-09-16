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
    /// 物业公司公告表实体类
    /// </summary>
    public class T_CompanyPost
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 公告发布的服务器时间
        /// </summary>
        public Nullable<DateTime> PublishedTime { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public int SubmitUserId { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        [ForeignKey("SubmitUserId")]
        public virtual T_CompanyUser SubmitUser { get; set; }

        /// <summary>
        /// 所属物业公司Id
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 所属公司关联表对象
        /// </summary>
        [ForeignKey("CompanyId")]
        public virtual T_Company Company { get; set; }

        /// <summary>
        /// 发布状态 0：未发布 1：已发布
        /// </summary>
        public int PublishStatus { get; set; }

        /// <summary>
        /// 是否公开 0：未公开 1:公开
        /// </summary>
        public int IsOpen { get; set; }

        /// <summary>
        /// 是否删除 0：使用 1：删除
        /// </summary>
        public int DelFlag { get; set; }
    }
}
