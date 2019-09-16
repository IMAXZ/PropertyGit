using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 获取验证码模型
    /// </summary>
    public class ValidateCodeGetModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 操作类型码：0：注册验证  1：找回密码验证 2：身份验证
        /// </summary>
        public int ActionCode { get; set; }
    }

    /// <summary>
    /// 验证码提交模型
    /// </summary>
    public class ValidateCodeModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }
    }

}