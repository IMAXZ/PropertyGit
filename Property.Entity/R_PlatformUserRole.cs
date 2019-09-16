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
    /// 平台用户角色表
    /// </summary>
    public class R_PlatformUserRole
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 外键 平台用户表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_PlatformUser PlatformUser { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 外键 平台角色表
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual T_PlatformRole PlatformRole { get; set; }

    }
}
