using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.ProductUI.Models
{
    /// <summary>
    /// 申请模型
    /// </summary>
    public class ApplyModel
    {
        /// <summary>
        /// 申请单位
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 申请人姓名/称呼
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Tel { get; set; }

        /// <summary>
        /// 其他联系方式
        /// </summary>
        [MaxLength(50)]
        public string OtherContactInfo { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [MaxLength(300)]
        public string Memo { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
    }
}