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
    /// 门店用户业务层访问类
    /// </summary>
    public class ShopUserBLL : BaseBLL<T_ShopUser>, IShopUserBLL
    {
        private const string _Type = "ShopUserDAL";

        private IShopUserDAL _Dal;

        public ShopUserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopUserDAL;
        }

        /// <summary>
        /// 删除门店用户，并删除其门店
        /// </summary>
        /// <param name="shopUser">门店用户</param>
        /// <returns></returns>
        public bool DeleteShopUser(T_ShopUser shopUser)
        {
            return this._Dal.DeleteShopUser(shopUser);
        }
    }
}
