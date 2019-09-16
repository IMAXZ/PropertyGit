using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 意见反馈模型
    /// </summary>
    public class FeedbackModel : TokenModel
    {
        /// <summary>
        /// 意见内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Base64图片列表
        /// </summary>
        public string PicList { get; set; }
    }
}