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
    /// 物业总公司操作日志业务层访问类
    /// </summary>
    public class CompanyOpreateLogBLL : BaseBLL<T_CompanyOpreateLog>, ICompanyOpreateLogBLL
    {
        private const string _Type = "CompanyOpreateLogDAL";

        private ICompanyOpreateLogDAL _Dal;

        public CompanyOpreateLogBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICompanyOpreateLogDAL;
        }
    }
}
