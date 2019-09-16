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
    /// 缴费编号业务层访问类
    /// </summary>
    public class PropertyExpenseNoBLL : BaseBLL<T_PropertyExpenseNo>, IPropertyExpenseNoBLL
    {
        private const string _Type = "PropertyExpenseNoDAL";

        private IPropertyExpenseNoDAL _Dal;

        public PropertyExpenseNoBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyExpenseNoDAL;
        }
    }
}
