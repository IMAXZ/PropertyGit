using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class QuestionDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public int Status { get; set; }
        public string UploadTime { get; set; }
        public string[] Imgs { get; set; }
        public string PropertyName { get; set; }
        public string DisposesTime { get; set; }
        public string AudioPath { get; set; }
        public int? VoiceDuration { get; set; }
        public string DisposeDesc { get; set; }

    }
}