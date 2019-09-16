using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Property.ShopRejectOrderServer
{
    public partial class Service1 : ServiceBase
    {
        protected bool RunningStatus = false;
        public Thread RejectOrderThread { get; set; }
        public Service1()
        {
            RejectOrderThread = new Thread(ChkSrv);
            RejectOrderThread.Name = "商家自动退单服务";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                EventLog.WriteEntry("商家自动退单服务启动");
                CommonUtils.WriteLogInfo("商家自动退单服务启动");
                RunningStatus = true;
                RejectOrderThread.Start();
                //CommonUtils.WriteLog("这说明服务启动开始");
            }
            catch (Exception ex)
            {
                CommonUtils.WriteLog(ex.Message);
                CommonUtils.WriteLogError(ex);
            }
        }

        protected override void OnStop()
        {
            RunningStatus = false;

            try
            {
                RejectOrderThread.Abort();
                EventLog.WriteEntry("商家自动退单服务停止");
                CommonUtils.WriteLogInfo("商家自动退单服务停止");
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
                    bool check = RejectOrderManage.RejectOrders();

                    if (check)
                    {
                        CommonUtils.WriteLogInfo("退单操作完毕。");
                        Thread.Sleep(1000 * 60 * 2);
                    }
                    else
                    {
                        RunningStatus = false;
                    }

                }
                catch (Exception ex)
                {
                    RunningStatus = false;
                    CommonUtils.WriteLogError(ex);
                }
            }
        }
    }
}
