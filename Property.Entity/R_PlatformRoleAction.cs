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
    /// 平台角色权限关联表
    /// </summary>
    public class R_PlatformRoleAction
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 外键 平台角色表
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual T_PlatformRole PlatformRole { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 外键 权限表
        /// </summary>
        [ForeignKey("ActionId")]
        public virtual M_Action Action { get; set; }
    }
}
