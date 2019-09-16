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
    /// 用户帖子收藏关联表
    /// </summary>
    public class R_UserPostBarTopic
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 外键 APP用户
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User AppUser { get; set; }

        /// <summary>
        /// 帖子主题ID
        /// </summary>
        public int PostBarTopicId { get; set; }

        /// <summary>
        /// 外键 帖子主题
        /// </summary>
        [ForeignKey("PostBarTopicId")]
        public virtual T_PostBarTopic PostBarTopic { get; set; }
    }
}
