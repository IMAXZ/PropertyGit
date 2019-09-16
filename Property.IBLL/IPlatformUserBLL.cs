using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 用户业务层接口
    /// </summary>
    public interface IPlatformUserBLL : IBaseBLL<T_PlatformUser>
    {

        /// <summary>
        /// 为指定平台用户分配角色
        /// </summary>
        /// <param name="user">平台用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        bool ConfigRole(T_PlatformUser user, List<R_PlatformUserRole> list);
    }
}
