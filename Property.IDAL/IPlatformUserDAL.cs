using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 数据访问层接口类
    /// </summary>
    public interface IPlatformUserDAL : IBaseDAL<T_PlatformUser>
    {
        /// <summary>
        /// 为指定用户分配角色
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        bool ConfigRole(T_PlatformUser user, List<R_PlatformUserRole> list);
    }
}
