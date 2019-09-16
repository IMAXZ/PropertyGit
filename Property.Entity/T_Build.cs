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
    /// 楼座表对应实体类
    /// </summary>
    public class T_Build
    {
        public T_Build()
        {
            this.BuildUnits = new HashSet<T_BuildUnit>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string BuildName { get; set; }

        /// <summary>
        /// 所属小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属小区ID关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 楼座描述
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }

        /// <summary>
        /// 该楼座下所有的单元表
        /// </summary>
        public virtual ICollection<T_BuildUnit> BuildUnits { get; set; }
    }
}
