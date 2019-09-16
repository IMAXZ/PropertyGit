using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 门店用户表对应实体类
    /// </summary>
    public class T_ShopUser
    {
        public T_ShopUser()
        {
            this.Shops = new HashSet<T_Shop>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

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
        /// 姓名
        /// </summary>
        [MaxLength(30)]
        public string TrueName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(200)]
        public string HeadPath { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该门店用户表下的所有门店表
        /// </summary>
        public virtual ICollection<T_Shop> Shops { get; set; }
    }
}
