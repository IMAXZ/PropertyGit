using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 数据访问层接口类
    /// </summary>
    public interface IHouseUserExpenseDetailsDAL : IBaseDAL<T_HouseUserExpenseDetails>
    {
        List<T_HouseUserExpenseDetails> SaveExpenseDetails(List<T_HouseUserExpenseTemplate> expenseTemplateList, int expensePeriod);
    }
}
