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
    /// 物业用户数据访问层
    /// </summary>
    public class PropertyUserDAL : BaseDAL<T_PropertyUser>, IPropertyUserDAL
    {

        /// <summary>
        /// 为指定用户分配角色
        /// </summary>
        /// <param name="user">用户数据实体</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_PropertyUser user, List<R_PropertyUserRole> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该用户所有对应角色关联
                    this.nContext.Database.ExecuteSqlCommand("delete from R_PropertyUserRole where UserId=" + user.Id);
                    //重新分配权限
                    foreach (var item in list)
                    {
                        user.PropertyUserRoles.Add(item);
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
