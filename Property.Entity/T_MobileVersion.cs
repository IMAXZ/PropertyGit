using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 客户端版本实体类
    /// </summary>
    public class T_MobileVersion
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 客户端类型 0：业主客户端 1：物业客户端
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int VersionCode { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string VersionName { get; set; }

        /// <summary>
        /// 版本描述
        /// </summary>
        [MaxLength(200)]
        public string Desc { get; set; }

        /// <summary>
        /// APK文件路径
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string ApkFilePath { get; set; }
    }
}
