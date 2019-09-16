using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 门店用户业务层接口
    /// </summary>
    public interface IShopUserBLL:IBaseBLL<T_ShopUser>
    {
        /// <summary>
        /// 删除门店用户，并删除其门店
        /// </summary>
        /// <param name="shopUser">门店用户</param>
        /// <returns></returns>
        bool DeleteShopUser(T_ShopUser shopUser);
    }
}
