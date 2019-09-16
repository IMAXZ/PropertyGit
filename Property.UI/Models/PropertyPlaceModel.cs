using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 物业小区表单模型
    /// </summary>
    public class PropertyPlaceModel
    {
        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PlaceName { get; set; }

        /// <summary>
        /// 所属物业公司ID
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 所属省
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 所属市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 所属县区
        /// </summary>
        public int? CountyId { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 小区介绍
        /// </summary>
        [AllowHtml]
        public string Content { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        public string Tel { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 小区类型（0：住宅小区 1：办公楼小区）
        /// </summary>
        public int PlaceType { get; set; }

        /// <summary>
        /// 是否需要验证
        /// </summary>
        public bool IsValidate { get; set; }

        /// <summary>
        /// 小区类型下拉列表集合
        /// </summary>
        public List<SelectListItem> PlaceTypeList { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        [MaxLength(200)]
        public string Img { get; set; }

        /// <summary>
        /// 物业公司下拉列表
        /// </summary>
        public List<SelectListItem> CompanyList { get; set; }

        /// <summary>
        /// 省份下拉列表
        /// </summary>
        public List<SelectListItem> ProvinceList { get; set; }

        /// <summary>
        /// 城市下拉列表
        /// </summary>
        public List<SelectListItem> CityList { get; set; }

        /// <summary>
        /// 区县下拉列表
        /// </summary>
        public List<SelectListItem> CountyList { get; set; }

    }

    /// <summary>
    /// 物业小区查询模型
    /// </summary>
    public class PropertyPlaceSearchModel : SearchModel
    {
        /// <summary>
        /// 物业小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 所属物业公司ID
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// 小区类型
        /// </summary>
        public int? PlaceType { get; set; }

        /// <summary>
        /// 小区类型下拉列表集合
        /// </summary>
        public List<SelectListItem> PlaceTypeList { get; set; }

        /// <summary>
        /// 物业公司下拉列表
        /// </summary>
        public List<SelectListItem> CompanyList { get; set; }
        public int? PropertyPlaceId { get; set; }
        /// <summary>
        /// 物业小区下拉列表
        /// </summary>
        public List<SelectListItem> PropertyPlaceList { get; set; }

        /// <summary>
        /// 查询到的物业小区数据列表
        /// </summary>
        public PagedList<T_PropertyPlace> DataList { get; set; }
        /// <summary>
        /// 查询到的物业员工数据列表
        /// </summary>
        public PagedList<T_PropertyUser> DataUserList { get; set; }
    }
}