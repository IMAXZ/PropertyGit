using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 修改登录用户个人信息模型
    /// </summary>
    public class LoggedInAccountModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(50)]
        public string UserName { get; set; }

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

    /// <summary>
    /// 登录用户头像设置模型
    /// </summary>
    public class AccountHeadPicModel
    {
        /// <summary>
        /// 图片数据
        /// </summary>
        public string Data;

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId;
    }

    /// <summary>
    /// 登录用户密码修改
    /// </summary>
    public class AccountPasswordChangeModel
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
    }
}
