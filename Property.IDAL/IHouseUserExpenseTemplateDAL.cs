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
    public interface IHouseUserExpenseTemplateDAL : IBaseDAL<T_HouseUserExpenseTemplate>
    {
        bool UpdateExpenseTemplate(string doorIds, List<T_HouseUserExpenseTemplate> houseUserExpenseTemplatelist,int expenseTypeId);
    }
}
