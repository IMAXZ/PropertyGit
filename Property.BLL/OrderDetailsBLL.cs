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
    /// 订单详细表 业务层访问类
    /// </summary>
    public class OrderDetailsBLL : BaseBLL<T_OrderDetails>,IOrderDetailsBLL
    {
        private const string _Type = "OrderDetailsDAL";

        private IOrderDetailsDAL _Dal;

        public OrderDetailsBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IOrderDetailsDAL;
        }
    }
}
