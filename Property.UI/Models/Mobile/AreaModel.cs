using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{

    /// <summary>
    /// 地区模型（省市区）
    /// </summary>
    public class AreaModel
    {
        /// <summary>
        /// 省份ID
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// 县区ID
        /// </summary>
        public int? CountyId { get; set; }
    }

    /// <summary>
    /// 省份模型
    /// </summary>
    public class ProvinceModel
    {
        /// <summary>
        /// 省份ID
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市列表
        /// </summary>
        public List<CityModel> Citys { get; set; }
    }

    /// <summary>
    /// 市模型
    /// </summary>
    public class CityModel
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 所有的县区
        /// </summary>
        public List<CountyModel> Countys { get; set; }
    }

    /// <summary>
    /// 县区模型
    /// </summary>
    public class CountyModel
    {
        /// <summary>
        /// 县区ID
        /// </summary>
        public int CountyId { get; set; }

        /// <summary>
        /// 县区名称
        /// </summary>
        public string CountyName { get; set; }
    }
}