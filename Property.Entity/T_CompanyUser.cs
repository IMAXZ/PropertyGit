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
    /// 物业公司用户表实体类
    /// </summary>
    public class T_CompanyUser
    {
        public T_CompanyUser()
        {
            this.CompanyOpreateLogs = new HashSet<T_CompanyOpreateLog>();
            this.CompanyUserRoles = new HashSet<R_CompanyUserRole>();
            this.CompanyPosts = new HashSet<T_CompanyPost>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }

        /// <summary>
        /// 性别
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
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 外键 物业总公司Id
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 物业总公司关联对象
        /// </summary>
        [ForeignKey("CompanyId")]
        public virtual T_Company Company { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public Nullable<DateTime> LatelyLoginTime { get; set; }

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
        /// 是否是管理员 0:默认 1：管理员
        /// </summary>
        public int IsMgr { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1：删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public Nullable<DateTime> CreatedDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 该物业公司用户的所有操作日志
        /// </summary>
        public virtual ICollection<T_CompanyOpreateLog> CompanyOpreateLogs { get; set; }

        /// <summary>
        /// 该物业公司用户的所有用户角色表
        /// </summary>
        public virtual ICollection<R_CompanyUserRole> CompanyUserRoles { get; set; }

        /// <summary>
        /// 该物业公司用户发布的新闻公告
        /// </summary>
        public virtual ICollection<T_CompanyPost> CompanyPosts { get; set; }
    }
}
