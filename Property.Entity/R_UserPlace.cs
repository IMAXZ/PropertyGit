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
    /// 业主小区关联表
    /// </summary>
    public class R_UserPlace
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 外键 业主信息表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User User { get; set; }

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
