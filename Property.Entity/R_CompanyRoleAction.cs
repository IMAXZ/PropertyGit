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
    ///物业总公司角色权限
    /// </summary>
    public class R_CompanyRoleAction
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
        /// 关联物业角色表
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual T_CompanyRole CompanyRole { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 关联权限表
        /// </summary>
        [ForeignKey("ActionId")]
        public virtual M_Action Action { get; set; }
    }
}
