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
    /// 物业客户端推送实体类
    /// </summary>
    public class T_PropertyUserPush
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RegistrationId { get; set; }

        /// <summary>
        /// 物业用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 关联的物业用户
        /// </summary>
        [ForeignKey("UserId")]
        public T_PropertyUser PropertyUser { get; set; }
    }
}
