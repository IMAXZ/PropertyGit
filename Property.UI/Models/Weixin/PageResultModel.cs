using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    public class PageResultModel
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 成功返回数据
        /// </summary>
        public object Result { get; set; }
    }

    public class JsonResultModel : JsonModel
    {
        /// <summary>
        /// 成功返回数据
        /// </summary>
        public object Result { get; set; }
    }
}