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
    /// 物业角色业务层访问类
    /// </summary>
    public class PropertyRoleBLL : BaseBLL<T_PropertyRole>,IPropertyRoleBLL
    {
        private const string _Type = "PropertyRoleDAL";

        private IPropertyRoleDAL _Dal;

        public PropertyRoleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyRoleDAL;
        }

        /// <summary>
        /// 为指定物业角色分配权限
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigAuth(T_PropertyRole role, List<R_PropertyRoleAction> list) 
        {
            return this._Dal.ConfigAuth(role, list);
        }
    }
}
