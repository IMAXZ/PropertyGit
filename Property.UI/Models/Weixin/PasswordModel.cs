using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 密码管理模型
    /// </summary>
    public class PasswordModel
    {
        /// <summary>
        /// 设置密码
        /// </summary>
        [RegularExpression("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,32}$")]
        public string sPassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 原密码
        /// </summary>
        public string BeforePassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
       [RegularExpression("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,32}$")]
        public string NewPassword { get; set; }
    }
}