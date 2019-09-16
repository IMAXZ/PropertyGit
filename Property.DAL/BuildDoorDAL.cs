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
    /// 单元户数据层访问类
    /// </summary>
    public class BuildDoorDAL : BaseDAL<T_BuildDoor>, IBuildDoorDAL
    {
        /// <summary>
        /// 批量添加单元户
        /// </summary>
        /// <param name="unitId">单元id</param>
        /// <param name="list">单元户对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool BatchAddDoor(int unitId, List<T_BuildDoor> list)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //批量添加单元
                    foreach (var item in list)
                    {
                        //添加
                        base.Save(item);
                    }
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
