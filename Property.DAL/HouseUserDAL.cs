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
    /// 住宅业主信息数据层访问类
    /// </summary>
    public class HouseUserDAL:BaseDAL<T_HouseUser>,IHouseUserDAL
    {
        public bool ImportHouseUsers(List<T_HouseUser> houseUserList)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var houseUser in houseUserList)
                    {
                        base.Save(houseUser);
                    }

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
