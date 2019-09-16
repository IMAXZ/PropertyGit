using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Common
{
    public class APIMessage
    {
        /// <summary>
        /// 手机号/邮箱不正确
        /// </summary>
        public const string USERNAME_ERROR = "手机号/邮箱不正确";

        /// <summary>
        /// 用户名不能为空
        /// </summary>
        public const string NAME_NO_NULL = "用户名不能为空";

        /// <summary>
        /// 用户名不正确
        /// </summary>
        public const string NAME_ERROR = "用户名不正确";

        /// <summary>
        /// 用户名已存在
        /// </summary>
        public const string USERNAME_EXIST = "用户名已存在";

        /// <summary>
        /// 邮箱已被使用
        /// </summary>
        public const string EMAIL_EXIST = "邮箱已被使用";

        /// <summary>
        /// 手机号已被使用
        /// </summary>
        public const string PHONE_EXIST = "手机号已被使用";

        /// <summary>
        /// 验证码不正确
        /// </summary>
        public const string VALIDATE_ERROR = "验证码不正确";

        /// <summary>
        /// 验证码已失效
        /// </summary>
        public const string VALIDATE_INVALID = "验证码已失效";

        /// <summary>
        /// 验证码获取失败，请重试
        /// </summary>
        public const string VALDATE_GET_FAIL = "验证码获取失败，请重试";

        /// <summary>
        /// 手机账号不存在
        /// </summary>
        public const string PHONE_NO_EXIST = "手机账号不存在";

        /// <summary>
        /// NO_REGISTER  微信账号未注册绑定
        /// </summary>
        public const string WEIXIN_NO_REGISTER = "微信账号未注册绑定";

        /// <summary>
        /// 密码不正确
        /// </summary>
        public const string PWD_ERROR = "密码不正确";

        /// <summary>
        /// 旧密码不正确
        /// </summary>
        public const string OLD_PWD_ERROR = "旧密码不正确";

        /// <summary>
        /// 用户不存在
        /// </summary>
        public const string NO_USER = "用户不存在";

        /// <summary>
        /// 令牌失效
        /// </summary>
        public const string TOKEN_INVALID = "令牌失效";

        /// <summary>
        /// 邮箱验证失败
        /// </summary>
        public const string EMAIL_ERROR = "邮箱验证失败";

        /// <summary>
        /// 邮件发送失败
        /// </summary>
        public const string EMAIL_SEND_ERROR = "邮件发送失败";

        /// <summary>
        /// 请求发生异常
        /// </summary>
        public const string REQUEST_EXCEPTION = "请求发生异常";

        /// <summary>
        /// 小区已添加
        /// </summary>
        public const string PLACE_EXIST = "小区已添加";

        /// <summary>
        /// 该小区不存在
        /// </summary>
        public const string PLACE_NO_EXIST = "该小区不存在";

        /// <summary>
        /// 当前已是最新版本
        /// </summary>
        public const string NO_APP = "当前已是最新版本";

        /// <summary>
        /// 所选省市区存在问题
        /// </summary>
        public const string AREA_ERROR = "所选省市区存在问题";

        /// <summary>
        /// 收货地址不存在
        /// </summary>
        public const string ADDRESS_NOEXIST = "收货地址不存在";

        /// <summary>
        /// 该小区正在审核中或已通过验证
        /// </summary>
        public const string VerifingOrYES = "该小区正在审核中或已通过验证";

        /// <summary>
        /// 缴费记录不存在
        /// </summary>
        public const string EXPENSE_RECORD_NOEXIST = "缴费记录不存在";

        /// <summary>
        /// 上报问题不存在
        /// </summary>
        public const string QUESTION_NOEXIST = "上报问题不存在";

        /// <summary>
        /// 异常结果不存在
        /// </summary>
        public const string EXCEPTION_NOEXIST = "异常结果不存在";

        /// <summary>
        /// 指派处理人失败
        /// </summary>
        public const string SET_DISPOSER_FAIL = "指派处理人失败";

        /// <summary>
        /// 门店未创建
        /// </summary>
        public const string SHOP_NOEXIST = "门店未创建";

        /// <summary>
        /// 该订单不存在
        /// </summary>
        public const string ORDER_NOEXIST = "该订单不存在";

        /// <summary>
        /// 只有待付款的订单才能支付
        /// </summary>
        public const string ORDER_NOPAYING = "只有待付款的订单才能支付";

        /// <summary>
        /// 微信支付预订单生成失败
        /// </summary>
        public const string WEIXIN_YUORDER_FAIL = "微信支付预订单生成失败";

        /// <summary>
        /// 订单退款申请失败
        /// </summary>
        public const string WEIXIN_REFUND_FAIL = "订单退款申请失败";
    }
}
