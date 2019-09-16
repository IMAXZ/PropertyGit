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
    /// 门店服务小区关联表实体对象
    /// </summary>
    public class R_ShopPlace
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 外键 门店表
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual T_Shop Shop { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 外键 物业小区表
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }
    }
}
