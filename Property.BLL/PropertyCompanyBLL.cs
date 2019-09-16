using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.Entity;
using Property.IBLL;
using Property.IDAL;

namespace Property.BLL
{

    /// <summary>
    /// 物业公司业务层访问类
    /// </summary>
    public class PropertyCompanyBLL:BaseBLL<T_Company>,IPropertyCompanyBLL
    {
        private const string _Type = "PropertyCompanyDAL";

        private IPropertyCompanyDAL _Dal;

        public PropertyCompanyBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyCompanyDAL;
        }
        /// <summary>
        /// 添加公司，指定系统角色
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public bool AddCompany(T_Company company)
        {
            return this._Dal.AddCompany(company);
        }

        /// <summary>
        /// 删除标识改为删除，删除系统角色
        /// </summary>
        /// <param name=company"></param>
        /// <returns></returns>
        public bool DeleteCompany(T_Company company)
        {
            return this._Dal.DeleteCompany(company);
        }
    }
}
