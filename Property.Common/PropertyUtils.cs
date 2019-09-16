using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using cn.jpush.api;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Property.Common
{
    public class PropertyUtils
    {
        private static readonly ILog log = LogManager.GetLogger("ServiceLog");

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

                //string toAddress = ConfigurationManager.AppSettings["ToAddress"].ToString();
                //PropertyUtils.SendEmail(toAddress, "刘涛", "缴费日志", sb.ToString());
            }
        }

        /// <summary>
        /// 获取配置参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigParamValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 将字符串进行MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5Str(string str)
        {
            StringBuilder sb = new StringBuilder();

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();


            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("X2"));
            }

            //md5Str就是最后得到加密后的字符串
            string md5Str = sb.ToString();

            return md5Str;
        }

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string EncodeBase64(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            try
            {
                source = Convert.ToBase64String(bytes);
            }
            catch
            {
            }
            return source;
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式 UTF-8</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }


        /// <summary>
        /// 解压压缩文件到指定目录
        /// </summary>
        /// <param name="zipFile">压缩文件</param>
        /// <param name="zipedDir">解压目录</param>
        /// <param name="zipedRelativePath">解压相对目录</param>
        /// <returns>解压的文件集的相对路径，多个用，分割</returns>
        public static string UnZip(string zipFile, string zipedDir, string zipedRelativePath)
        {
            //如果要解压的文件不存在
            if (!File.Exists(zipFile))
            {
                return null;
            }
            //创建解压目录
            if (!Directory.Exists(zipedDir))
            {
                Directory.CreateDirectory(zipedDir);
            }

            StringBuilder imgsSB = new StringBuilder();
            string imgsStr = null;
            ZipInputStream zipIS = null;
            ZipEntry zipEntry = null;
            FileStream streamWriter = null;
            try
            {
                string filePath;
                zipIS = new ZipInputStream(File.OpenRead(zipFile));
                while ((zipEntry = zipIS.GetNextEntry()) != null)
                {
                    if (zipEntry.Name != String.Empty)
                    {
                        filePath = Path.Combine(zipedDir, zipEntry.Name);
                        //如果解压的是文件夹
                        if (filePath.EndsWith("/") || filePath.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(filePath);
                            continue;
                        }
                        streamWriter = File.Create(filePath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = zipIS.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        imgsSB.Append(zipedRelativePath + "/" + zipEntry.Name + ";");
                    }
                }
                imgsStr = imgsSB.ToString();
                imgsStr = imgsStr.Substring(0, imgsStr.Length - 1);
            }
            finally
            {
                //删除.zip文件
                FileInfo f = new FileInfo(zipFile);
                if (f.Exists)
                    f.Delete();

                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
                if (zipEntry != null)
                {
                    zipEntry = null;
                }
                if (zipIS != null)
                {
                    zipIS.Close();
                    zipIS = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return imgsStr;
        }

        //// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /**/
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="quality">质量压缩比</param>
        /// <param name="multiple">收缩倍数</param>
        /// <param name="outputFile">输出文件名</param>
        /// <returns>成功返回true,失败则返回false</returns>
        public static bool getThumImage(string sourceFile, long quality, int multiple, string outputFile)
        {
            try
            {
                long imageQuality = quality;
                Bitmap sourceImage = new Bitmap(sourceFile);
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                float xWidth = sourceImage.Width;
                float yWidth = sourceImage.Height;
                Bitmap newImage = new Bitmap((int)(xWidth / multiple), (int)(yWidth / multiple));
                Graphics g = Graphics.FromImage(newImage);
                g.DrawImage(sourceImage, 0, 0, xWidth / multiple, yWidth / multiple);
                g.Dispose();
                newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);
                return true;
            }
            catch (Exception ex)
            {
                PropertyUtils.WriteLogError(ex);
                return false;
            }
        }

        ///// <summary>
        ///// 测试用
        ///// </summary>
        ///// <param name="sourceFile"></param>
        ///// <param name="quality"></param>
        ///// <param name="multiple"></param>
        ///// <param name="outputFile"></param>
        ///// <returns></returns>
        //public static bool getThumImageTest(string sourceFile, long quality, int multiple, string outputFile)
        //{
        //    try
        //    {
        //        string aaaaaaa = System.AppDomain.CurrentDomain.BaseDirectory + "YST.txt";
        //        StreamWriter sw22 = new StreamWriter(aaaaaaa, true);
        //        sw22.Write("sourceFile是：" + sourceFile);
        //        sw22.Write("outputFile是：" + outputFile);
        //        sw22.Close();
        //        long imageQuality = quality;
        //        Bitmap sourceImage = new Bitmap(sourceFile);
        //        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
        //        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
        //        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        //        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
        //        myEncoderParameters.Param[0] = myEncoderParameter;
        //        float xWidth = sourceImage.Width;
        //        float yWidth = sourceImage.Height;
        //        Bitmap newImage = new Bitmap((int)(xWidth / multiple), (int)(yWidth / multiple));
        //        Graphics g = Graphics.FromImage(newImage);
        //        g.DrawImage(sourceImage, 0, 0, xWidth / multiple, yWidth / multiple);
        //        g.Dispose();

        //        string aaaa = System.AppDomain.CurrentDomain.BaseDirectory + "YST.txt";
        //        StreamWriter sw2 = new StreamWriter(aaaa, true);
        //        sw2.Write("outputFile是：" + outputFile);
        //        sw2.Write("myImageCodecInfo是：" + myImageCodecInfo);
        //        sw2.Write("myEncoderParameters是：" + myImageCodecInfo);
        //        sw2.Close();

        //        newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);

        //        string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "YST.txt";
        //        StreamWriter sw = new StreamWriter(filePath, true);
        //        sw.Write("成功");
        //        sw.Close();
        //        PropertyUtils.SendEmail("liutao@sarnath.cn", "刘涛", "压缩图", "成功");
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        string exInfo = Environment.NewLine + "事件:" + ex.Message + Environment.NewLine + "操作时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "YST.txt";
        //        StreamWriter sw = new StreamWriter(filePath, true);
        //        sw.Write(exInfo);
        //        sw.Close();
        //        PropertyUtils.SendEmail("liutao@sarnath.cn", "刘涛", "压缩图", exInfo);
        //        return false;
        //    }
        //}

        /**/
        /// <summary>
        /// 获取图片编码信息
        /// </summary>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public static bool SendEmail(string To, string Name, string Subject, string Content)
        {
            //获取SMTP的服务器地址
            string smtpServer = ConfigurationManager.AppSettings["SmtpServer"].ToString();

            //构造SmtpClient指定SMTP服务器地址和端口号
            SmtpClient client = new SmtpClient(smtpServer, 25);

            //设置是否使用默认凭证，True：使用 Flase：不适用
            client.UseDefaultCredentials = false;
            //获取或设置用于验证发件人身份的凭据。
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["ServerMail"],
                ConfigurationManager.AppSettings["Password"]);
            try
            {
                client.Send(InitMailMessage(To, Name, Subject, Content));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化信件相关信息
        /// </summary>
        /// <param name="reEmailPath">发件人</param>
        /// <param name="reEmailName">发件人姓名</param>
        /// <param name="sendEmailTitle">发送标题</param>
        /// <param name="sendEmailContent">发送内容</param>
        /// <returns></returns>
        private static MailMessage InitMailMessage(string To, string name, string Subject, string sendEmailContent)
        {
            MailMessage mail = new MailMessage();
            //发件人
            mail.From = new MailAddress(ConfigurationManager.AppSettings["ServerMail"], "物业生活管理平台");

            //收件人
            if (name != "")
            {
                MailAddress mailAdd = new MailAddress(To, name);
                mail.To.Add(mailAdd);
            }
            else
            {
                mail.To.Add(To);
            }
            //主题
            mail.Subject = Subject;

            //内容
            mail.Body = sendEmailContent;

            //邮件主题和正文的编码格式
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            //邮件正文允许html编码
            mail.IsBodyHtml = true;
            //优先级
            mail.Priority = MailPriority.Normal;

            //密送——就是将信密秘抄送给收件人以外的人，所有收件人看不到密件抄送的地址
            //mail.Bcc.Add("");


            //抄送——就是将信抄送给收件人以外的人,所有的收件人可以在抄送地址处看到此信还抄送给谁
            //mail.CC.Add("");

            //mail.Attachments.Add(new Attachment("D:\\1.doc"));     //添加附件

            return mail;

        }

        /// <summary>
        /// 发送推送
        /// </summary>
        /// <param name="androidTitle">通知栏标题</param>
        /// <param name="alert">通知消息</param>
        /// <param name="AppType">APP类型 0：业主APP  1：物业APP</param>
        /// <param name="registrationIds">设备Id集合数组</param>
        /// <returns></returns>
        public static bool SendPush(string androidTitle, string alert, int AppType, params string[] registrationIds)
        {
            try
            {
                //通知信息
                var notification = new Notification().setAlert(alert);
                notification.AndroidNotification = new AndroidNotification().setTitle(androidTitle);

                if (registrationIds.Count() > 0)
                {
                    PushPayload pushPayload = new PushPayload();
                    //设置平台信息：全部
                    pushPayload.platform = Platform.all();
                    pushPayload.options.apns_production = true;
                    //推送目标：设备Id集
                    pushPayload.audience = Audience.s_registrationId(registrationIds);

                    notification.IosNotification = new IosNotification();
                    notification.IosNotification.incrBadge(1).setSound("sound");
                    notification.IosNotification.AddExtra("Title", androidTitle);
                    //推送消息
                    pushPayload.notification = notification.Check();

                    //根据客户端类型选择相应的KEY推送
                    if (AppType == ConstantParam.MOBILE_TYPE_OWNER)
                    {
                        JPushClient client = new JPushClient(ConstantParam.APP_KEY, ConstantParam.MASTER_SECRET);
                        client.SendPush(pushPayload);
                    }
                    else if (AppType == ConstantParam.MOBILE_TYPE_PROPERTY)
                    {
                        JPushClient client = new JPushClient(ConstantParam.PROPERTY_APP_KEY, ConstantParam.PROPERTY_MASTER_SECRET);
                        client.SendPush(pushPayload);
                    }
                    else if (AppType == ConstantParam.MOBILE_TYPE_SHOP)
                    {
                        JPushClient client = new JPushClient(ConstantParam.SHOP_APP_KEY, ConstantParam.SHOP_MASTER_SECRET);
                        client.SendPush(pushPayload);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="tos">接收号码，多个之间用,分割</param>
        /// <param name="msg">短信内容（1-70）</param>
        /// <param name="otime">定时发送时间，为null时即时发送</param>
        /// <returns></returns>
        public static bool SendSMS(string tos, string msg, string otime)
        {
            string uid = ConfigurationManager.AppSettings["SmsUid"];
            string pwd = ConfigurationManager.AppSettings["SmsPwd"];
            try
            {
                //WebService接口实现
                Service1.Service1 service = new Service1.Service1();
                string result = service.SendMessages(uid, pwd, tos, msg, otime);

                //HTTP接口实现
                //string url = "http://service.winic.org/sys_port/gateway/?id=" + uid + "&pwd=" + pwd + "&to=" + tos + "&content=" + msg + "&time="+otime;
                //MSXML2.XMLHTTP xmlhttp = new MSXML2.XMLHTTP();

                //xmlhttp.open("GET", url, false, null, null);
                //xmlhttp.send("");
                //XMLDocument dom = new XMLDocument();
                //Byte[] b = (Byte[])xmlhttp.responseBody;
                //string result = System.Text.Encoding.GetEncoding("GB2312").GetString(b).Trim(); 

                if (result.Contains("-"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将对象转化成json字符串
        /// </summary>
        /// <param name="obj">对象内容</param>
        /// <returns></returns>
        public static string ModelToJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将实体中的所有支持公有设置的double类型字段都保留指定位数的小数
        /// </summary>
        /// <param name="model">实体</param>
        public static void Round(object model)
        {
            var type = model.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var p in props)
            {
                var method = p.GetSetMethod();
                if ((p.PropertyType == typeof(double) || p.PropertyType == typeof(double?)) && p.CanWrite && method != null)
                {
                    var v = p.GetValue(model, null);
                    if (v is double)
                    {
                        double val = (double)p.GetValue(model, null);
                        val = Math.Round(val, ConstantParam.DECIMAL_PLACE);
                        p.SetValue(model, val, null);
                    }
                    else if (v is double?)
                    {
                        double? val = (double?)p.GetValue(model, null);
                        if (val.HasValue)
                        {
                            val = Math.Round(val.Value, ConstantParam.DECIMAL_PLACE);
                            p.SetValue(model, val, null);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 将实体中的所有字段都保留指定位数的小数
        /// </summary>
        /// <param name="model">实体</param>
        public static void Round(double?[] model)
        {
            if (model != null)
            {
                for (int i = 0; i < model.Length; i++)
                {
                    if (model[i].HasValue)
                    {
                        model[i] = Math.Round(model[i].Value, ConstantParam.DECIMAL_PLACE);
                    }
                }
            }
        }

        /// <summary>
        /// 将NULL值或者小于0的值设置为0
        /// </summary>
        /// <param name="model"></param>
        public static void CleanNull(object model)
        {
            if (model == null)
                return;
            var type = model.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var p in props)
            {
                var method = p.GetSetMethod();
                if (method != null && method.IsPublic)
                {
                    if (p.PropertyType == typeof(double?))
                    {
                        var v = p.GetValue(model, null) as double?;
                        if (!v.HasValue || v < 0)
                        {
                            p.SetValue(model, 0.0, null);
                        }
                    }
                    if (p.PropertyType == typeof(DateTime?))
                    {
                        var v = p.GetValue(model, null) as DateTime?;
                        if (!v.HasValue)
                        {
                            p.SetValue(model, DateTime.MinValue, null);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s = "";
            StreamReader sr = new StreamReader(Path);
            s = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return s;
        }
    }
}
