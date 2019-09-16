using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class TopicHomePageModel
    {
        public TopicHomePageModel()
        {
            PropertyList = new List<PropertyInfoModel>();
            TopicList = new List<TopicInfoModel>();
        }
        public List<PropertyInfoModel> PropertyList { get; set; }
        public List<TopicInfoModel> TopicList { get; set; }
    }
}