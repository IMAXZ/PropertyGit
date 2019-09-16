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
    /// 楼座单元表对应实体类 
    /// </summary>
    public class T_BuildUnit
    {
        public T_BuildUnit()
        {
            this.BuildDoors = new HashSet<T_BuildDoor>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 楼座单元名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UnitName { get; set; }

        /// <summary>
        /// 所属楼座ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 所属楼座ID关联表对象
        /// </summary>
        [ForeignKey("BuildId")]
        public virtual T_Build Build { get; set; }

        /// <summary>
        /// 该单元下的所有单元户
        /// </summary>
        public virtual ICollection<T_BuildDoor> BuildDoors { get; set; }
    }
}
