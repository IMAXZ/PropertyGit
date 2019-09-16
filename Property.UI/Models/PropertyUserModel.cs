using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Webdiyer.WebControls.Mvc;


namespace Property.UI.Models
{
    /// <summary>
    /// 物业用户模型
    /// </summary>
    public class PropertyUserModel
    {

        /// <summary>
        /// 物业用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 物业用户名
        /// </summary>
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 物业公司Id
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 物业小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 物业小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        [DisplayName("用户描述")]
        public string Memo { get; set; }

    }
    public class PropertyUserSearchModel : SearchModel
    {
        /// <summary>
        /// 所属物业小区
        /// </summary>
        public string PlaceId { get; set; }
        /// <summary>
        /// 所属物业小区下拉列表
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> PlaceList { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        public PagedList<T_PropertyUser> DataList { get; set; }


    }
    /// <summary>
    /// 物业用户角色模型
    /// </summary>
    public class PropertyUserRoleModel
    {
        /// <summary>
        /// 物业用户
        /// </summary>
        public PropertyUserModel User { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<T_PropertyRole> RoleList { get; set; }

        /// <summary>
        /// 该用户已分配的角色ID
        /// </summary>
        public List<int> RoleIds { get; set; }
    }

    /// <summary>
    /// 物业用户分配角色模型
    /// </summary>
    public class PropertyUserConfigRoleModel
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