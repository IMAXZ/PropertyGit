using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 主题讨论表 业务层数据接口
    /// </summary>
    public interface IPostBarTopicDiscussBLL : IBaseBLL<T_PostBarTopicDiscuss>
    {
        bool DeleteLevelOneDiscuss(int replyId);
    }
}
