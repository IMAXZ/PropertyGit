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
    /// 市表
    /// </summary>
    public class M_City
    {
        public M_City() 
        {
            this.Counties = new HashSet<M_County>();
            this.PropertyPlaces = new HashSet<T_PropertyPlace>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// 市名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string CityName { get; set; }


        /// <summary>
        /// 市编号
        /// </summary>
        [MaxLength(30)]
        public string CityNo { get; set; }

        /// <summary>
        /// 所属省份
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 所属省份ID关联表对象
        /// </summary>
        [ForeignKey("ProvinceId")]
        public virtual M_Province Province { get; set; }

        /// <summary>
        /// 该市下所有的县区
        /// </summary>
        public virtual ICollection<M_County> Counties { get; set; }

        /// <summary>
        /// 该市下所有的物业小区
        /// </summary>
        public virtual ICollection<T_PropertyPlace> PropertyPlaces { get; set; }
    }
}
