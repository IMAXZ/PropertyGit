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
    /// 巡检异常处理记录业务层访问类
    /// </summary>
    public class InspectionExceptionDisposeBLL : BaseBLL<T_InspectionExceptionDispose>, IInspectionExceptionDisposeBLL
    {
        private const string _Type = "InspectionExceptionDisposeDAL";

        private IInspectionExceptionDisposeDAL _Dal;

        public InspectionExceptionDisposeBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IInspectionExceptionDisposeDAL;
        }
    }
}
