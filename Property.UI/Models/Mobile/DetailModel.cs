using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 问题、公告等详细查询模型
    /// </summary>
    public class DetailSearchModel : TokenModel
    {
        /// <summary>
        /// 要查询的详细ID
        /// </summary>
        public int Id { get; set; }
    }
}