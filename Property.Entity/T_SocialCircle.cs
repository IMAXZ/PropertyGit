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
    /// 小区业主圈子数据实体类
    /// </summary>
    public class T_SocialCircle
    {
        public T_SocialCircle() 
        {
            this.UserSocialCircles = new HashSet<R_UserSocialCircle>();
            this.SocialCircleMassTextings = new HashSet<T_SocialCircleMassTexting>();
            this.SocialCircleChats = new HashSet<T_SocialCircleChat>();
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 圈子名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 圈子介绍
        /// </summary>
        [MaxLength(200)]
        public string Content { get; set; }

        /// <summary>
        /// 圈子头像图片路径
        /// </summary>
        [MaxLength(200)]
        public string HeadImgPath { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreaterId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ForeignKey("CreaterId")]
        public virtual T_User Creater { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 所属小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        [ForeignKey("PlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 该圈子成员关联数据
        /// </summary>
        public virtual ICollection<R_UserSocialCircle> UserSocialCircles { get; set; }

        /// <summary>
        /// 该圈子下关联群发记录数据
        /// </summary>
        public virtual ICollection<T_SocialCircleMassTexting> SocialCircleMassTextings { get; set; }

        /// <summary>
        /// 该圈子所有聊天记录
        /// </summary>
        public virtual ICollection<T_SocialCircleChat> SocialCircleChats { get; set; }
    }
}
