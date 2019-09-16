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
    /// 住宅业主信息登记表对应实体类
    /// </summary>
    public class T_HouseUser
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Nullable<int> Gender { get; set; }

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
        /// 单元户ID
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 所属单元户ID关联表对象
        /// </summary>
        [ForeignKey("DoorId")]
        public virtual T_BuildDoor BuildDoor { get; set; }

        /// <summary>
        /// 业主备注
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }

        /// <summary>
        /// 缴费备注
        /// </summary>
        [MaxLength(500)]
        public string PayDesc { get; set; }

        /// <summary>
        /// 服务备注
        /// </summary>
        [MaxLength(500)]
        public string ServiceDesc { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }
    }
}
