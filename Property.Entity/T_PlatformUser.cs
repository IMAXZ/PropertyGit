using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 平台用户信息表
    /// </summary>
    public class T_PlatformUser
    {
        public T_PlatformUser() 
        {
            this.PlatformUserRoles = new HashSet<R_PlatformUserRole>();
            this.PlatformOpreateLogs = new HashSet<T_PlatformOpreateLog>();
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
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }

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
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(32)]
        [Required]
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
        /// 是否是管理员 0:默认 1:管理员
        /// </summary>
        public int IsMgr { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该平台用户所有的角色关联
        /// </summary>
        public virtual ICollection<R_PlatformUserRole> PlatformUserRoles { get; set; }

        /// <summary>
        /// 该平台用户所有的操作日志
        /// </summary>
        public virtual ICollection<T_PlatformOpreateLog> PlatformOpreateLogs { get; set; }
    }
}
