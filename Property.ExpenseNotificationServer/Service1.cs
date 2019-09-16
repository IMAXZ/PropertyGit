using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Property.ExpenseNotificationServer
{
    public partial class Service1 : ServiceBase
    {
        protected bool RunningStatus = false;
        public Thread ExpenseNotificationThread { get; set; }

        public Service1()
        {
            ExpenseNotificationThread = new Thread(ChkSrv);
            ExpenseNotificationThread.Name = "业主缴费推送";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                EventLog.WriteEntry("我的物业缴费服务启动");
                CommonUtils.WriteLogInfo("物业缴费服务启动");
                RunningStatus = true;
                ExpenseNotificationThread.Start();
            }
            catch (Exception ex)
            {
                CommonUtils.WriteLogError(ex);
            }
        }

        protected override void OnStop()
        {
            RunningStatus = false;

            try
            {
                ExpenseNotificationThread.Abort();
                EventLog.WriteEntry("我的物业缴费服务停止");
                CommonUtils.WriteLogInfo("我的物业缴费服务停止");

            }
            catch (Exception ex)
            {
                CommonUtils.WriteLogError(ex);
            }

        }

        public void ChkSrv()
        {
            while (RunningStatus)
            {
                try
                {
                    //获取哪个时刻进行处理
                    int pushHour = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PushHour"].ToString());

                    if (DateTime.Now.Hour == pushHour)
                    {
                        if (RunningStatus)
                        {
                            //执行处理往缴费明细表插记录
                            ExpenseNotificationManage.ExpenseNotification();
                            CommonUtils.WriteLogInfo("成功插入费用详细表并发推送。");
                            //睡眠1小时
                            Thread.Sleep(1000 * 60 * 60);
                        }
                    }
                    else
                    {
                        //睡眠1分钟
                        Thread.Sleep(1000 * 60 * 1);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtils.WriteLogError(ex);
                }

            }

        }

    }
}
