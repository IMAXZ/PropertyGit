using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 业务层接口类
    /// </summary>
    public interface IPropertyUserBLL:IBaseBLL<T_PropertyUser>
    {
        /// <summary>
        /// 为指定平台用户分配角色
        /// </summary>
        /// <param name="user">平台用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        bool ConfigRole(T_PropertyUser user, List<R_PropertyUserRole> list);
    }
}
