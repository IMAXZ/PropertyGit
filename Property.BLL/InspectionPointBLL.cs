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
    /// 巡检点业务层访问类
    /// </summary>
    public class InspectionPointBLL : BaseBLL<T_InspectionPoint>, IInspectionPointBLL
    {
        private const string _Type = "InspectionPointDAL";

        private IInspectionPointDAL _Dal;

        public InspectionPointBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IInspectionPointDAL;
        }
    }
}
