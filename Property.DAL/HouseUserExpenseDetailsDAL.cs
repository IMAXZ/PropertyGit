using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
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
    public class HouseUserExpenseDetailsDAL : BaseDAL<T_HouseUserExpenseDetails>, IHouseUserExpenseDetailsDAL
    {
        public List<T_HouseUserExpenseDetails> SaveExpenseDetails(List<T_HouseUserExpenseTemplate> expenseTemplateList, int expensePeriod)
        {
            //当前日期
            DateTime CurrentDate = DateTime.Now.Date;
            //明天日期
            DateTime TommorrowDate = CurrentDate.AddDays(1).Date;

            var expenseDetailsList = new List<T_HouseUserExpenseDetails>();

            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //往要发送的推送保存到物业小区业主缴费明细表
                    foreach (var expenseInfo in expenseTemplateList)
                    {
                        var expenseDetail = new T_HouseUserExpenseDetails();

                        string ExpenseDateDes = string.Empty;

                        //看看要处理是不是今天，如果不是今天按照推送的周期加周期的天数（每月加30，每两月加60，每季度90...)
                        var checkIsToday = CurrentDate <= expenseInfo.NotificationDate && expenseInfo.NotificationDate < TommorrowDate;

                        if (!checkIsToday)
                        {
                            expenseInfo.NotificationDate = expenseInfo.NotificationDate.AddDays(expenseInfo.ExpenseCycleId * 30);
                        }

                        switch (expenseInfo.ExpenseCycleId)
                        {
                            case 1:
                                ExpenseDateDes = string.Format("{0}月{1}费用", expenseInfo.NotificationDate.Month, expenseInfo.PropertyExpenseType.Name);
                                break;
                            case 2:
                                ExpenseDateDes = string.Format("从{0}月起两个月的{1}费用", expenseInfo.NotificationDate.Month, expenseInfo.PropertyExpenseType.Name);
                                break;
                            case 3:
                                ExpenseDateDes = string.Format("从{0}月起三个月的{1}费用", expenseInfo.NotificationDate.Month, expenseInfo.PropertyExpenseType.Name);
                                break;
                            case 4:
                                ExpenseDateDes = string.Format("从{0}月起半年的{1}费用", expenseInfo.NotificationDate.Month, expenseInfo.PropertyExpenseType.Name);
                                break;
                            case 5:
                                ExpenseDateDes = string.Format("从{0}月起一年的{1}费用", expenseInfo.NotificationDate.Month, expenseInfo.PropertyExpenseType.Name);
                                break;
                        }

                        expenseDetail.ExpenseDateDes = ExpenseDateDes;
                        expenseDetail.ExpenseTypeId = expenseInfo.ExpenseTypeId;
                        expenseDetail.Expense = expenseInfo.Expense;
                        expenseDetail.ExpenseCycleId = expenseInfo.ExpenseCycleId;
                        expenseDetail.CreateDate = DateTime.Now;
                        expenseDetail.ExpenseBeginDate = expenseInfo.NotificationDate;
                        expenseDetail.ExpenseEndDate = expenseInfo.NotificationDate.AddDays(expensePeriod);
                        expenseDetail.BuildDoorId = expenseInfo.BuildDoorId;
                        expenseDetail.IsPayed = 0;
                        expenseDetail.InvoiceType = 0;
                        expenseDetail.PropertyExpenseType = expenseInfo.PropertyExpenseType;
                        expenseDetail.BuildDoor = expenseInfo.BuildDoor;
                        this.nContext.T_HouseUserExpenseDetails.Add(expenseDetail);
                        this.nContext.SaveChanges();
                        expenseDetailsList.Add(expenseDetail);
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }

            return expenseDetailsList;
        }
    }
}
