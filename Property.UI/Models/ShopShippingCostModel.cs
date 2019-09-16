using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    public class ShopShippingCostModel
    {
        public int? Id { get; set; }
        public double? OrderExpense { get; set; }
        public double? Price { get; set; }
        public int ShopId { get; set; }
        public bool IsFree { get; set; }
    }
}