using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 物业角色数据层接口
    /// </summary>
    public interface IPropertyRoleDAL : IBaseDAL<T_PropertyRole>
    {
        /// <summary>
        /// 为指定角色分配权限
        /// </summary>
        /// <param name="role">角色数据实体</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        bool ConfigAuth(T_PropertyRole role, List<R_PropertyRoleAction> list);
    }
}
