using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 沟通列表业务层接口
    /// </summary>
    public interface IPostBarTopicBLL : IBaseBLL<T_PostBarTopic>
    {
        IEnumerable<T_PostBarTopic> GetSetTopPageList(Expression<Func<T_PostBarTopic, bool>> where, int pageIndex, int pageSize);
        bool DeleteTopic(int topicId);
    }
}
