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
    /// 业主圈子数据层访问类
    /// </summary>
    public class SocialCircleDAL : BaseDAL<T_SocialCircle>, ISocialCircleDAL
    {
        /// <summary>
        /// 解散圈子
        /// </summary>
        /// <param name="sc">要解散的圈子实体</param>
        /// <returns></returns>
        public bool Dissolve(T_SocialCircle sc) 
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除圈子成员关联关系
                    this.nContext.Database.ExecuteSqlCommand("delete from R_UserSocialCircle where SocialCircleId=" + sc.Id);
                    //删除圈子聊天记录
                    this.nContext.Database.ExecuteSqlCommand("delete from T_SocialCircleChat where SocialCircleId=" + sc.Id);
                    //删除圈子
                    this.nContext.Database.ExecuteSqlCommand("delete from T_SocialCircle where Id=" + sc.Id);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
                return true;
            }
        }
    }
}
