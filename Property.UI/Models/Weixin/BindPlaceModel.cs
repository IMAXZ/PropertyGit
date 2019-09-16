using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 绑定小区模型
    /// </summary>
    public class BindPlaceModel
    {
        /// <summary>
        /// 绑定的小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 绑定的小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 所属公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 验证状态 -1. 未申请 0.审核中 1.通过  2.驳回
        /// </summary>
        public int VerifyStatus { get; set; }
    }

    /// <summary>
    /// 小区绑定提交模型
    /// </summary>
    public class PlaceAddSubmitModel
    {
        /// <summary>
        /// 所选城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 城市下拉列表
        /// </summary>
        public List<SelectCityModel> CityList { get; set; }

        /// <summary>
        /// 要绑定的小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 小区下拉列表
        /// </summary>
        public List<PlaceListModel> PlaceList { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId { get; set; }
    }

    /// <summary>
    /// 小区列表模型
    /// </summary>
    public class PlaceListModel 
    {
        /// <summary>
        /// 小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 小区名称首字母
        /// </summary>
        public string FirstLetter { get; set; }
    }

    /// <summary>
    /// 小区绑定提交模型
    /// </summary>
    public class PlaceIdentityVerifyModel
    {
        /// <summary>
        /// 要绑定的小区ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 小区类型 0：住宅小区 1：办公楼小区
        /// </summary>
        public int PlaceType { get; set; }

        /// <summary>
        /// 楼座下拉列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }

        /// <summary>
        /// 楼座下拉列表
        /// </summary>
        public List<SelectListItem> BuildCompanyList { get; set; }

        /// <summary>
        /// 楼座（单位）ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 单元下拉列表
        /// </summary>
        public List<SelectListItem> UnitList { get; set; }

        /// <summary>
        /// 单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 户下拉列表
        /// </summary>
        public List<SelectListItem> DoorList { get; set; }

        /// <summary>
        /// 绑定小区身份验证 户ID
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 业主电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerityCode { get; set; }

    }

    /// <summary>
    /// 选择城市模型
    /// </summary>
    public class SelectCityModel
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityId{get;set;}

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName{get;set;}

        /// <summary>
        /// 小区个数
        /// </summary>
        public int PlaceCount { get; set; }
    }
}