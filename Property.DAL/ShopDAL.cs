using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 门店数据层访问类
    /// </summary>
    public class ShopDAL : BaseDAL<T_Shop>, IShopDAL
    {
        /// <summary>
        /// 编辑门店，重新设置服务小区
        /// </summary>
        /// <param name="shop">门店记录</param>
        /// <param name="placeIds">服务小区ID</param>
        /// <returns></returns>
        public bool UpdateShop(T_Shop shop, string placeIds)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该门店对应的服务小区关联对象
                    this.nContext.Database.ExecuteSqlCommand("delete from R_ShopPlace where ShopId=" + shop.Id);

                    //重新添加服务小区
                    //如果服务小区不为空
                    if (!string.IsNullOrEmpty(placeIds))
                    {
                        foreach (var placeId in placeIds.Split(','))
                        {
                            shop.ShopPlaces.Add(new R_ShopPlace()
                            {
                                PropertyPlaceId = Convert.ToInt32(placeId)
                            });
                        }
                    }
                    //更新
                    base.Update(shop);
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

        /// <summary>
        /// 为商家设置支付类型
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SetupPayTypes(T_Shop shop, List<T_ShopPaymentManagement> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该商家所有支付类型
                    this.nContext.Database.ExecuteSqlCommand("delete from T_ShopPaymentManagement where shopId=" + shop.Id);
                    //重新设置类型
                    foreach (var item in list)
                    {
                        shop.ShopPaymentManagements.Add(item);
                    }
                    //更新
                    base.Update(shop);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return true;
        }
    }
}
