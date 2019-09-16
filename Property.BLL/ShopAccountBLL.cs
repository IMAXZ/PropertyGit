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
    /// 门店账户业务层访问类
    /// </summary>
    public class ShopAccountBLL:BaseBLL<T_ShopAccounts>,IShopAccountBLL
    {
        private const string _Type = "ShopAccountDAL";

        private IShopAccountDAL _Dal;

        public ShopAccountBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopAccountDAL;
        }
    }
}
