using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 物业总公司角色业务层接口
    /// </summary>
    public interface ICompanyRoleBLL : IBaseBLL<T_CompanyRole>
    {
        /// <summary>
        /// 为指定物业总公司角色分配权限
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        bool ConfigAuth(T_CompanyRole role, List<R_CompanyRoleAction> list);
    }
}
