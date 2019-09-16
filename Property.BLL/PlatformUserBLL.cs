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
    /// 平台用户业务层访问类
    /// </summary>
    public class PlatformUserBLL : BaseBLL<T_PlatformUser>, IPlatformUserBLL
    {
        private const string _Type = "PlatformUserDAL";

        private IPlatformUserDAL _Dal;

        public PlatformUserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPlatformUserDAL;
        }

        /// <summary>
        /// 为指定用户分配角色
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_PlatformUser user, List<R_PlatformUserRole> list)
        {
            return this._Dal.ConfigRole(user, list);
        }
    }
}
