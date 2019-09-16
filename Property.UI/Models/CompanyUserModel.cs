using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    public class CompanyUserModel
    {
        /// <summary>
        /// 公司用户Id
        /// </summary>
        public int CompanyUserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
       [MaxLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        public string Password { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }
        /// <summary>
        /// 物业公司id
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 头像图片路径
        /// </summary>
        [MaxLength(200)]
        public string HeadPath { get; set; }
        ///
        [MaxLength(200)]
        public string HeadThumbnail { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        [DisplayName("备注")]
        public string Memo { get; set; }

    }

    /// <summary>
    /// 物业总公司用户角色模型
    /// </summary>
    public class CompanyUserRoleModel
    {
        /// <summary>
        /// 物业总公司用户
        /// </summary>
        public CompanyUserModel User { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<T_CompanyRole> RoleList { get; set; }

        /// <summary>
        /// 该用户已分配的角色ID
        /// </summary>
        public List<int> RoleIds { get; set; }
    }

    /// <summary>
    /// 物业总公司用户分配角色模型
    /// </summary>
    public class CompanyUserConfigRoleModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 该用户要分配的角色
        /// </summary>
        public int[] ids { get; set; }
    }
}