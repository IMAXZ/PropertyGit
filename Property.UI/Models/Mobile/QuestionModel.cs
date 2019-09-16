using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 提报问题模型
    /// </summary>
    public class QuestionModel : TokenModel
    {
        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 语音Base64文件
        /// </summary>
        public string VoiceFile { get; set; }

        /// <summary>
        /// 语音时长（s）
        /// </summary>
        public int? VoiceDuration { get; set; }

        /// <summary>
        /// 问题Base64图片列表
        /// </summary>
        public string PicList { get; set; }


        /// <summary>
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }
    }
}