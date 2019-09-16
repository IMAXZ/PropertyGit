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
    /// 巡检类别（内容）业务层访问类
    /// </summary>
    public class InspectionCategoryBLL : BaseBLL<T_InspectionCategory>, IInspectionCategoryBLL
    {
        private const string _Type = "InspectionCategoryDAL";

        private IInspectionCategoryDAL _Dal;

        public InspectionCategoryBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IInspectionCategoryDAL;
        }
    }
}
