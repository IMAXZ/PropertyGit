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
    /// 生活记账分类 业务层访问类
    /// </summary>
    public class LifeBillTypeBLL:BaseBLL<T_LifeBillType>,ILifeBillTypeBLL
    {
        private const string _Type = "LifeBillTypeDAL";

        private ILifeBillTypeDAL _Dal;

        public LifeBillTypeBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ILifeBillTypeDAL;
        }
    }
}
