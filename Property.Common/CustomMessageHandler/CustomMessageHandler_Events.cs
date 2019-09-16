using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;

namespace Property.Common.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            //菜单点击，需要跟创建菜单时的Key匹配
            //switch (requestMessage.EventKey)
            //{
            //    case "V1001_HUNYU":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "个人业务-婚育",
            //                Description = "婚育包括结婚、离婚、准生、领养等",
            //                Url = "http://www.banshir.cn/Weixin/PersonalBusiness?CategoryID=4",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/huyu.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1001_JIAOYUJIUYE":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "个人业务-教育就业",
            //                Description = "教育就业包括从业资格考试、资格认定等",
            //                Url = "http://www.banshir.cn/Weixin/PersonalBusiness?CategoryID=1",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/jiaoyujiuye.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1001_SHEHUIBAOZHANG":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "个人业务-社会保险",
            //                Description = "社保包括社会保险、缴费记录查询等",
            //                Url = "http://www.banshir.cn/Weixin/PersonalBusiness?CategoryID=2",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/shebao.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1001_ZHENGJIANBANLI":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "个人业务-证件办理",
            //                Description = " 证件办理包括身份证、户口本、护照等",
            //                Url = "http://www.banshir.cn/Weixin/PersonalBusiness?CategoryID=5",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/zhengjianbanli.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1001_FANGWUJIAOTONG":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "个人业务-房屋交通",
            //                Description = "房屋交通包括公积金、住房、驾驶证等",
            //                Url = "http://www.banshir.cn/Weixin/PersonalBusiness?CategoryID=3",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/jiaotong.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1002_QIYEKAIBAN":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "企业业务-企业开办",
            //                Description = "企业开办包括企业登记、申请、许可证办理等",
            //                Url = "http://www.banshir.cn/Weixin/EnterpriseBusiness?CategoryID=9",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/qiyekaiban.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1002_RENLIZIYUAN":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "企业业务-人力资源",
            //                Description = "人力资源劳动用工参保、就业登记、年审等",
            //                Url = "http://www.banshir.cn/Weixin/EnterpriseBusiness?CategoryID=11",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/renliziyuan.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1002_XIANGMUSHENBAO":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "企业业务-项目申报",
            //                Description = "项目申报和审批包括项目申报、审批、登记备案等",
            //                Url = "http://www.banshir.cn/Weixin/EnterpriseBusiness?CategoryID=12",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/shenbao.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1002_JINGYINGNASHUI":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "企业业务-经营纳税",
            //                Description = "经营纳税包括申报税、纳税人、发票等",
            //                Url = "http://www.banshir.cn/Weixin/EnterpriseBusiness?CategoryID=10",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/shuishou.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1002_ZIZHIRENDING":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "企业业务-资质认定",
            //                Description = " 资质认定包括企业认定、许可证、专利等",
            //                Url = "http://www.banshir.cn/Weixin/EnterpriseBusiness?CategoryID=13",
            //                PicUrl = "http://www.banshir.cn/Images/Weixin/zizhirenzheng.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1003_GUANYUWOMEN":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "办事儿-关于我们",
            //                Description = "办事儿，是青岛萨纳斯科技有限公司为广大用户量身定制的一款生活服务类软件.....",
            //                Url = "http://www.banshir.cn/about.html",
            //                PicUrl = "http://www.banshir.cn/image/logo.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1003_JIANSUO":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            //            strongResponseMessage.Articles.Add(new Article()
            //            {
            //                Title = "办事儿-检索",
            //                Description = "个人业务，企业业务尽情搜索,让他做您贴身的生活顾问",
            //                Url = "http://www.banshir.cn/weixin/banshirsearch",
            //                PicUrl = "http://www.banshir.cn/image/banshir.png"
            //            });
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //    case "V1003_DOWNLOAD":
            //        {
            //            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
            //            strongResponseMessage.Content = "欢迎关注【办事儿】，让他做您贴身的私人生活顾问。\r\n点击链接下载：\r\n <a href='http://www.banshir.cn/index.html'>办事下载</a>";
            //            reponseMessage = strongResponseMessage;
            //        }
            //        break;
            //}
            return reponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描关注
            return base.OnEvent_ScanRequest(requestMessage);
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            responseMessage.Articles.Add(new Article()
            {
                Title = "欢迎关注【Ai我家】",
                Description = "欢迎关注【Ai我家】，让他为您提供全方位的物业家庭服务。",
                Url = "http://v.homekeeper.com.cn/WeixinProduct/Index/",
                PicUrl = "http://v.homekeeper.com.cn/Images/Weixin/welcome.png"
            });
            responseMessage.Articles.Add(new Article()
            {
                Title = "绑定小区获得更多功能",
                Description = "通过绑定小区可以获得更多功能。点击链接进行小区绑定",
                Url = "http://v.homekeeper.com.cn/WeixinIdentityBind/Index/",
                PicUrl = "http://v.homekeeper.com.cn/Images/Weixin/bangdin_place.png"
            });
            responseMessage.Articles.Add(new Article()
            {
                Title = "分享二维码给好友邻居，关注【Ai我家】微信公众号",
                Description = "分享二维码给好友邻居，关注【Ai我家】微信公众号。",
                Url = "http://v.homekeeper.com.cn/WeixinProduct/QrCodeShare/",
                PicUrl = "http://v.homekeeper.com.cn/Images/Weixin/share_qr.png"
            });
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "";
            return responseMessage;
        }
    }
}