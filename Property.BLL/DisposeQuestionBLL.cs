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
    /// 问题处理业务层访问类
    /// </summary>
    public class DisposeQuestionBLL : BaseBLL<T_QuestionDispose>, IDisposeQuestionBLL
    {
        private const string _Type = "DisposeQuestionDAL";

        private IDisposeQuestionDAL _Dal;

        public DisposeQuestionBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IDisposeQuestionDAL;
        }
    }
}
