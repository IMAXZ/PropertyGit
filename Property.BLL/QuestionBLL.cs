using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System.Linq.Expressions;

namespace Property.BLL
{

    /// <summary>
    /// 业主上报问题业务层访问类
    /// </summary>
    public class QuestionBLL : BaseBLL<T_Question>, IQuestionBLL
    {
        private const string _Type = "QuestionDAL";

        private IQuestionDAL _Dal;

        public QuestionBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IQuestionDAL;
        }

        /// <summary>
        /// 处理业主上报问题
        /// </summary>
        /// <param name="Question">问题新状态</param>
        /// <param name="QuestionDispose">处理记录对象</param>
        public void DisposeQuestion(T_Question Question, T_QuestionDispose QuestionDispose) 
        {
            this._Dal.DisposeQuestion(Question, QuestionDispose);
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
            return this._Dal.GetQuestionPageList(where, pageIndex, pageSize);
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
            return this._Dal.GetOwnQuestionPageList(where, pageIndex, pageSize);
        }
    }
}

