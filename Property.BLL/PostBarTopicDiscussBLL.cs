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
    /// 主题讨论表 业务层访问类
    /// </summary>
    public class PostBarTopicDiscussBLL : BaseBLL<T_PostBarTopicDiscuss>, IPostBarTopicDiscussBLL
    {
        private const string _Type = "PostBarTopicDiscussDAL";

        private IPostBarTopicDiscussDAL _Dal;

        public PostBarTopicDiscussBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPostBarTopicDiscussDAL;
        }

        public bool DeleteLevelOneDiscuss(int replyId)
        {
            return this._Dal.DeleteLevelOneDiscuss(replyId);
        }
    }
}
