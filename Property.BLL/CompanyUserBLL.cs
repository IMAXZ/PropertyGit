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
    public class CompanyUserBLL : BaseBLL<T_CompanyUser>, ICompanyUserBLL
    {
         private const string _Type = "CompanyUserDAL";

        private ICompanyUserDAL _Dal;

        public CompanyUserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICompanyUserDAL;
        }

        /// <summary>
        /// 为指定物业总公司用户分配角色
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_CompanyUser user, List<R_CompanyUserRole> list)
        {
            return this._Dal.ConfigRole(user, list);
        }
    }
}
