using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    public class GoodsCategoryInfoModel : TokenModel
    {
        public int? Id { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
    }

    public class GoodsCategorySearchModel : PagedSearchModel
    {
        public int ShopId { get; set; }
    }

    public class GoodsSearchModel : PagedSearchModel
    {
        public int ShopId { get; set; }
        public int InSales { get; set; }
        public int GoodsCategoryId { get; set; }
    }

    public class GoodsInfoModel : TokenModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string PicList { get; set; }
        public double Price { get; set; }
        public int RemainingAmount { get; set; }
        public int GoodCategoryId { get; set; }
        public int ShopId { get; set; }
        public int InSales { get; set; }
        public string delPicList { get; set; }
        public int IsPush { get; set; }
    }
}