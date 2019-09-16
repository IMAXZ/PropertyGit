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
    /// 手机业主用户推送信息表
    /// </summary>
    public class T_UserPush
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
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 关联的业主用户
        /// </summary>
        [ForeignKey("UserId")]
        public T_User User { get; set; }
    }
}
