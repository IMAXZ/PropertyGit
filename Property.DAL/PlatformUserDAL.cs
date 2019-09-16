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
    /// 平台用户数据层访问类
    /// </summary>
    public class PlatformUserDAL : BaseDAL<T_PlatformUser>, IPlatformUserDAL
    {
        /// <summary>
        /// 为指定用户分配角色
        /// </summary>
        /// <param name="user">用户数据实体</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_PlatformUser user, List<R_PlatformUserRole> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该用户所有对应角色关联
                    this.nContext.Database.ExecuteSqlCommand("delete from R_PlatformUserRole where UserId=" + user.Id);
                    //重新分配角色
                    foreach (var item in list)
                    {
                        user.PlatformUserRoles.Add(item);
                    }
                    //更新
                    base.Update(user);
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
