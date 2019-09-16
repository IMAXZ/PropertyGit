using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 主题讨论表数据层接口
    /// </summary>
    public interface IPostBarTopicDiscussDAL : IBaseDAL<T_PostBarTopicDiscuss>
    {
        bool DeleteLevelOneDiscuss(int replyId);
    }
}
