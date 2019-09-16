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
    public class PropertyUserBLL : BaseBLL<T_PropertyUser>, IPropertyUserBLL
    {
        /// <summary>
        /// 物业用户业务访问层
        /// </summary>
        private const string _Type = "PropertyUserDAL";

        private IPropertyUserDAL _Dal;

        public PropertyUserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyUserDAL;
        }

        /// <summary>
        /// 为指定用户分配角色
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_PropertyUser user, List<R_PropertyUserRole> list)
        {
            return this._Dal.ConfigRole(user, list);
        }
    }
}
