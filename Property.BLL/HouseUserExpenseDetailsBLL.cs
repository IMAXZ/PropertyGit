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
    public class HouseUserExpenseDetailsBLL : BaseBLL<T_HouseUserExpenseDetails>, IHouseUserExpenseDetailsBLL
    {
        private const string _Type = "HouseUserExpenseDetailsDAL";

        private IHouseUserExpenseDetailsDAL _Dal;

        public HouseUserExpenseDetailsBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IHouseUserExpenseDetailsDAL;
        }

        public List<T_HouseUserExpenseDetails> SaveExpenseDetails(List<T_HouseUserExpenseTemplate> expenseTemplateList, int expensePeriod)
        {
            return this._Dal.SaveExpenseDetails(expenseTemplateList, expensePeriod);
        }
    }
}
