using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.IDAL;
using Property.Entity;
using Property.IBLL;
using Property.FactoryBLL;
using Property.Common;

namespace Property.DAL
{

    /// <summary>
    /// 物业公司数据层访问类
    /// </summary>
    public class PropertyCompanyDAL : BaseDAL<T_Company>, IPropertyCompanyDAL
    {
        /// <summary>
        /// 添加公司，指定系统角色
        /// </summary>
        /// <param name="company">公司实体对象</param>
        /// <returns></returns>
        public bool AddCompany(T_Company company)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //添加公司
                    var newCompany = base.Save(company);
                    if (newCompany != null)
                    {
                        //给公司设定系统角色
                        ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
                        //初始化物业系统角色数据实体
                        T_CompanyRole role = new T_CompanyRole()
                        {
                            RoleName = ConstantParam.COMPANY_SYSTEM_ROLE_NAME,
                            RoleMemo = ConstantParam.COMPANY_SYSTEM_ROLE_MEMO,
                            CompanyId = newCompany.Id,
                            IsSystem = ConstantParam.USER_ROLE_MGR
                        };
                        //保存
                        companyRoleBll.Save(role);

                        //提交事务
                        tran.Commit();
                    }
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 删除公司，删除其角色和用户
        /// </summary>
        /// <param name="company">公司实体对象</param>
        /// <returns></returns>
        public bool DeleteCompany(T_Company company)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //修改为删除标识
                    ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
                    var users = companyUserBll.GetList(u => u.CompanyId == company.Id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList();
                    foreach (var user in users)
                    {
                        user.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                        companyUserBll.Update(user);
                    }

                    //改为删除标识
                    base.Update(company);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
