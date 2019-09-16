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
    /// 生活记账 业务层访问类
    /// </summary>
    public class LifeBillBLL:BaseBLL<T_LifeBill>,ILifeBillBLL
    {
        private const string _Type = "LifeBillDAL";

        private ILifeBillDAL _Dal;

        public LifeBillBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ILifeBillDAL;
        }
    }
}
