using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 用户密码重置模型
    /// </summary>
    public class UserPassResetModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        public string Activecode { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [RegularExpression(@"(?![^a-zA-Z]+$)(?!\D+$).{2,}", ErrorMessage = "密码必须包含字母和数字")]
        [MinLength(6, ErrorMessage = "密码长度至少6位")]
        [MaxLength(32, ErrorMessage = "密码长度最大32位")]
        [Compare("ConfirmPassword", ErrorMessage = "两次密码输入不一致，请重新输入")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 密码重置链接激活跳转模型
    /// </summary>
    public class PassResetActiveModel
    {
        /// <summary>
        /// 用户ID加密字符串
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        public string Activecode { get; set; }
    }
}