using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 小区沟通主题类别模型
    /// </summary>
    public class TopicTypeModel
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string CategoryName { get; set; }
    }
}