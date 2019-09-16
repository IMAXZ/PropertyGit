using Property.Entity;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 主题讨论表 数据层访问类
    /// </summary>
    public class PostBarTopicDiscussDAL : BaseDAL<T_PostBarTopicDiscuss>, IPostBarTopicDiscussDAL
    {
        public bool DeleteLevelOneDiscuss(int replyId)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除二级回复
                    string strSQL = string.Format("DELETE FROM T_PostBarTopicDiscuss WHERE ParentId={0}", replyId);
                    this.nContext.Database.ExecuteSqlCommand(strSQL);

                    //删除一级回复
                    string strSQL1 = string.Format("DELETE FROM T_PostBarTopicDiscuss WHERE Id={0}", replyId);
                    this.nContext.Database.ExecuteSqlCommand(strSQL1);

                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
