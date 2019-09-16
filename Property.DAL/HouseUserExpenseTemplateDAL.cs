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
    /// 物业小区业主缴费模板数据访问层
    /// </summary>
    public class HouseUserExpenseTemplateDAL : BaseDAL<T_HouseUserExpenseTemplate>, IHouseUserExpenseTemplateDAL
    {
        public bool UpdateExpenseTemplate(string doorIds, List<T_HouseUserExpenseTemplate> houseUserExpenseTemplatelist, int expenseTypeId)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该物业总公司用户所有对应角色关联
                    this.nContext.Database.ExecuteSqlCommand(string.Format("delete from T_HouseUserExpenseTemplate where BuildDoorId in ({0}) and ExpenseTypeId={1}", doorIds, expenseTypeId));
                    //重新分配权限
                    foreach (var houseUserExpenseTemplate in houseUserExpenseTemplatelist)
                    {
                        base.Save(houseUserExpenseTemplate);
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
