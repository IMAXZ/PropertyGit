using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Common
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public sealed class ConstantParam
    {

        /// <summary>
        /// 分页大小
        /// </summary>
        public const int PAGE_SIZE = 10;

        /// <summary>
        /// 保留小数位
        /// </summary>
        public const int DECIMAL_PLACE = 1;

        /// <summary>
        /// 保存到session的用户数据的KEY
        /// </summary>
        public const string SESSION_USERINFO = "UserSession";

        #region Ajax请求操作相关常量
        /// <summary>
        /// Ajax请求返回标示位，表示操作成功
        /// </summary>
        public const string JSON_RESULT_OK = "OK";

        /// <summary>
        /// Ajax请求返回标示位，表示操作失败
        /// </summary>
        public const string JSON_RESULT_NO = "NO";

        /// <summary>
        /// Ajax请求返回标示位，模型验证失败
        /// </summary>
        public const string JSON_RESULT_MODEL_CHECK_ERROR = "表单验证失败";

        #endregion

        #region 系统用户类型，角色类型常量

        /// <summary>
        /// 类型:物业平台，物业用户
        /// </summary>
        public const int USER_TYPE_PROPERTY = 0;

        /// <summary>
        /// 类型:平台后台，后台用户
        /// </summary>
        public const int USER_TYPE_PLATFORM = 1;

        /// <summary>
        /// 类型:门店平台（用户）
        /// </summary>
        public const int USER_TYPE_SHOP = 2;

        /// <summary>
        /// 类型：总公司平台（用户）
        /// </summary>
        public const int USER_TYPE_COMPANY = 3;

        /// <summary>
        /// 用户角色 0:默认用户,或普通角色
        /// </summary>
        public const int USER_ROLE_DEFAULT = 0;

        /// <summary>
        /// 用户角色 :管理员或系统角色
        /// </summary>
        public const int USER_ROLE_MGR = 1;

        #endregion

        /// <summary>
        /// 菜单类型：左侧导航菜单
        /// </summary>
        public const int MENU_LEFT = 0;

        /// <summary>
        /// 菜单类型：其他菜单
        /// </summary>
        public const int MENU_OTHER = 1;


        /// <summary>
        /// 删除标识：默认
        /// </summary>
        public const int DEL_FLAG_DEFAULT = 0;

        /// <summary>
        /// 删除标识：删除
        /// </summary>
        public const int DEL_FLAG_DELETE = 1;

        /// <summary>
        /// 巡检类别：日巡检
        /// </summary>
        public const int INSPECTION_TYPE_DAY = 0;

        /// <summary>
        /// 巡检类别：周巡检
        /// </summary>
        public const int INSPECTION_TYPE_WEEK = 1;

        /// <summary>
        /// 巡检类别：月巡检
        /// </summary>
        public const int INSPECTION_TYPE_MONTH = 2;

        /// <summary>
        /// 状态：未处理
        /// </summary>
        public const int NO_DISPOSE = 0;
        /// <summary>
        /// 状态：已处理
        /// </summary>
        public const int DISPOSED = 1;

        /// <summary>
        /// 性别：男
        /// </summary>
        public const int GENDER_ZERO = 0;

        /// <summary>
        /// 性别：女
        /// </summary>
        public const int GENDER_ONE = 1;
        /// <summary>
        /// 缴费：非固定
        /// </summary>
        public const int NO_FIXED = 0;
        /// <summary>
        /// 缴费固定
        /// </summary>
        public const int FIXED = 1;
        /// <summary>
        /// 业主头像目录
        /// </summary>
        public const string OWNER_HEAD_DIR = "/Images/Owner/";

        /// <summary>
        /// 物业用户头像目录
        /// </summary>
        public const string PROPERTY_USER_HEAD_DIR = "/Images/PropertyUser/";

        /// <summary>
        /// 平台用户头像目录
        /// </summary>
        public const string PLATFORM_USER_HEAD_DIR = "/Images/PlatformUser/";

        /// <summary>
        /// 门店用户头像
        /// </summary>
        public const string ShOPFORM_USER_HEAD_DIR = "/Images/ShopformUser/";

        /// <summary>
        /// 总公司用户头像
        /// </summary>
        public const string COMPANY_USER_HEAD_DIR = "/Images/CompanyUser/";

        /// <summary>
        /// 小区图片目录
        /// </summary>
        public const string PROPERTY_PLACE_DIR = "/Images/PropertyPlace/";

        /// <summary>
        /// 小区缩略图目录
        /// </summary>
        public const string PROPERTY_PLACE_ThumIMG_DIR = "/Images/PropertyPlaceThum/";

        /// <summary>
        /// 公司图片目录
        /// </summary>
        public const string PROPERTY_COMPANY_DIR = "/Images/PropertyCompany/";

        /// <summary>
        /// 支付宝私钥公钥文件目录
        /// </summary>
        public const string ALIPAY_KEY = "/App_Data/";

        /// <summary>
        /// 话题图片目录
        /// </summary>
        public const string Topic_Pictures_DIR = "/Images/TopicPictures/";

        /// <summary>
        /// 话题缩略图目录
        /// </summary>
        public const string Topic_ThumPictures_DIR = "/Images/TopicThumPictures/";

        /// <summary>
        /// 业主圈子头像
        /// </summary>
        public const string SOCIAL_CIRCLE_HEAD_DIR = "/Images/SocialCircle/";

        /// <summary>
        /// 上报问题、异常上传的文件保存目录
        /// </summary>
        public const string QUESTION_FILE = "/Upload/Place_";

        /// <summary>
        /// 意见反馈 图片保存目录
        /// </summary>
        public const string FEEDBACK = "/Upload/Feedback";

        /// <summary>
        /// 移动端版本类型：业主 0
        /// </summary>
        public const int MOBILE_TYPE_OWNER = 0;

        /// <summary>
        /// 移动端版本类型：物业 1
        /// </summary>
        public const int MOBILE_TYPE_PROPERTY = 1;

        /// <summary>
        /// 移动端版本类型：商户 2
        /// </summary>
        public const int MOBILE_TYPE_SHOP = 2;

        /// <summary>
        /// APK文件上传目录
        /// </summary>
        public const string APK_FILE_DIR = "/Upload/Apk/";

        /// <summary>
        /// 极光推送APP KEY
        /// </summary>
        public static string APP_KEY = "e01beb7258fd7bebc5904ec9";

        /// <summary>
        /// 密钥
        /// </summary>
        public static string MASTER_SECRET = "2003fd7db431b88ab77441a5";

        /// <summary>
        /// 物业客户端 极光推送 APP KEY
        /// </summary>
        public static string PROPERTY_APP_KEY = "9c4018eb47a20583c52fb5ff";

        /// <summary>
        /// 物业客户端 密钥
        /// </summary>
        public static string PROPERTY_MASTER_SECRET = "243b7169748ea9d05bececb2";

        /// <summary>
        /// 商家客户端 极光推送 APP KEY
        /// </summary>
        public static string SHOP_APP_KEY = "b061708bf838006ddc020828";

        /// <summary>
        /// 商家客户端 密钥
        /// </summary>
        public static string SHOP_MASTER_SECRET = "f24a53d388d2c533806704f8";

        /// <summary>
        /// 物业系统角色名称
        /// </summary>
        public static string SYSTEM_ROLE_NAME = "物业管理员";

        /// <summary>
        /// 物业系统角色描述
        /// </summary>
        public static string SYSTEM_ROLE_MEMO = "管理小区物业人员，角色等";

        /// <summary>
        /// 物业总公司系统角色名称
        /// </summary>
        public static string COMPANY_SYSTEM_ROLE_NAME = "总公司管理员";

        /// <summary>
        /// 物业总公司系统角色描述
        /// </summary>
        public static string COMPANY_SYSTEM_ROLE_MEMO = "查看公司旗下的小区的物业内容";

        /// <summary>
        /// 巡检结果：正常
        /// </summary>
        public static int NORMAL = 0;

        /// <summary>
        /// 巡检结果：异常
        /// </summary>
        public static int EXCEPTION = 1;

        /// <summary>
        /// 发布（公示）
        /// </summary>
        public static int PUBLISHED_TRUE = 1;

        /// <summary>
        /// 不发布（不公示）
        /// </summary>
        public static int PUBLISHED_FALSE = 0;

        /// <summary>
        /// 重置密码邮件标题
        /// </summary>
        public static string PASS_RESET_EMAIL_TITLE = "业主客户端密码重置";

        /// <summary>
        /// 否：0
        /// </summary>
        public static int DELIVERY_FLAG_FALSE = 0;

        /// <summary>
        /// 是：1
        /// </summary>
        public static int DELIVERY_FLAG_TRUE = 1;

        /// <summary>
        /// 门店类型0：绿色直供
        /// </summary>
        public static int SHOP_TYPE_0 = 0;

        /// <summary>
        /// 门店类型1：开锁服务
        /// </summary>
        public static int SHOP_TYPE_1 = 1;

        /// <summary>
        /// 门店类型2：生活小卖店
        /// </summary>
        /// 
        public static int SHOP_TYPE_2 = 2;

        /// <summary>
        /// 门店类型3：五金用品
        /// </summary>
        public static int SHOP_TYPE_3 = 3;

        /// <summary>
        /// 门店类型0：绿色直供
        /// </summary>
        public static string SHOP_TYPE_String_0 = "绿色直供";

        /// <summary>
        /// 门店类型1：开锁服务
        /// </summary>
        public static string SHOP_TYPE_String_1 = "开锁服务";

        /// <summary>
        /// 门店类型2：生活小卖店
        /// </summary>
        public static string SHOP_TYPE_String_2 = "生活小卖店";

        /// <summary>
        /// 门店类型3：五金用品
        /// </summary>
        public static string SHOP_TYPE_String_3 = "五金用品";

        /// <summary>
        /// 门店图片文件上传目录
        /// </summary>
        public const string SHOP_IMG_DIR = "/Upload/ShopImg/";

        /// <summary>
        /// 门店图片缩略图片上传目录
        /// </summary>
        public const string SHOP_THUM_IMG_DIR = "/Upload/ShopThumbImg/";

        /// <summary>
        /// 门店促销上传图片
        /// </summary>
        public const string SHOP_Sales = "/Upload/ShopSale";

        /// <summary>
        /// 门店促销上传缩略图片
        /// </summary>
        public const string SHOP_Sales_ThumIMG = "/Upload/ShopSaleThumImg";

        /// <summary>
        /// 小区类型：住宅小区
        /// </summary>
        public const int PLACE_TYPE_HOUSE = 0;

        /// <summary>
        /// 小区类型：办公楼小区
        /// </summary>
        public const int PLACE_TYPE_COMPANY = 1;

        /// <summary>
        /// 账户类型：微信
        /// </summary>
        public const int PROPERTY_ACCOUNT_WeChat = 0;

        /// <summary>
        /// 账户类型：支付宝
        /// </summary>
        public const int PROPERTY_ACCOUNT_Alipay = 1;

        /// <summary>
        /// 未缴费
        /// </summary>
        public const int PAYED_FALSE = 0;

        /// <summary>
        /// 已缴费
        /// </summary>
        public const int PAYED_TRUE = 1;

        /// <summary>
        /// 缴费周期：每月
        /// </summary>
        public const int ExpenseCycle_ONE_MONTH = 1;

        /// <summary>
        /// 缴费周期：每两月
        /// </summary>
        public const int ExpenseCycle_TWO_MONTH = 2;

        /// <summary>
        /// 缴费周期：每季度
        /// </summary>
        public const int ExpenseCycle_ONE_QUARTER = 3;

        /// <summary>
        /// 缴费周期：每半年
        /// </summary>
        public const int ExpenseCycle_HARF_YEAR = 4;

        /// <summary>
        /// 缴费周期：每年
        /// </summary>
        public const int ExpenseCycle_ONE_YEAR = 5;

        /// <summary>
        /// 开票类型：未开
        /// </summary>
        public const int InvoiceType_NO = 0;

        /// <summary>
        /// 开票类型：收据
        /// </summary>
        public const int InvoiceType_SJ = 1;

        /// <summary>
        /// 开票类型：小票
        /// </summary>
        public const int InvoiceType_XP = 2;

        /// <summary>
        /// 业主审批信息 审核中
        /// </summary>
        public const int IsVerified_DEFAULT = 0;

        /// <summary>
        /// 业主审批信息 通过验证
        /// </summary>
        public const int IsVerified_YES = 1;

        /// <summary>
        /// 业主审批信息 驳回
        /// </summary>
        public const int IsVerified_NO = 2;

        /// <summary>
        /// 待付款
        /// </summary>
        public const int OrderStatus_NOPAY = 0;

        /// <summary>
        /// 待确认
        /// </summary>
        public const int OrderStatus_CONFIRM = 1;

        /// <summary>
        /// 待收货
        /// </summary>
        public const int OrderStatus_RECEIPT = 2;

        /// <summary>
        /// 已退单
        /// </summary>
        public const int OrderStatus_EXIT = 3;

        /// <summary>
        /// 交易完成
        /// </summary>
        public const int OrderStatus_FINISH = 4;

        /// <summary>
        /// 交易关闭
        /// </summary>
        public const int OrderStatus_CLOSE = 5;

        /// <summary>
        /// 默认未支付
        /// </summary>
        public const int DEFAULT_NO_PAY = 0;

        /// <summary>
        /// 微信在线支付
        /// </summary>
        public const int WeChat_ONLINE_PAY = 1;

        /// <summary>
        /// 支付宝在线支付
        /// </summary>
        public const int AliPay_ONLINE_PAY = 2;

        /// <summary>
        /// 货到现金付款
        /// </summary>
        public const int DELIVER_CASH_PAY = 3;

        /// <summary>
        /// 货到微信付款
        /// </summary>
        public const int DELIVER_WeChat_PAY = 4;

        /// <summary>
        /// 货到支付宝付款
        /// </summary>
        public const int DELIVER_AliPay_PAY = 5;
        /// <summary>
        /// 商品已经下架
        /// </summary>
        public const int SHOPSALED= 0;
        /// <summary>
        /// 商品已经上架
        /// </summary>
        public const int SHOPSALING = 1;
        #region 微信常量
        /// <summary>
        /// 由开发者可以任意填写，用作生成签名
        /// </summary>
        public const string Token = "aiwojia_weixin_server";

        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public const string AppId = "wx31d637be898356a5";

        /// <summary>
        /// 由开发者手动填写或随机生成，将用作消息体加解密密钥
        /// </summary>
        public const string EncodingAESKey = "Aiwojia1weixin2server3key4aiwojia5weixinkey";

        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public const string AppSecret = "fb16ce97250630736d44fda4bee52c48";

        /// <summary>
        /// 圈子模块上传图片路径
        /// </summary>
        public const string SOCIAL_CIRCLE_CHAT_DIR = "/Upload/Weixin/SocialCircle/";

        /// <summary>
        /// 问题上报模块上传图片路径
        /// </summary>
        public const string QUESTION_DIR = "/Upload/Weixin/";

        /// <summary>
        /// 话题圈模块上传图片路径
        /// </summary>
        public const string TOPIC_DIR = "/Upload/Weixin/";

        #endregion
    }
}
