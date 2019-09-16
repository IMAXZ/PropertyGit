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
    /// <summary>
    /// 门店促销业务层访问类
    /// </summary>
    public class ShopSaleBLL:BaseBLL<T_ShopSale>,IShopSaleBLL
    {
        private const string _Type = "ShopSaleDAL";

        private IShopSaleDAL _Dal;

        public ShopSaleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopSaleDAL;
        }
    }
}
