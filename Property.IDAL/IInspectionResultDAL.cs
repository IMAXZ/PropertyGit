using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 巡检结果数据层接口
    /// </summary>
    public interface IInspectionResultDAL : IBaseDAL<T_InspectionResult>
    {
        /// <summary>
        /// 处理巡检异常
        /// </summary>
        /// <param name="result">巡检结果</param>
        /// <param name="ExceptionDispose">处理记录实体对象</param>
        void DisposeException(T_InspectionResult result, T_InspectionExceptionDispose ExceptionDispose);


        /// <summary>
        /// 排序 - 获取要指派处理人的巡检异常
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T_InspectionResult> GetInspectionResultPageList(Expression<Func<T_InspectionResult, bool>> where, int pageIndex, int pageSize);

        /// <summary>
        /// 排序 - 获取处理人为自己的巡检异常
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T_InspectionResult> GetOwnInspectionResultPageList(Expression<Func<T_InspectionResult, bool>> where, int pageIndex, int pageSize);
    }
}
