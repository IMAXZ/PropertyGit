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
    /// 门店平台门店编辑模型
    /// </summary>
    public class ShopPlatformModel
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 门店联系电话
        /// </summary>
        [MaxLength(20)]
        public string Tel { get; set; }

        /// <summary>
        /// 门店主营介绍
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string MainSale { get; set; }

        ///// <summary>
        ///// 是否支持上门送货
        ///// </summary>
        //public bool IsDelivery { get; set; }

        /// <summary>
        /// 门店内容介绍
        /// </summary>
        [AllowHtml]
        public string Content { get; set; }

        /// <summary>
        /// 营业时间列表
        /// </summary>
        public List<SelectListItem> TimeList { get; set; }

        /// <summary>
        /// 开始营业时刻
        /// </summary>
        public int StartBusinessTime { get; set; }

        /// <summary>
        /// 结束营业时刻
        /// </summary>
        public int EndBusinessTime { get; set; }

        /// <summary>
        /// 省列表
        /// </summary>
        public List<SelectListItem> ProvinceList { get; set; }

        /// <summary>
        /// 市列表
        /// </summary>
        public List<SelectListItem> CityList { get; set; }

        /// <summary>
        /// 县列表
        /// </summary>
        public List<SelectListItem> CountyList { get; set; }

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
        public int? CountyId { get; set; }
    }


    /// <summary>
    /// 门店模型
    /// </summary>
    public class ShopModel
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public string Types { get; set; }

        /// <summary>
        /// 门店地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 门店联系电话
        /// </summary>
        [MaxLength(20)]
        public string Tel { get; set; }

        /// <summary>
        /// 类型列表
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// 省列表
        /// </summary>
        public List<SelectListItem> ProvinceList { get; set; }

        /// <summary>
        /// 市列表
        /// </summary>
        public List<SelectListItem> CityList { get; set; }

        /// <summary>
        /// 县列表
        /// </summary>
        public List<SelectListItem> CountyList { get; set; }

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
        public int? CountyId { get; set; }

        /// <summary>
        /// 门店主营介绍
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string MainSale { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        public int ShopUserId { get; set; }

        /// <summary>
        /// 所属用户名称
        /// </summary>
        public string ShopUserName { get; set; }

        ///// <summary>
        ///// 是否支持上门送货
        ///// </summary>
        //public bool IsDelivery { get; set; }

        /// <summary>
        /// 门店内容介绍
        /// </summary>
        [AllowHtml]
        public string Content { get; set; }

        /// <summary>
        /// 服务小区Id列表
        /// </summary>
        public string PlaceIds { get; set; }

        /// <summary>
        /// 服务小区名称列表
        /// </summary>
        public string placeNames { get; set; }

        /// <summary>
        /// 服务小区列表
        /// </summary>
        public List<SelectListItem> PlaceList { get; set; }

        /// <summary>
        /// 营业时间列表
        /// </summary>
        public List<SelectListItem> TimeList { get; set; }

        /// <summary>
        /// 开始营业时刻
        /// </summary>
        public int StartBusinessTime { get; set; }

        /// <summary>
        /// 结束营业时刻
        /// </summary>
        public int EndBusinessTime { get; set; }
    }


    /// <summary>
    /// 门店查询模型
    /// </summary>
    public class ShopSearchModel : SearchModel
    {
        /// <summary>
        /// 门店名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public Nullable<int> Type { get; set; }

        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public PagedList<T_Shop> List { get; set; }

        /// <summary>
        /// 类型列表
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }
    }

    /// <summary>
    /// 门店图片模型
    /// </summary>
    public class ShopImgModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 门店图片路径
        /// </summary>
        [MaxLength(200)]
        public string ImgPath { get; set; }

        /// <summary>
        /// 门店缩略图路径
        /// </summary>
        [MaxLength(200)]
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 门店图片路径集合
        /// </summary>
        public List<string> ImgPathArray { get; set; }

        /// <summary>
        /// 门店缩略图路径集合
        /// </summary>
        public List<string> ImgThumbPathArray { get; set; }
    }

    /// <summary>
    /// 封面图片上传模型
    /// </summary>
    public class CoverImgUploadModel
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 封面图片文件
        /// </summary>
        [Required]
        public HttpPostedFileBase CoverImgFile { get; set; }
    }
}