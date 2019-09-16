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
    /// 物业小区身份验证表 实体类
    /// </summary>
    public class R_PropertyIdentityVerification
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 单元户Id
        /// </summary>
        public Nullable<int> DoorId { get; set; }

        /// <summary>
        /// 外键 单元户信息表
        /// </summary>
        [ForeignKey("DoorId")]
        public virtual T_BuildDoor BuildDoor { get; set; }

        /// <summary>
        /// 手机App用户Id
        /// </summary>
        public int AppUserId { get; set; }

        /// <summary>
        /// 外键 业主信息表
        /// </summary>
        [ForeignKey("AppUserId")]
        public virtual T_User User { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Phone { get; set; }

        /// <summary>
        /// 办公楼Id
        /// </summary>
        public Nullable<int> BuildCompanyId { get; set; }

        /// <summary>
        /// 外键 办公楼单位业主信息登记表
        /// </summary>
        [ForeignKey("BuildCompanyId")]
        public virtual T_BuildCompany BuildCompany { get; set; }

        /// <summary>
        /// 是否通过验证 0.审核中 1.通过  2.驳回
        /// </summary>
        public int IsVerified { get; set; }
    }
}
