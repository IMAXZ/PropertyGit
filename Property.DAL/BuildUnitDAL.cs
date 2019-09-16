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
    /// 楼座单元数据层访问类
    /// </summary>
    public class BuildUnitDAL : BaseDAL<T_BuildUnit>, IBuildUnitDAL
    {
        /// <summary>
        /// 批量添加单元
        /// </summary>
        /// <param name="buildId">楼座id</param>
        /// <param name="list">单元对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool BatchAddUnit(int buildId, List<T_BuildUnit> list)
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
