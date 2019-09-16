using Property.Entity;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// APP注册用户收货地址访问类
    /// </summary>
    public class AppUserShippingAddressDAL : BaseDAL<T_AppUserShippingAddress>, IAppUserShippingAddressDAL
    {
        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        /// <param name="address">收货地址实体对象</param>
        public bool SetDefault(T_AppUserShippingAddress address)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //将该地址所属用户的默认地址修改为非默认
                    this.nContext.Database.ExecuteSqlCommand("update T_AppUserShippingAddress set IsDefault = 0 where IsDefault = 1 and AppUserId =" + address.AppUserId);

                    //修改地址为默认
                    address.IsDefault = 1;
                    base.Update(address);
                    //提交事务
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
    }
}
