using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// 物业总公司角色表对应实体类
    /// </summary>
    public class T_CompanyRole
    {
        public T_CompanyRole()
        {
            this.CompanyRoleActions = new HashSet<R_CompanyRoleAction>();
            this.CompanyUserRoles = new HashSet<R_CompanyUserRole>();
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 物业名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        /// <summary>
        /// 物业角色描述
        /// </summary>
        [MaxLength(200)]
        public string RoleMemo { get; set; }

        /// <summary>
        /// 是否是系统角色
        /// </summary>
        public int IsSystem { get; set; }

        /// <summary>
        /// 所属总公司ID
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 所属总公司ID关联表对象
        /// </summary>
        [ForeignKey("CompanyId")]
        public virtual T_Company Company { get; set; }

        /// <summary>
        /// 物业总公司角色表下所有的权限
        /// </summary>
        public virtual ICollection<R_CompanyRoleAction> CompanyRoleActions { get; set; }

        /// <summary>
        /// 物业总公司角色表下所有的用户角色表
        /// </summary>
        public virtual ICollection<R_CompanyUserRole> CompanyUserRoles { get; set; }
    }
}
