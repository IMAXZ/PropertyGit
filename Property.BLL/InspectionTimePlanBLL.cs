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
    /// 巡检时间安排业务层访问类
    /// </summary>
    public class InspectionTimePlanBLL : BaseBLL<T_InspectionTimePlan>, IInspectionTimePlanBLL
    {
        private const string _Type = "InspectionTimePlanDAL";

        private IInspectionTimePlanDAL _Dal;

        public InspectionTimePlanBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IInspectionTimePlanDAL;
        }
    }
}
