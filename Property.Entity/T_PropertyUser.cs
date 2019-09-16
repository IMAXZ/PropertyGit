using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 物业小区员工表实体类
    /// </summary>
    public class T_PropertyUser
    {
        public T_PropertyUser() 
        {
            this.InspectionResults = new HashSet<T_InspectionResult>();
            this.Posts = new HashSet<T_Post>();
            this.PropertyUserRoles = new HashSet<R_PropertyUserRole>();
            this.UserPlaces = new HashSet<R_UserPlace>();
            this.PropertyOpreateLogs = new HashSet<T_PropertyOpreateLog>();
            this.QuestionDisposes = new HashSet<T_QuestionDispose>();
            this.ExceptionDisposes = new HashSet<T_InspectionExceptionDispose>();
            this.HouseUserExpenseTemplates = new HashSet<T_HouseUserExpenseTemplate>();
            this.HouseUserExpenseDetails = new HashSet<T_HouseUserExpenseDetails>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
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
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }

        /// <summary>
        /// 性别 0:男 1：女
        /// </summary>
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// 手机号
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
        [Required]
        public string Email { get; set; }

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
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区ID关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 是否是管理员 0:默认 1:管理员
        /// </summary>
        public int IsMgr { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该物业用户提报的所有巡检结果
        /// </summary>
        public virtual ICollection<T_InspectionResult> InspectionResults { get; set; }

        /// <summary>
        /// 该物业用户提交的所有公告
        /// </summary>
        public virtual ICollection<T_Post> Posts { get; set; }

        /// <summary>
        /// 该物业用户所有的角色关联
        /// </summary>
        public virtual ICollection<R_PropertyUserRole> PropertyUserRoles { get; set; }

        /// <summary>
        /// 该物业用户所有的小区关联
        /// </summary>
        public virtual ICollection<R_UserPlace> UserPlaces { get; set; }

        /// <summary>
        /// 该物业用户所有的操作日志
        /// </summary>
        public virtual ICollection<T_PropertyOpreateLog> PropertyOpreateLogs { get; set; }

        /// <summary>
        /// 该物业用户处理的所有问题
        /// </summary>
        public virtual ICollection<T_QuestionDispose> QuestionDisposes { get; set; }

        /// <summary>
        /// 该物业用户处理的所有巡检异常
        /// </summary>
        public virtual ICollection<T_InspectionExceptionDispose> ExceptionDisposes { get; set; }
        /// <summary>
        /// 物业用户下所有的缴费模板表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseTemplate> HouseUserExpenseTemplates { get; set; }
        /// <summary>
        /// 物业用户下缴费明细表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseDetails> HouseUserExpenseDetails { get; set; }
    }
}
