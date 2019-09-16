using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.BLL
{
    public class ShopShippingCostBLL : BaseBLL<T_ShopShippingCost>, IShopShippingCostBLL
    {
        private const string _Type = "ShopShippingCostDAL";

        private IShopShippingCostDAL _Dal;

        public ShopShippingCostBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopShippingCostDAL;
        }
    }
}
