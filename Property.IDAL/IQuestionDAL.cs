using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.Entity;
using System.Linq.Expressions;

namespace Property.IDAL
{
    /// <summary>
    /// 业主上报问题数据层接口
    /// </summary>
    public interface IQuestionDAL : IBaseDAL<T_Question>
    {
        /// <summary>
        /// 处理业主上报问题
        /// </summary>
        /// <param name="Question">问题新状态</param>
        /// <param name="QuestionDispose">处理记录对象</param>
        void DisposeQuestion(T_Question Question, T_QuestionDispose QuestionDispose);


        /// <summary>
        /// 排序 - 获取要指派处理人的问题
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T_Question> GetQuestionPageList(Expression<Func<T_Question, bool>> where, int pageIndex, int pageSize);

        /// <summary>
        /// 排序 - 获取处理人为自己的问题
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T_Question> GetOwnQuestionPageList(Expression<Func<T_Question, bool>> where, int pageIndex, int pageSize);
    }
}
