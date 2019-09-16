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
    /// APP注册用户收货地址业务层访问类
    /// </summary>
    public class AppUserShippingAddressBLL : BaseBLL<T_AppUserShippingAddress>, IAppUserShippingAddressBLL
    {
        private const string _Type = "AppUserShippingAddressDAL";

        private IAppUserShippingAddressDAL _Dal;

        public AppUserShippingAddressBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IAppUserShippingAddressDAL;
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        /// <param name="address">收货地址实体对象</param>
        public bool SetDefault(T_AppUserShippingAddress address) 
        {
            return this._Dal.SetDefault(address);
        }
    }
}
