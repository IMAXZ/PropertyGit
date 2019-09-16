using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    public class T_ExpressCompany
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 快递电话
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Phone { get; set; }

        /// <summary>
        /// Logo图片
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Img { get; set; }

        /// <summary>
        /// 删除标识 0.未删除 1.删除
        /// </summary>
        public int DelFlag { get; set; }
    }
}
