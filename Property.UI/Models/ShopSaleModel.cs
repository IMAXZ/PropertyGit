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
    /// 商品查询模型
    /// </summary>
    public class ShopSaleSearchModel : SearchModel
    {
        /// <summary>
        /// 商品分类ID
        /// </summary>
        public int? GoodsCategoryId { get; set; }
        /// <summary>
        /// 商品是否上架
        /// </summary>
        public int? InSale { get; set; }

        /// <summary>
        /// 商品分类列表
        /// </summary>
        public List<SelectListItem> GoodsCategoryList { get; set; }
        /// <summary>
        /// 商品状态列表
        /// </summary>
        public List<SelectListItem> GoodsStateList { get; set; }
        /// <summary>
        /// 查询到的商品列表
        /// </summary>
        public PagedList<T_ShopSale> ResultList { get; set; }
    }

    /// <summary>
    /// 商品模型
    /// </summary>
    public class ShopSaleModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 商品介绍
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 商品分类ID
        /// </summary>
        public int? GoodsCategoryId { get; set; }

        /// <summary>
        /// 商品分类列表
        /// </summary>
        public List<SelectListItem> GoodsCategoryList { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainingAmout { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public List<string> PathList { get; set; }
        /// <summary>
        /// 缩略图路径
        /// </summary>
        public List<string> ThumPathList { get; set; }

        /// <summary>
        /// 是否推送
        /// </summary>
        public bool IsPush { get; set; }
    }
}