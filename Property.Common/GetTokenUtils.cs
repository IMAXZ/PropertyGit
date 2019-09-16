using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Property.Common
{
    public class GetTokenUtils
    {
        public static readonly string Token = "aiwojia_weixin_server";//与微信企业账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "Aiwojia1weixin2server3key4aiwojia5weixinkey";//与微信企业账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string CorpId = "wx31d637be898356a5";//与微信企业账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string appsecret = "fb16ce97250630736d44fda4bee52c48";
        
        /// <summary>
        /// 获取每次操作微信API的Token访问令牌
        /// </summary>
        /// <param name="corpid">企业Id</param>
        /// <param name="corpsecret">管理组的凭证密钥</param>
        /// <returns></returns>
        public static string GetAccessTokenNoCache()
        {
            //string returndate = HttpClient.HttpGet(@"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + CorpId + "&corpsecret=" + appsecret + "", "");
            string returndate = HttpClient.HttpGet(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + CorpId + "&secret=" + appsecret,"");

            string stoken = returndate.Split(',')[0].Split(':')[1].ToString();
            stoken = stoken.Replace("\"", "");
            string result = stoken + "&" + DateTime.Now; //XDJHKSHHFJFFHFH: TIME        
            string configFile = GetConfigFilePath("access_token.txt");//避免重复请求令牌，应进行全局静态存储
            File.WriteAllText(configFile, result);
            return result;

        }

        public static String GetJsApiTicketNoCache() {
            string token = GetTokenUtils.GetToken();
            //string strUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + token);
            string strUrl = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi");
        
            string returndate = HttpClient.HttpGet(strUrl, "");
            string stoken = returndate.Split(',')[2].Split(':')[1].ToString();
            string jstoken = stoken.Replace("\"", "");
            string result = jstoken + "&" + DateTime.Now; //XDJHKSHHFJFFHFH: TIME        
            string configFile = GetConfigFilePath("access_jsapi_token.txt");//避免重复请求令牌，应进行全局静态存储
            File.WriteAllText(configFile, result);
            return result;
        }
        /// <summary>
        /// 获取存储的全局Token访问令牌
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            string[] res;
            string stoken="";
            try
            {
                //string confilgFile = GetConfigFilePath("access_token.txt");
                //string result = File.ReadAllText(confilgFile);
                //res = result.Split('&');
                //TimeSpan time = (DateTime.Now - DateTime.Parse(res[1]));
                //if (time.Hours >= 2 || time.Days > 0)//正常情况下AccessToken有效期为7200秒，有效期内重复获取返回相同结果，并自动续期。
                //{
                //    string strs = GetAccessTokenNoCache();
                //    return strs.Substring(0, strs.LastIndexOf('&'));

                //}
                //else
                //    return res[0];
                string returndate = HttpClient.HttpGet(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + CorpId + "&secret=" + appsecret, "");

                stoken = returndate.Split(',')[0].Split(':')[1].ToString();
                stoken = stoken.Replace("\"", "");
                
            }
            catch (Exception)
            {
                string strs = GetAccessTokenNoCache();
                return strs.Substring(0, strs.LastIndexOf('&'));
            }
            return stoken;
           
        }
        public static string GetJsApiTicket() {
            string[] res;
            string jstoken = "";
            try
            {
                //string confilgFile = GetConfigFilePath("access_jsapi_token.txt");
                //string result = File.ReadAllText(confilgFile);
                //res = result.Split('&');
                //TimeSpan time = (DateTime.Now - DateTime.Parse(res[1]));
                //if (time.Hours >= 2 || time.Days > 0)//正常情况下AccessToken有效期为7200秒，有效期内重复获取返回相同结果，并自动续期。
                //{
                //    string strs = GetJsApiTicketNoCache();
                //    return strs.Substring(0, strs.LastIndexOf('&'));

                //}
                //else
                //    return res[0];

                string token = GetTokenUtils.GetToken();
                string strUrl = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi");

                string returndate = HttpClient.HttpGet(strUrl, "");
                string stoken = returndate.Split(',')[2].Split(':')[1].ToString();
                jstoken = stoken.Replace("\"", "");
                
            }
            catch (Exception)
            {
                string strs = GetJsApiTicketNoCache();
                return strs.Substring(0, strs.LastIndexOf('&'));
            }
            return jstoken;
        }
        private static string GetConfigFilePath(string filename)
        {
            string currenctDir = AppDomain.CurrentDomain.BaseDirectory + @"\tencent\SysLog";
            string configFile = System.IO.Path.Combine(currenctDir, filename);//当前文件夹下的connstr.txt文件
            return configFile;
        }


        /// <summary>
        /// 用户无意义操作
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="fromuser"></param>
        /// <param name="createtime"></param>
        /// <returns></returns>
        public static string NothingDo(string touser, string fromuser, string createtime)
        {
            string content = "呃…不大明白，或者您的问题真的难倒我了！您也可以输入序号快速办理业务：\r\n[1]通信录查询\r\n[2]员工信息查询\r\n[3]我的基本信息\r\n...\r\n还无法解决您的问题吗？那就联系管理员吧！";
            string str = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName> 
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[text]]></MsgType>
                                        <Content><![CDATA[{3}]]></Content>
                                      </xml>", touser, fromuser, createtime, content);
            return str;

        }

        public static string reviewSame(string content, string touser, string fromuser, string createtime)
        {
            if (content.ToUpper().Equals("JS"))
                content = "调用萨纳斯微信Demo！\r\n<a  href='http://weixin.sarnath.cn/tencent/JSSDK/SDKDemo'>点击进入DEMO</a>";
            else
                content = "内容为："+content;
            string str = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName> 
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[text]]></MsgType>
                                        <Content><![CDATA[{3}]]></Content>
                                      </xml>", touser, fromuser, createtime, content);
            return str;
        }
    }
}