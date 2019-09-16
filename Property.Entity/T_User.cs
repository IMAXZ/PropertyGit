using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{

    /// <summary>
    /// 业主信息表
    /// </summary>
    public class T_User
    {
        public T_User()
        {
            this.Questions = new HashSet<T_Question>();
            this.UserPlaces = new HashSet<R_UserPlace>();
            this.PostBarTopics = new HashSet<T_PostBarTopic>();
            this.Orders = new HashSet<T_Order>();
            this.AppUserShippingAddresss = new HashSet<T_AppUserShippingAddress>();
            this.PostBarTopicDiscussList = new HashSet<T_PostBarTopicDiscuss>();
            this.ReplyPostBarTopicDiscussList = new HashSet<T_PostBarTopicDiscuss>();
            this.PropertyIdentityVerification = new HashSet<R_PropertyIdentityVerification>();
            this.UserPostBarTopics = new HashSet<R_UserPostBarTopic>();

            this.SocialCircles = new HashSet<T_SocialCircle>();
            this.UserSocialCircles = new HashSet<R_UserSocialCircle>();
            this.UserSocialCircleMassTextings = new HashSet<R_UserSocialCircleMassTexting>();
            this.SocialCircleChats = new HashSet<T_SocialCircleChat>();
            this.LifeBills = new HashSet<T_LifeBill>();
            this.Feedbacks = new HashSet<T_Feedback>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        [MaxLength(100)]
        public string Token { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public Nullable<DateTime> LatelyLoginTime { get; set; }

        /// <summary>
        /// Token失效时间
        /// </summary>
        public Nullable<DateTime> TokenInvalidTime { get; set; }

        /// <summary>
        /// 重置密码的激活码
        /// </summary>
        [MaxLength(100)]
        public string Activecode { get; set; }

        /// <summary>
        /// 重置密码的激活码 失效时间
        /// </summary>
        public Nullable<DateTime> ActivecodeInvalidTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 性别 0:男 1：女
        /// </summary>
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// 手机
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
        /// 微信用户OpenId
        /// </summary>
        [MaxLength(100)]
        public string WeixinOpenId { get; set; }

        /// <summary>
        /// 微信用户UnionId
        /// </summary>
        [MaxLength(100)]
        public string WeixinUnionId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        public string Password { get; set; }


        /// <summary>
        /// 头像图片路径
        /// </summary>
        [MaxLength(200)]
        public string HeadPath { get; set; }


        /// <summary>
        /// 头像缩略图路径
        /// </summary>
        [MaxLength(200)]
        public string HeadThumbnail { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该业主用户提报的所有问题
        /// </summary>
        public virtual ICollection<T_Question> Questions { get; set; }

        /// <summary>
        /// 该业主用户属于的所有小区关联
        /// </summary>
        public virtual ICollection<R_UserPlace> UserPlaces { get; set; }

        /// <summary>
        /// 该业主用户发布的所有物业小区贴吧主题
        /// </summary>
        public virtual ICollection<T_PostBarTopic> PostBarTopics { get; set; }

        /// <summary>
        /// 该业主用户的贴吧回复记录
        /// </summary>
        public virtual ICollection<T_PostBarTopicDiscuss> PostBarTopicDiscussList { get; set; }

        /// <summary>
        /// 该业主用户的贴吧被回复记录
        /// </summary>
        public virtual ICollection<T_PostBarTopicDiscuss> ReplyPostBarTopicDiscussList { get; set; }

        /// <summary>
        /// 该业主用户的所有订单表
        /// </summary>
        public virtual ICollection<T_Order> Orders { get; set; }

        /// <summary>
        /// 该业主用户的所有App收货地址
        /// </summary>
        public virtual ICollection<T_AppUserShippingAddress> AppUserShippingAddresss { get; set; }

        /// <summary>
        /// 该业主用户的身份验证表
        /// </summary>
        public virtual ICollection<R_PropertyIdentityVerification> PropertyIdentityVerification { get; set; }

        /// <summary>
        /// 该业主用户收藏的所有物业小区贴吧主题关联对象
        /// </summary>
        public virtual ICollection<R_UserPostBarTopic> UserPostBarTopics { get; set; }

        /// <summary>
        /// 该业主用户创建的圈子
        /// </summary>
        public virtual ICollection<T_SocialCircle> SocialCircles { get; set; }

        /// <summary>
        /// 该业主用户申请加入的圈子关联
        /// </summary>
        public virtual ICollection<R_UserSocialCircle> UserSocialCircles { get; set; }

        /// <summary>
        /// 该业主用户圈子群发成员关联
        /// </summary>
        public virtual ICollection<R_UserSocialCircleMassTexting> UserSocialCircleMassTextings { get; set; }

        /// <summary>
        /// 该业主用户所有的聊天记录
        /// </summary>
        public virtual ICollection<T_SocialCircleChat> SocialCircleChats { get; set; }

        /// <summary>
        /// 该业主用户所有的生活记账
        /// </summary>
        public virtual ICollection<T_LifeBill> LifeBills { get; set; }

        /// <summary>
        /// 该业主用户的所有意见反馈
        /// </summary>
        public virtual ICollection<T_Feedback> Feedbacks { get; set; }
    }
}
