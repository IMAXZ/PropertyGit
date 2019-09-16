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
    /// 门店用户数据层访问类
    /// </summary>
    public class ShopUserDAL : BaseDAL<T_ShopUser>, IShopUserDAL
    {
        /// <summary>
        /// 删除门店用户，并删除其门店
        /// </summary>
        /// <param name="shopUser">门店用户</param>
        /// <returns></returns>
        public bool DeleteShopUser(T_ShopUser shopUser)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除对应的门店
                    this.nContext.Database.ExecuteSqlCommand("delete from T_Shop where ShopUserId=" + shopUser.Id);

                    //修改门店用户为删除标识
                    base.Update(shopUser);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
