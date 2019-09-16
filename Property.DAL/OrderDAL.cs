using Property.Common;
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
    /// 订单表 数据层访问类
    /// </summary>
    public class OrderDAL : BaseDAL<T_Order>, IOrderDAL
    {
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="order">订单模型</param>
        /// <param name="goodsDic">订单中的商品数据</param>
        /// <returns>是否修改成功</returns>
        public bool UpdateOrder(T_Order order, Dictionary<int, int> goodsDic)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    //更新商品剩余数量
                    foreach (var Goods in order.OrderDetails)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.ShopSaleId);
                        sale.RemainingAmout = sale.RemainingAmout + Goods.SaledAmount;
                        saleBll.Update(sale);
                    }

                    //删除该订单中的商品详细关联数据
                    this.nContext.Database.ExecuteSqlCommand("delete from T_OrderDetails where OrderId=" + order.Id);

                    //重新给订单添加商品
                    foreach (var Goods in goodsDic)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.Key);
                        if (sale != null)
                        {
                            order.OrderDetails.Add(new T_OrderDetails()
                            {
                                ShopSaleId = Goods.Key,
                                SaledAmount = Goods.Value,
                                Price = sale.Price * Goods.Value
                            });
                            sale.RemainingAmout = sale.RemainingAmout + Goods.Value;
                            saleBll.Update(sale);
                        }
                    }
                    //更新
                    base.Update(order);
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
        /// 取消订单
        /// </summary>
        /// <param name="order">要取消的订单实体对象</param>
        /// <returns></returns>
        public bool CancelOrder(T_Order order) 
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    //更新商品剩余数量
                    foreach (var Goods in order.OrderDetails)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.ShopSaleId);
                        sale.RemainingAmout = sale.RemainingAmout + Goods.SaledAmount;
                        saleBll.Update(sale);
                    }
                    //更新
                    base.Update(order);
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
        /// 提交订单
        /// </summary>
        /// <param name="order">要提交的订单</param>
        public bool SubmitOrder(T_Order order)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    IShopSaleBLL saleBll = BLLFactory<IShopSaleBLL>.GetBLL("ShopSaleBLL");
                    //更新商品剩余数量
                    foreach (var Goods in order.OrderDetails)
                    {
                        var sale = saleBll.GetEntity(s => s.Id == Goods.ShopSaleId);
                        sale.RemainingAmout = sale.RemainingAmout - Goods.SaledAmount;
                        saleBll.Update(sale);
                    }
                    //更新
                    base.Save(order);
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
