using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 检索使用的模型，作为其他检索使用模型的基类
    /// </summary>
    public class SearchModel
    {
        /// <summary>
        /// 分页页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 单一检索文本框时候的关键字
        /// </summary>
        public string Kword { get; set; }

        /// <summary>
        /// 判断是否第一次进入，如果第一次的话为null
        /// 如果是不是第一次的话，!=null
        /// </summary>
        public int? IsFirstRequest { get; set; }
    }
}