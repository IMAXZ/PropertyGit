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
    public interface IHouseUserExpenseTemplateBLL : IBaseBLL<T_HouseUserExpenseTemplate>
    {
        bool UpdateExpenseTemplate(string doorIds, List<T_HouseUserExpenseTemplate> houseUserExpenseTemplatelist, int expenseTypeId);
    }
}
