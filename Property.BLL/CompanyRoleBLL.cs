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
    /// 物业总公司角色业务层访问类
    /// </summary>
    public class CompanyRoleBLL : BaseBLL<T_CompanyRole>, ICompanyRoleBLL
    {
        private const string _Type = "CompanyRoleDAL";

        private ICompanyRoleDAL _Dal;

        public CompanyRoleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICompanyRoleDAL;
        }

        /// <summary>
        /// 为指定物业总公司角色分配权限
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigAuth(T_CompanyRole role, List<R_CompanyRoleAction> list)
        {
            return this._Dal.ConfigAuth(role, list);
        }
    }
}
