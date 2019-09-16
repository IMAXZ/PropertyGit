using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 角色模型
    /// </summary>
    public class RoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色简介
        /// </summary>
        [MaxLength(200)]
        public string RoleMemo { get; set; }
    }


    /// <summary>
    /// 角色查询模型
    /// </summary>
    public class RoleSearchModel : SearchModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 角色权限模型
    /// </summary>
    public class RoleAuthModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public RoleModel Role { get; set; }

        /// <summary>
        /// 所有菜单列表
        /// </summary>
        public List<M_Menu> MenuList { get; set; }

        /// <summary>
        /// 该角色已有的权限的ID
        /// </summary>
        public List<int> ActionIds { get; set; }
    }

    /// <summary>
    /// 角色分配权限模型
    /// </summary>
    public class RoleConfigAuthModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 该角色要分配的权限
        /// </summary>
        public int[] ids { get; set; }
    }
}