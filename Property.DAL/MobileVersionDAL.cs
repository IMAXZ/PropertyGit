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
    /// APP版本管理数据层访问类
    /// </summary>
    public class MobileVersionDAL : BaseDAL<T_MobileVersion>, IMobileVersionDAL
    {

        //// <summary>
        /// 删除指定版本
        /// </summary>
        /// <param name="id">版本id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteVersion(int Id)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该指定版本
                    this.nContext.Database.ExecuteSqlCommand("delete from T_MobileVersion where Id=" + Id);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return true;
        }
    }
}
