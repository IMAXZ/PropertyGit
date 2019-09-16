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
    /// 物业公司用户数据访问层
    /// </summary>
    public class CompanyUserDAL : BaseDAL<T_CompanyUser>, ICompanyUserDAL
    {
        /// <summary>
        /// 为指定物业总公司用户分配角色
        /// </summary>
        /// <param name="user">用户数据实体</param>
        /// <param name="list">用户角色关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigRole(T_CompanyUser user, List<R_CompanyUserRole> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该物业总公司用户所有对应角色关联
                    this.nContext.Database.ExecuteSqlCommand("delete from R_CompanyUserRole where UserId=" + user.Id);
                    //重新分配权限
                    foreach (var item in list)
                    {
                        user.CompanyUserRoles.Add(item);
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
