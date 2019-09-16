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
    /// 订单表 业务层访问类
    /// </summary>
    public class OrderBLL : BaseBLL<T_Order>,IOrderBLL
    {
        private const string _Type = "OrderDAL";

        private IOrderDAL _Dal;

        public OrderBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IOrderDAL;
        }

        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="order">订单模型</param>
        /// <param name="goodsDic">订单中的商品数据</param>
        /// <returns>是否修改成功</returns>
        public bool UpdateOrder(T_Order order, Dictionary<int, int> goodsDic) 
        {
            return this._Dal.UpdateOrder(order, goodsDic);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order">要取消的订单实体对象</param>
        /// <returns></returns>
        public bool CancelOrder(T_Order order) 
        {
            return this._Dal.CancelOrder(order);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="order">要提交的订单</param>
        public bool SubmitOrder(T_Order order) 
        {
            return this._Dal.SubmitOrder(order);
        }
    }
}
