using Property.Entity;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace Property.DAL
{
    /// <summary>
    /// 沟通列表数据层访问类
    /// </summary>
    public class PostBarTopicDAL : BaseDAL<T_PostBarTopic>, IPostBarTopicDAL
    {
        public IEnumerable<T_PostBarTopic> GetSetTopPageList(Expression<Func<T_PostBarTopic, bool>> where, int pageIndex, int pageSize)
        {
            return this.nContext.T_PostBarTopic.Where(where).OrderByDescending(t => t.IsTop).ThenByDescending(t => t.PostDate).ToPagedList(pageIndex, pageSize);
        }
        public bool DeleteTopic(int topicId)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除主题
                    string strSQL = string.Format("DELETE FROM T_PostBarTopic WHERE Id={0}", topicId);
                    this.nContext.Database.ExecuteSqlCommand(strSQL);

                    //删除收藏关系
                    string strSQL1 = string.Format("DELETE FROM R_UserPostBarTopic WHERE PostBarTopicId={0}", topicId);
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
