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
    /// 沟通列表业务逻辑层访问类
    /// </summary>
    public class PostBarTopicBLL : BaseBLL<T_PostBarTopic>, IPostBarTopicBLL
    {
        private const string _Type = "PostBarTopicDAL";

        private IPostBarTopicDAL _Dal;

        public PostBarTopicBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPostBarTopicDAL;
        }

        public IEnumerable<T_PostBarTopic> GetSetTopPageList(Expression<Func<T_PostBarTopic, bool>> where, int pageIndex, int pageSize)
        {
            return this._Dal.GetSetTopPageList(where, pageIndex, pageSize);
        }

        public bool DeleteTopic(int topicId)
        {
            return this._Dal.DeleteTopic(topicId);
        }
    }
}
