using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace Property.DAL
{
    /// <summary>
    /// 巡检结果数据层访问类
    /// </summary>
    public class InspectionResultDAL : BaseDAL<T_InspectionResult>, IInspectionResultDAL
    {
        /// <summary>
        /// 处理巡检异常
        /// </summary>
        /// <param name="result">巡检结果</param>
        /// <param name="ExceptionDispose">处理记录实体对象</param>
        public void DisposeException(T_InspectionResult result, T_InspectionExceptionDispose ExceptionDispose)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //更新巡检结果
                    base.Update(result);

                    //添加巡检异常处理记录
                    IInspectionExceptionDisposeBLL exceptionBll = BLLFactory<IInspectionExceptionDisposeBLL>.GetBLL("InspectionExceptionDisposeBLL");

                    var eDispose = exceptionBll.GetEntity(dq => dq.ExceptionResultId == result.Id);
                    if (eDispose == null)
                    {
                        exceptionBll.Save(ExceptionDispose);
                    }
                    else
                    {
                        eDispose.DisposeDesc = ExceptionDispose.DisposeDesc;
                        eDispose.DisposeTime = DateTime.Now;
                        exceptionBll.Update(eDispose);
                    }
                    exceptionBll.Save(ExceptionDispose);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
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
            return this.nContext.T_InspectionResult.Where(where).OrderBy(u => u.DisposerId == null ? 0 : 1).ThenByDescending(u => u.UploadTime).ToPagedList(pageIndex, pageSize);
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
            return this.nContext.T_InspectionResult.Where(where).OrderBy(u => u.DisposeStatus == null ? null : u.DisposeStatus).ThenByDescending(u => u.UploadTime).ToPagedList(pageIndex, pageSize);
        }
    }
}
