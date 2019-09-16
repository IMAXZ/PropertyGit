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
    /// 物业小区贴吧主题讨论表 实体类
    /// </summary>
    public class T_PostBarTopicDiscuss
    {
        public T_PostBarTopicDiscuss()
        {
            this.PostBarTopicDiscusses = new HashSet<T_PostBarTopicDiscuss>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [MaxLength(200)]
        public string ImgPath { get; set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        [MaxLength(200)]
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 回复人ID
        /// </summary>
        public int PostUserId { get; set; }

        /// <summary>
        /// 回复人
        /// </summary>
        [ForeignKey("PostUserId")]
        public virtual T_User PostUser { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime PostTime { get; set; }

        /// <summary>
        /// 回复谁
        /// </summary>
        public int ReplyId { get; set; }

        /// <summary>
        /// 回复谁Id关联表对象
        /// </summary>
        [ForeignKey("ReplyId")]
        public virtual T_User ReplyUser { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public Nullable<int> ParentId { get; set; }

        /// <summary>
        /// 父级Id关联表对象
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual T_PostBarTopicDiscuss ParentReply { get; set; }

        /// <summary>
        /// 所属主题
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// 所属主题Id关联表对象
        /// </summary>
        [ForeignKey("TopicId")]
        public virtual T_PostBarTopic PostBarTopic { get; set; }

        public virtual ICollection<T_PostBarTopicDiscuss> PostBarTopicDiscusses { get; set; }
    }
}
