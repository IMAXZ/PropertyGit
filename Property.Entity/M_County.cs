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
    /// 县区表
    /// </summary>
    public class M_County
    {
        public M_County() 
        {
            this.PropertyPlaces = new HashSet<T_PropertyPlace>();
            this.UserShippingAddressList = new HashSet<T_AppUserShippingAddress>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 县区名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string CountyName { get; set; }


        /// <summary>
        /// 县区编号
        /// </summary>
        [MaxLength(30)]
        public string CountyNo { get; set; }

        /// <summary>
        /// 所属城市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 所属市ID 关联表对象
        /// </summary>
        [ForeignKey("CityId")]
        public virtual M_City City { get; set; }

        /// <summary>
        /// 该县区下所有的物业小区
        /// </summary>
        public virtual ICollection<T_PropertyPlace> PropertyPlaces { get; set; }

        /// <summary>
        /// 该县区下所有的收货地址
        /// </summary>
        public virtual ICollection<T_AppUserShippingAddress> UserShippingAddressList { get; set; }
    }
}
