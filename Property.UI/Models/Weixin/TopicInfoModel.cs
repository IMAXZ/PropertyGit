using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class TopicInfoModel
    {
        public int TopicId { get; set; }
        public string TopicType { get; set; }
        public string TopicTitle { get; set; }
        public string TopicContent { get; set; }
        public string PostUserName { get; set; }
        public string PostUserHeadImg { get; set; }
        public string PostDate { get; set; }
        public string TopicImgList { get; set; }
        public int TopicDiscussTotal { get; set; }
    }
}