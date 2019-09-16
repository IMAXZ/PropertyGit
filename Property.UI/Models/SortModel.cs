using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 排序帮助模型
    /// </summary>
    public class SortModel
    {
        /// <summary>
        /// 排序的名称
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// 是否升序
        /// </summary>
        public bool IsAsc { get; set; }
    }
}