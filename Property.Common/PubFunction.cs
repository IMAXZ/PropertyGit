using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;

namespace Property.Common
{
    public class PubFunction
    {
        #region ========错误日志输出========
        /// <summary>
        /// 错误日志书写
        /// </summary>fx 2014.08.21
        /// <param name="sqlStr">发生错误的sql语句或者标记</param>
        /// <param name="errorMessage">错误信息</param>
        public static void ErrorLogPrint(string sqlStr, string errorMessage)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\tencent\SysLog";//防止系统盘不可访问
            StringBuilder result = new StringBuilder("-----------------------------");
            result.Append(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"));
            result.Append("---------------------------\r\n");
            result.Append("发生错误的语句：");
            result.Append(sqlStr);
            result.Append("\r\n错误信息:");
            result.Append(errorMessage);
            if (!Directory.Exists(filePath))//文件所在路径不存在时 创建路径
            {
                Directory.CreateDirectory(filePath);
            }
            FileInfo fi = new FileInfo(filePath + @"\ErrorLog.txt");
            if (!fi.Exists)//文件不存在时 创建文件
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
            else
            {
                if (fi.Length >= 6000)//文件内容的长度大于一定范围时，清空
                {
                    fi.Delete();
                }
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
        }
   
        #endregion

        #region ========信息打印========
        /// <summary>
        /// 信息打印
        /// </summary>fx 2016.05.25
        /// <param name="sqlStr">发生错误的sql语句或者标记</param>
        /// <param name="errorMessage">错误信息</param>
        public static void InfoLogPrint(string sqlStr, string errorMessage)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\tencent\SysLog";//防止系统盘不可访问
            StringBuilder result = new StringBuilder("-----------------------------");
            result.Append(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"));
            result.Append("---------------------------\r\n");
            result.Append("语句：");
            result.Append(sqlStr);
            result.Append("\r\n信息:");
            result.Append(errorMessage);
            if (!Directory.Exists(filePath))//文件所在路径不存在时 创建路径
            {
                Directory.CreateDirectory(filePath);
            }
            FileInfo fi = new FileInfo(filePath + @"\ErrorLog.txt");
            if (!fi.Exists)//文件不存在时 创建文件
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
            else
            {
                if (fi.Length >= 6000)//文件内容的长度大于一定范围时，清空
                {
                    fi.Delete();
                }
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
        }

        #endregion

        #region ========错误日志输出========
        /// <summary>
        /// 错误日志书写
        /// </summary>fx 2014.08.21
        /// <param name="sqlStr">发生错误的sql语句或者标记</param>
        /// <param name="errorMessage">错误信息</param>
        public static void ErrorOaPrint(string sqlStr, string errorMessage)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\SysLog";//防止系统盘不可访问
            StringBuilder result = new StringBuilder("-----------------------------");
            result.Append(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"));
            result.Append("---------------------------\r\n");
            result.Append("发生错误的语句：");
            result.Append(sqlStr);
            result.Append("\r\n错误信息:");
            result.Append(errorMessage);
            if (!Directory.Exists(filePath))//文件所在路径不存在时 创建路径
            {
                Directory.CreateDirectory(filePath);
            }
            FileInfo fi = new FileInfo(filePath + @"\ErrorLog.log");
            if (!fi.Exists)//文件不存在时 创建文件
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
            else
            {
                if (fi.Length >= 600000)//文件内容的长度大于一定范围时，清空
                {
                    fi.Delete();
                }
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(result.ToString());
                }
            }
        }

        #endregion
    }
}