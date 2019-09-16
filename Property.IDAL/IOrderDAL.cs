using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 订单表 数据层接口
    /// </summary>
    public interface IOrderDAL : IBaseDAL<T_Order>
    {
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="order">订单模型</param>
        /// <param name="goodsDic">订单中的商品数据</param>
        /// <returns>是否修改成功</returns>
        bool UpdateOrder(T_Order order, Dictionary<int, int> goodsDic);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order">要取消的订单实体对象</param>
        /// <returns></returns>
        bool CancelOrder(T_Order order);

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="order"></param>
        bool SubmitOrder(T_Order order);
    }
}
