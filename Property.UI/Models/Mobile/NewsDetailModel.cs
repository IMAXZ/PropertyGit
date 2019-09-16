using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 新闻公告详细接口模型
    /// </summary>
    public class NewsDetailModel : TokenModel
    {
        /// <summary>
        /// 公告id
        /// </summary>
        public int PostId { get; set; }
    }
}