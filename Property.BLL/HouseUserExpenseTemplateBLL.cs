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
    public class HouseUserExpenseTemplateBLL : BaseBLL<T_HouseUserExpenseTemplate>, IHouseUserExpenseTemplateBLL
    {
        private const string _Type = "HouseUserExpenseTemplateDAL";

        private IHouseUserExpenseTemplateDAL _Dal;

        public HouseUserExpenseTemplateBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IHouseUserExpenseTemplateDAL;
        }

        public bool UpdateExpenseTemplate(string doorIds, List<T_HouseUserExpenseTemplate> houseUserExpenseTemplatelist,int expenseTypeId)
        {
            return this._Dal.UpdateExpenseTemplate(doorIds, houseUserExpenseTemplatelist, expenseTypeId);
        }
    }
}
