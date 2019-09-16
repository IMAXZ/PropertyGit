using Property.Common;
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
    /// 业主数据层访问类
    /// </summary>
    public class UserDAL : BaseDAL<T_User>, IUserDAL
    {

        public bool DeleteUser(T_User user)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该业主对应的小区关联记录
                    this.nContext.Database.ExecuteSqlCommand("delete from R_UserPlace where UserId=" + user.Id);

                    user.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                    //更新
                    base.Update(user);
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
