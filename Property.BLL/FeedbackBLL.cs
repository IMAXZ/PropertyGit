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
    /// 意见反馈 业务层访问类
    /// </summary>
    public class FeedbackBLL : BaseBLL<T_Feedback>, IFeedbackBLL
    {
        private const string _Type = "FeedbackDAL";

        private IFeedbackDAL _Dal;

        public FeedbackBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IFeedbackDAL;
        }
    }
}
