using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 平台角色表
    /// </summary>
    public class T_PlatformRole
    {
        public T_PlatformRole() 
        {
            this.PlatformRoleActions = new HashSet<R_PlatformRoleAction>();
            this.PlatformUserRoles = new HashSet<R_PlatformUserRole>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// 角色名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string RoleName { get; set; }


        /// <summary>
        /// 角色简介
        /// </summary>
        [MaxLength(200)]
        public string RoleMemo { get; set; }

        /// <summary>
        /// 是否是系统角色
        /// </summary>
        public int IsSystem { get; set; }

        /// <summary>
        /// 该角色所有的权限关联
        /// </summary>
        public virtual ICollection<R_PlatformRoleAction> PlatformRoleActions { get; set; }

        /// <summary>
        /// 该角色所有的用户关联
        /// </summary>
        public virtual ICollection<R_PlatformUserRole> PlatformUserRoles { get; set; }
    }
}
