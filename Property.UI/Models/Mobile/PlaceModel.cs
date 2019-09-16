using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 根据小区查询模型
    /// </summary>
    public class PlaceModel : TokenModel
    {
        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }
    }
}