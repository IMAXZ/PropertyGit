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
    /// 物业小区贴吧主题表 实体类
    /// </summary>
    public class T_PostBarTopic
    {
        public T_PostBarTopic()
        {
            this.PostBarTopicDiscusss = new HashSet<T_PostBarTopicDiscuss>();
            this.UserPostBarTopics = new HashSet<R_UserPostBarTopic>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 题目
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 发帖人
        /// </summary>
        public int PostUserId { get; set; }

        /// <summary>
        /// 发帖人 关联表对象
        /// </summary>
        [ForeignKey("PostUserId")]
        public virtual T_User PostUser { get; set; }

        /// <summary>
        /// 发帖日期
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// 是否置顶 0:不置顶 1:置顶
        /// </summary>
        public int IsTop { get; set; }

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
        /// 所属贴吧主题分类
        /// </summary>
        public int TopicTypeId { get; set; }

        /// <summary>
        /// 所属贴吧主题分类 关联表对象
        /// </summary>
        [ForeignKey("TopicTypeId")]
        public virtual T_PostBarTopicType PostBarTopicType { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属小区 关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 该物业小区贴吧主题表的所有讨论表
        /// </summary>
        public virtual ICollection<T_PostBarTopicDiscuss> PostBarTopicDiscusss { get; set; }

        /// <summary>
        /// 该物业小区贴吧主题被收藏的关联数据
        /// </summary>
        public virtual ICollection<R_UserPostBarTopic> UserPostBarTopics { get; set; }
    }
}
