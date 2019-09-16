using Property.Common;
using Property.DAL;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.ExpenseNotificationServer
{
    public class ExpenseNotificationManage
    {
        public static void ExpenseNotification()
        {
            try
            {
                var allList = new List<T_HouseUserExpenseTemplate>();

                //当前日期
                DateTime CurrentDate = DateTime.Now.Date;
                //明天日期
                DateTime TommorrowDate = CurrentDate.AddDays(1).Date;

                //所有符合发推送的缴费信息列表
                IHouseUserExpenseTemplateBLL templateBLL = BLLFactory<IHouseUserExpenseTemplateBLL>.GetBLL("HouseUserExpenseTemplateBLL");
                var expenseTemplateList = templateBLL.GetList().ToList();

                foreach (var expenseTemplate in expenseTemplateList)
                {
                    //看看要处理是不是今天，如果不是今天按照推送的周期加周期的天数（每月加30，每两月加60，每季度90...)
                    var isCheck = CurrentDate <= expenseTemplate.NotificationDate && expenseTemplate.NotificationDate < TommorrowDate;

                    if (isCheck)
                    {
                        allList.Add(expenseTemplate);
                    }
                    else
                    {
                        expenseTemplate.NotificationDate = expenseTemplate.NotificationDate.AddDays(expenseTemplate.ExpenseCycleId * 30);
                        isCheck = CurrentDate <= expenseTemplate.NotificationDate && expenseTemplate.NotificationDate < TommorrowDate;

                        if (isCheck)
                        {
                            allList.Add(expenseTemplate);
                        }
                    }
                }

                //保存到ExpenseDetail表
                IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
                //缴费周期 应缴费开始时间加上缴费周期天数就是应缴费结束时间
                int expensePeriod = int.Parse(ConfigurationManager.AppSettings["ExpensePeriod"].ToString());
                var detailsList = expenseDetailsBLL.SaveExpenseDetails(allList, expensePeriod);

                var registrationIdList = new List<string>();
                var title = "缴费提醒";
                var content = "您有一条缴费提醒, 请注意查收。";

                //将保存成功的ExpenseDetail队列发推送
                if (detailsList.Count > 0)
                {
                    CommonUtils.WriteLogInfo("已经进入到发推送里面");
                    IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    //全部保存成功后，取得所有要推送的用户
                    foreach (var item in detailsList)
                    {
                        var appUserIds = item.BuildDoor.PropertyIdentityVerifications.Select(p => p.AppUserId);
                        var registrationIds = userPushBLL.GetList(u => appUserIds.Contains(u.UserId)).Select(u => u.RegistrationId);
                        registrationIdList.AddRange(registrationIds);
                    }

                    foreach (var item in registrationIdList)
                    {
                        CommonUtils.WriteLogInfo(string.Format("发推送人的设备是：{0}", item));
                    }

                    //批量推送出去
                    bool check = Property.Common.PropertyUtils.SendPush(title, content, ConstantParam.MOBILE_TYPE_OWNER, registrationIdList.ToArray());
                    CommonUtils.WriteLogInfo(string.Format("调用完推送返回的结果：{0}", check));
                    CommonUtils.WriteLogInfo("已经完成发推送。");
                }
            }
            catch (Exception ex)
            {
                CommonUtils.WriteLogError(ex);
            }
        }
    }
}
