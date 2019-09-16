using log4net;
using Property.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Property.ExpenseNotificationServer
{
    public class CommonUtils
    {
        private static readonly ILog log = LogManager.GetLogger("ServiceLog");
        //public static log4net.ILog logInfo = log4net.LogManager.GetLogger("LogInfo");
        //public static log4net.ILog logError = log4net.LogManager.GetLogger("LogError");

        /// <summary>
        /// 写跟踪信息(Info级)
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLogInfo(string message)
        {
            if (log.IsInfoEnabled)
            {
                log.Info("信息:" + message);
            }
        }

        /// <summary>
        /// 写错误信息(Error级)
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteLogError(Exception ex)
        {
            if (log.IsErrorEnabled)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(string.Format("当前时间:{0}{1}", DateTime.Now.ToString(), Environment.NewLine));
                sb.Append(string.Format("异常信息:{0}{1}", ex.Message, Environment.NewLine));
                sb.Append(string.Format("异常对象:{0}{1}", ex.Source, Environment.NewLine));
                sb.Append(string.Format("触发方法:{0}{1}", ex.TargetSite, Environment.NewLine));
                log.Error("错误信息：" + sb.ToString());

                string toAddress = ConfigurationManager.AppSettings["ToAddress"].ToString();
                PropertyUtils.SendEmail(toAddress, "杨璐", "缴费日志", sb.ToString());
            }
        }

        /// <summary>
        /// 普通的方式记录日志
        /// </summary>
        /// <param name="txt"></param>
        public static void WriteLog(string txt)
        {
            string exInfo = Environment.NewLine + "事件:" + txt + Environment.NewLine + "操作时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "ShopRejectOrderLog.txt";
                StreamWriter sw = new StreamWriter(filePath, true);
                sw.Write(exInfo);
                sw.Close();
            }
            catch (Exception ex)
            {
                exInfo = ex.Message;
            }
            finally
            {
                string toAddress = ConfigurationManager.AppSettings["ToAddress"].ToString();
                PropertyUtils.SendEmail(toAddress, "刘涛", "缴费日志", exInfo);
            }
        }
    }
}
