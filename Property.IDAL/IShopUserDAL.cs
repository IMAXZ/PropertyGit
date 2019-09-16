using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 门店用户数据层接口
    /// </summary>
    public interface IShopUserDAL:IBaseDAL<T_ShopUser>
    {
        /// <summary>
        /// 删除门店用户，并删除其门店
        /// </summary>
        /// <param name="shopUser">门店用户</param>
        /// <returns></returns>
        bool DeleteShopUser(T_ShopUser shopUser);
    }
}
