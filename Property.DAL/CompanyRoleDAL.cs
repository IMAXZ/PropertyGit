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
    /// 物业总公司角色数据层访问类
    /// </summary>
    public class CompanyRoleDAL : BaseDAL<T_CompanyRole>, ICompanyRoleDAL
    {
        /// <summary>
        /// 为指定物业总公司角色分配权限
        /// </summary>
        /// <param name="role">角色数据实体</param>
        /// <param name="list">角色权限关联对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool ConfigAuth(T_CompanyRole role, List<R_CompanyRoleAction> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该角色下的对应权限关联
                    this.nContext.Database.ExecuteSqlCommand("delete from R_CompanyRoleAction where RoleId=" + role.Id);
                    //重新分配权限
                    foreach (var item in list)
                    {
                        role.CompanyRoleActions.Add(item);
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
