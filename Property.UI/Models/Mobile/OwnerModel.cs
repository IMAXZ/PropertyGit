using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 业主信息模型
    /// </summary>
    public class OwnerModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string HeadPath { get; set; }

        /// <summary>
        /// 业主性别
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }

    /// <summary>
    /// 登录模型
    /// </summary>
    public class OwnerLoginModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 推送设备ID
        /// </summary>
        public string RegistrationId { get; set; }
    }

    /// <summary>
    /// 登录模型
    /// </summary>
    public class WeixinLoginModel
    {
        /// <summary>
        /// 微信授权ID
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 推送设备ID
        /// </summary>
        public string RegistrationId { get; set; }
    }

    /// <summary>
    /// 业主注册模型
    /// </summary>
    public class OwnerRegisterModel
    {
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }
    }


    /// <summary>
    /// 业主手机号注册模型
    /// </summary>
    public class OwnerPhoneRegisterModel
    {

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string validateCode { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }
    }

    /// <summary>
    /// 业主微信第三方账号注册模型
    /// </summary>
    public class OwnerWeixinRegisterModel
    {
        /// <summary>
        /// 微信授权ID
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }
    }

    /// <summary>
    /// 业主信息设置模型
    /// </summary>
    public class OwnerInfoModel : TokenModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像图片文件
        /// </summary>
        public string HeadPic { get; set; }

        /// <summary>
        /// 业主性别 0:男 1：女
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }
    }


    /// <summary>
    /// 业主密码修改模型
    /// </summary>
    public class OwnerChangePasswordModel : TokenModel
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }
    }

    /// <summary>
    /// 密码重置模型
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// 手机重置密码模型
    /// </summary>
    public class PhoneResetPasswordModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }
    }

    /// <summary>
    /// 业主身份审批验证模型
    /// </summary>
    public class OwnerApprovalModel : TokenModel
    {
        /// <summary>
        /// 小区Id
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 户Id或办公楼单位ID
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 业主电话
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string CodeNum { get; set; }
    }
}