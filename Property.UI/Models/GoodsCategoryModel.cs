using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    public class GoodsCategoryModel:SearchModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 所属商家 
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 查询到的结果
        /// </summary>
        public PagedList<T_GoodsCategory> DataList { get; set; }
    }
}