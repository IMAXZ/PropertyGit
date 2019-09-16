using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class PropertyNoticeDetailModel
    {
        public int Id { get; set; }
        public string PlaceName { get; set; }
        public string PublishedTime { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}