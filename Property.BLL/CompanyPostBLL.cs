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
    /// 总公司新闻公告业务层访问类
    /// </summary>
    public class CompanyPostBLL : BaseBLL<T_CompanyPost>, ICompanyPostBLL
    {
        private const string _Type = "CompanyPostDAL";

        private ICompanyPostDAL _Dal;

        public CompanyPostBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICompanyPostDAL;
        }
    }
}
