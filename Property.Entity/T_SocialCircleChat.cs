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
    /// 小区业主圈子聊天对话实体类
    /// </summary>
    public class T_SocialCircleChat
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 聊天内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 聊天图片
        /// </summary>
        [MaxLength(200)]
        public string Img { get; set; }

        /// <summary>
        /// 聊天人ID
        /// </summary>
        public int ChatUserId { get; set; }

        /// <summary>
        /// 聊天人
        /// </summary>
        [ForeignKey("ChatUserId")]
        public virtual T_User ChatUser { get; set; }

        /// <summary>
        /// 聊天时间
        /// </summary>
        public DateTime ChatTime { get; set; }

        /// <summary>
        /// 所属圈子ID
        /// </summary>
        public int SocialCircleId { get; set; }

        /// <summary>
        /// 所属圈子
        /// </summary>
        [ForeignKey("SocialCircleId")]
        public virtual T_SocialCircle SocialCircle { get; set; }
    }
}
