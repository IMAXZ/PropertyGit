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
    /// <summary>
    /// 物业账户业务层访问类
    /// </summary>
    public class PropertyAccountBLL:BaseBLL<T_PropertyAccount>,IPropertyAccountBLL
    {
        private const string _Type = "PropertyAccountDAL";

        private IPropertyAccountDAL _Dal;

        public PropertyAccountBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyAccountDAL;
        }
    }
}
