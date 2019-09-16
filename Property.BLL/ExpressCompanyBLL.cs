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
    /// 快递公司 业务层访问类
    /// </summary>
    public class ExpressCompanyBLL : BaseBLL<T_ExpressCompany>, IExpressCompanyBLL
    {
        private const string _Type = "ExpressCompanyDAL";

        private IExpressCompanyDAL _Dal;

        public ExpressCompanyBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IExpressCompanyDAL;
        }
    }
}
