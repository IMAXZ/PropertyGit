using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.IDAL;
using Property.Entity;
using Property.IBLL;
using Property.FactoryBLL;
using System.Linq.Expressions;
using Webdiyer.WebControls.Mvc;

namespace Property.DAL
{
    /// <summary>
    /// 业主上报问题数据层访问类
    /// </summary>
    public class QuestionDAL : BaseDAL<T_Question>, IQuestionDAL
    {
        /// <summary>
        /// 处理业主上报问题
        /// </summary>
        /// <param name="status">问题新状态</param>
        /// <param name="QuestionDispose">处理记录对象</param>
        public void DisposeQuestion(T_Question Question, T_QuestionDispose QuestionDispose)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //更新上报问题
                    base.Update(Question);

                    //添加处理记录
                    IDisposeQuestionBLL questionBll = BLLFactory<IDisposeQuestionBLL>.GetBLL("DisposeQuestionBLL");
                    var qd = questionBll.GetEntity(dq => dq.QuestionId == Question.Id);

                    if (qd == null)
                    {
                        questionBll.Save(QuestionDispose);
                    }
                    else
                    {
                        qd.DisposeDesc = QuestionDispose.DisposeDesc;
                        qd.DisposeTime = DateTime.Now;
                        questionBll.Update(qd);
                    }

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
        /// 排序 - 获取要指派处理人的问题
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<T_Question> GetQuestionPageList(Expression<Func<T_Question, bool>> where, int pageIndex, int pageSize)
        {
            return this.nContext.T_Question.Where(where).OrderBy(u => u.DisposerId == null ? 0 : 1).ThenByDescending(u => u.UploadTime).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// 排序 - 获取处理人为自己的问题
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<T_Question> GetOwnQuestionPageList(Expression<Func<T_Question, bool>> where, int pageIndex, int pageSize)
        {
            return this.nContext.T_Question.Where(where).OrderBy(u => u.Status).ThenByDescending(u => u.UploadTime).ToPagedList(pageIndex, pageSize);
        }
    }
}
