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
    /// App用户收货地址表 实体类
    /// </summary>
    public class T_AppUserShippingAddress
    {
        public T_AppUserShippingAddress() 
        {
            this.Orders = new HashSet<T_Order>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string ShipperName { get; set; }

        /// <summary>
        /// 性别 0:男 1：女
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 县区ID
        /// </summary>
        public int CountyId { get; set; }

        /// <summary>
        /// 所属县区 关联表对象
        /// </summary>
        [ForeignKey("CountyId")]
        public virtual M_County County { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string AddressDetails { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Telephone { get; set; }

        /// <summary>
        /// 是否默认地址 0.不是 1.是（只能有一个默认收货地址）
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 所属AppUser
        /// </summary>
        public int AppUserId { get; set; }

        /// <summary>
        /// 所属AppUser 关联表对象
        /// </summary>
        [ForeignKey("AppUserId")]
        public virtual T_User User { get; set; }

        /// <summary>
        /// 收货地址为该地址的所有订单
        /// </summary>
        public virtual ICollection<T_Order> Orders { get; set; }
    }
}
