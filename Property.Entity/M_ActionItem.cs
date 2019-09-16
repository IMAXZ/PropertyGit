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
    /// 权限包含详细
    /// </summary>
    public class M_ActionItem
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ItemName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [MaxLength(300)]
        public string ItemDesc { get; set; }

        /// <summary>
        /// 链接Url地址
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string Href { get; set; }

        /// <summary>
        /// 所属权限ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 所属权限组ID 关联表
        /// </summary>
        [ForeignKey("ActionId")]
        public virtual M_Action Action { get; set; }
    }
}
