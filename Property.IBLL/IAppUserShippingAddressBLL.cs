using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// APP注册用户收货地址业务层访问接口
    /// </summary>
    public interface IAppUserShippingAddressBLL : IBaseBLL<T_AppUserShippingAddress>
    {
        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        /// <param name="address">收货地址实体对象</param>
        bool SetDefault(T_AppUserShippingAddress address);
    }
}
