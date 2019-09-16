using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 手机验证实体类
    /// </summary>
    public class T_PhoneValidate
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(15)]
        [Required]
        public string PhoneNum { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [MaxLength(6)]
        [Required]
        public string ValidateCode { get; set; }

        /// <summary>
        /// 0：注册验证  1：找回密码验证 2：身份验证
        /// </summary>
        public int ActionCode{get;set;}

        /// <summary>
        /// 验证码失效时间
        /// </summary>
        public DateTime InvalidTime { get; set; }
    }
}
