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
    /// 小区业主圈子群发记录实体类
    /// </summary>
    public class T_SocialCircleMassTexting
    {
        public T_SocialCircleMassTexting()
        {
            this.UserSocialCircleMassTextings = new HashSet<R_UserSocialCircleMassTexting>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 群发内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 群发图片
        /// </summary>
        [MaxLength(100)]
        public string Img { get; set; }

        /// <summary>
        /// 群发时间
        /// </summary>
        public DateTime ChatTime { get; set; }

        /// <summary>
        /// 所属圈子
        /// </summary>
        public int SocialCircleId { get; set; }

        /// <summary>
        /// 所属圈子关联表对象
        /// </summary>
        [ForeignKey("SocialCircleId")]
        public virtual T_SocialCircle SocialCircle { get; set; }

        /// <summary>
        ///该业主圈子群发记录成员关联表
        /// </summary>
        public virtual ICollection<R_UserSocialCircleMassTexting> UserSocialCircleMassTextings { get; set; }
    }
}
