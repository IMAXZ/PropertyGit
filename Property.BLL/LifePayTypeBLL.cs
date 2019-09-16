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
    /// 生活支付方式 业务层访问类
    /// </summary>
    public class LifePayTypeBLL : BaseBLL<T_LifePayType>, ILifePayTypeBLL
    {
        private const string _Type = "LifePayTypeDAL";

        private ILifePayTypeDAL _Dal;

        public LifePayTypeBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ILifePayTypeDAL;
        }
    }
}
