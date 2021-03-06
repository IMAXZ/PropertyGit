﻿using Property.Entity;
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
    /// 角色业务层访问类
    /// </summary>
    public class PlatformRoleBLL: BaseBLL<T_PlatformRole>, IPlatformRoleBLL
    {
        private const string _Type = "PlatformRoleDAL";

        private IPlatformRoleDAL _Dal;

        public PlatformRoleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPlatformRoleDAL;
        }

        /// <summary>
        /// 为指定角色分配权限
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigAuth(T_PlatformRole role, List<R_PlatformRoleAction> list) 
        {
            return this._Dal.ConfigAuth(role, list);
        }
    }
}
