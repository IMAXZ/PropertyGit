using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    public class R_CompanyUserRole
    {
         /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 关联用户
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_CompanyUser CompanyUser { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 关联角色表
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual T_CompanyRole CompanyRole { get; set; }
    }
 }

