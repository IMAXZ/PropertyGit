using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Models
{

    /// <summary>
    /// 门店用户模型
    /// </summary>
    public class ShopUserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///用户名 
        /// </summary>
        [MaxLength(30)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(15)]
        public string TrueName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [MaxLength(15)]
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 性别下拉列表
        /// </summary>
        public List<SelectListItem> GenderList { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [MaxLength(32)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}