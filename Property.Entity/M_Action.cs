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
    /// 页面权限表
    /// </summary>
    public class M_Action
    {
        public M_Action() 
        {
            this.ActionItems = new HashSet<M_ActionItem>();
            this.PlatformRoleActions = new HashSet<R_PlatformRoleAction>();
            this.PropertyRoleActions = new HashSet<R_PropertyRoleAction>();
            this.CompanyRoleActions = new HashSet<R_CompanyRoleAction>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ActionName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [MaxLength(300)]
        public string ActionDesc { get; set; }

        /// <summary>
        /// 链接Url地址
        /// </summary>
        [MaxLength(200)]
        public string Href { get; set; }

        /// <summary>
        /// 所属菜单
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 外键 菜单表
        /// </summary>
        [ForeignKey("MenuId")]
        public virtual M_Menu Menu { get; set; }

        /// <summary>
        /// 该权限组下所有权限Item
        /// </summary>
        public virtual ICollection<M_ActionItem> ActionItems { get; set; }

        /// <summary>
        /// 包含该权限的所有平台角色关联
        /// </summary>
        public virtual ICollection<R_PlatformRoleAction> PlatformRoleActions { get; set; }

        /// <summary>
        /// 包含该权限的所有物业角色关联
        /// </summary>
        public virtual ICollection<R_PropertyRoleAction> PropertyRoleActions { get; set; }
        /// <summary>
        /// 包含该权限所有权限关联表
        /// </summary>
        public virtual ICollection<R_CompanyRoleAction> CompanyRoleActions { get; set; }
    }
}
