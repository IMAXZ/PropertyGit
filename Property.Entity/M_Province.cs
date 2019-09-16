using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 省份表
    /// </summary>
    public class M_Province
    {
        public M_Province() 
        {
            this.Citys = new HashSet<M_City>();
            this.PropertyPlaces = new HashSet<T_PropertyPlace>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// 省份名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ProvinceName { get; set; }


        /// <summary>
        /// 省份编号
        /// </summary>
        [MaxLength(30)]
        public string ProvinceNo { get; set; }

        /// <summary>
        /// 该省下所有的市
        /// </summary>
        public virtual ICollection<M_City> Citys { get; set; }

        /// <summary>
        /// 该省下所有的物业小区
        /// </summary>
        public virtual ICollection<T_PropertyPlace> PropertyPlaces { get; set; }
    }
}
