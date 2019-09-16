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
    /// 用户加入圈子关联数据实体类
    /// </summary>
    public class R_UserSocialCircle
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 申请加入的用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User ApplyUser { get; set; }

        /// <summary>
        /// 申请加入的圈子ID
        /// </summary>
        public int SocialCircleId { get; set; }

        /// <summary>
        /// 申请加入的圈子
        /// </summary>
        [ForeignKey("SocialCircleId")]
        public virtual T_SocialCircle SocialCircle { get; set; }

        /// <summary>
        /// 申请状态 0:未通过 1:通过 2:驳回 3：已退出
        /// </summary>
        public int ApplyStatus { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
    }
}
