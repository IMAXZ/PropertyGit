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
    /// 圈子群发成员实体类
    /// </summary>
    public class R_UserSocialCircleMassTexting
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 圈子成员Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 圈子成员关联表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User User { get; set; }

        /// <summary>
        /// 所属群发记录Id
        /// </summary>
        public int MassTextingId { get; set; }

        /// <summary>
        /// 群发记录关联表
        /// </summary>
        [ForeignKey("MassTextingId")]
        public virtual T_SocialCircleMassTexting SocialCircleMassTexting { get; set; }

        /// <summary>
        /// 是否未读
        /// </summary>
        public bool IsNoRead { get; set; }
    }
}
