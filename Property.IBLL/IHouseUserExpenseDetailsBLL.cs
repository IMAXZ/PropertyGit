using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 业务层接口类
    /// </summary>
    public interface IHouseUserExpenseDetailsBLL : IBaseBLL<T_HouseUserExpenseDetails>
    {
        List<T_HouseUserExpenseDetails> SaveExpenseDetails(List<T_HouseUserExpenseTemplate> expenseTemplateList, int expensePeriod);
    }
}
