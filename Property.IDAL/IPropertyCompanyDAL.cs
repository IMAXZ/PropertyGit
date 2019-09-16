using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.Entity;

namespace Property.IDAL
{

    /// <summary>
    /// 物业公司数据层接口
    /// </summary>
    public interface IPropertyCompanyDAL:IBaseDAL<T_Company>
    {
        /// <summary>
        /// 添加公司，指定系统角色
        /// </summary>
        /// <param name="company">公司实体对象</param>
        /// <returns></returns>
        bool AddCompany(T_Company company);

        /// <summary>
        /// 删除标识改为删除，删除系统角色
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        bool DeleteCompany(T_Company company);
    }
}
