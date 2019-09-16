using Property.Entity;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 菜单数据层访问类
    /// </summary>
    public class PlatformRoleDAL : BaseDAL<T_PlatformRole>, IPlatformRoleDAL
    {
        /// <summary>
        /// 为指定角色分配权限
        /// </summary>
        /// <param name="role">角色数据实体</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigAuth(T_PlatformRole role, List<R_PlatformRoleAction> list) 
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该角色下的对应权限关联
                    this.nContext.Database.ExecuteSqlCommand("delete from R_PlatformRoleAction where RoleId=" + role.Id);
                    //重新分配权限
                    foreach (var item in list) 
                    {
                        role.PlatformRoleActions.Add(item);
                    }
                    //更新
                    base.Update(role);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return true;
        }
    }
}
