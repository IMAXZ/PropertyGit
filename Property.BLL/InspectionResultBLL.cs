using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.BLL
{
    /// <summary>
    /// 巡检结果业务层访问类
    /// </summary>
    public class InspectionResultBLL : BaseBLL<T_InspectionResult>, IInspectionResultBLL
    {
        private const string _Type = "InspectionResultDAL";

        private IInspectionResultDAL _Dal;

        public InspectionResultBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IInspectionResultDAL;
        }

        /// <summary>
        /// 处理巡检异常
        /// </summary>
        /// <param name="result">巡检结果</param>
        /// <param name="ExceptionDispose">处理记录实体对象</param>
        public void DisposeException(T_InspectionResult result, T_InspectionExceptionDispose ExceptionDispose)
        {
            this._Dal.DisposeException(result, ExceptionDispose);
        }


        /// <summary>
        /// 排序 - 获取要指派处理人的巡检异常
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<T_InspectionResult> GetInspectionResultPageList(Expression<Func<T_InspectionResult, bool>> where, int pageIndex, int pageSize)
        {
            return this._Dal.GetInspectionResultPageList(where, pageIndex, pageSize);
        }

        /// <summary>
        /// 排序 - 获取处理人为自己的巡检异常
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<T_InspectionResult> GetOwnInspectionResultPageList(Expression<Func<T_InspectionResult, bool>> where, int pageIndex, int pageSize)
        {
            return this._Dal.GetOwnInspectionResultPageList(where, pageIndex, pageSize);
        }
    }
}
