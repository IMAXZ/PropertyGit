using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 门店业务层接口
    /// </summary>
    public interface IShopBLL : IBaseBLL<T_Shop>
    {
        /// <summary>
        /// 编辑门店，重新设置服务小区
        /// </summary>
        /// <param name="shop">门店记录</param>
        /// <param name="placeIds">服务小区ID</param>
        /// <returns></returns>
        bool UpdateShop(T_Shop shop, string placeIds);

        /// <summary>
        /// 为商家设置支付类型
        /// </summary>
        /// <param name="shop">商家</param>
        /// <param name="list">支付类型列表</param>
        /// <returns>是否分配成功</returns>
        bool SetupPayTypes(T_Shop shop, List<T_ShopPaymentManagement> list);
    }
}
