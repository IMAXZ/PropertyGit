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
    /// 物业角色表对应实体类
    /// </summary>
    public class T_PropertyRole
    {
        public T_PropertyRole() 
        {
            this.PropertyRoleActions = new HashSet<R_PropertyRoleAction>();
            this.PropertyUserRoles = new HashSet<R_PropertyUserRole>();
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 物业角色名称
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
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区ID关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 该角色所有的权限关联
        /// </summary>
        public virtual ICollection<R_PropertyRoleAction> PropertyRoleActions { get; set; }

        /// <summary>
        /// 该角色所有的用户关联
        /// </summary>
        public virtual ICollection<R_PropertyUserRole> PropertyUserRoles { get; set; }
    }
}
