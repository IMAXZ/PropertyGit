using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 平台用户管理模型
    /// </summary>
    public class PlatformUserModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(50)]
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        [DisplayName("姓名")]
        public string TrueName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [MaxLength(15)]
        [DisplayName("手机号码")]
        public string Phone { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        [DisplayName("联系方式")]
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50)]
        [DisplayName("电子邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        [DisplayName("用户描述")]
        public string Memo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        [DisplayName("用户密码")]
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 头像图片路径
        /// </summary>
        [MaxLength(200)]
        public string HeadPath { get; set; }
    }

    /// <summary>
    /// 平台用户管理 查询模型
    /// </summary>
    public class PlatformUserSearchModel : SearchModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        [DisplayName("姓名")]
        public string TrueName { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }
    }


    /// <summary>
    /// 平台用户角色模型
    /// </summary>
    public class PlatformUserRoleModel
    {
        /// <summary>
        /// 平台用户
        /// </summary>
        public PlatformUserModel User { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<T_PlatformRole> RoleList { get; set; }

        /// <summary>
        /// 该用户已分配的角色ID
        /// </summary>
        public List<int> RoleIds { get; set; }
    }


    /// <summary>
    /// 用户分配角色模型
    /// </summary>
    public class UserConfigRoleModel
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