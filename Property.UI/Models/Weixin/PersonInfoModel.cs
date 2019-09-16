using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 个人资料
    /// </summary>
    public class PersonInfoModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadPath { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 性别下拉列表
        /// </summary>
        public List<SelectListItem> GenderList { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerityCode { get; set; }
    }

    /// <summary>
    /// 修改手机
    /// </summary>
    public class PhoneInfoModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerityCode { get; set; }
    }

    /// <summary>
    /// 修改用户名
    /// </summary>
    public class UserNameInfoModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// 修改邮箱
    /// </summary>
    public class EmailInfoModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// 修改性别
    /// </summary>
    public class GenderInfoModel
    {
        /// <summary>
        /// 性别
        /// </summary>
        public int? Gerder { get; set; }
    }
}