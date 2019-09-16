using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.Entities;
using Property.Common.CustomMessageHandler;


namespace Property.UI.Controllers
{
    public class WeixinController : Controller
    {

        #region 微信token
        public readonly static string Token = "aiwojia_weixin_server";
        public readonly static string AppId = "wx31d637be898356a5";
        public readonly static string EncodingAESKey = "Aiwojia1weixin2server3key4aiwojia5weixinkey";
        public readonly static string AppSecret = "fb16ce97250630736d44fda4bee52c48";
        #endregion



        /// <summary>
        /// 用户处理微信后台与服务器后台的校验
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr);//返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 处理微信后台发过来的用户post请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            ////首先进行校验
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;
            postModel.AppId = AppId;

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);

            //执行微信处理过程
            messageHandler.Execute();

            //返回结果
            return new WeixinResult(messageHandler);
        }


        public ActionResult GetToken(string appId, string appSecret)
        {
            try
            {
                //if (!AccessTokenContainer.CheckRegistered(appId))
                //{
                //    AccessTokenContainer.Register(appId, appSecret);
                //}
                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(appId, appSecret);//AccessTokenContainer.GetTokenResult(appId);

                //也可以直接一步到位：
                //var result = AccessTokenContainer.TryGetAccessToken(appId, appSecret);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //TODO:为简化代码，这里不处理异常（如Token过期）
                return Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateMenu(string token, GetMenuResultFull resultFull, MenuMatchRule menuMatchRule)
        {
            var useAddCondidionalApi = menuMatchRule != null && !menuMatchRule.CheckAllNull();
            var apiName = string.Format("使用接口：{0}。", (useAddCondidionalApi ? "个性化菜单接口" : "普通自定义菜单接口"));
            try
            {
                //重新整理按钮信息
                WxJsonResult result = null;
                IButtonGroupBase buttonGroup = null;
                if (useAddCondidionalApi)
                {
                    //个性化接口
                    buttonGroup = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ConditionalButtonGroup()).menu;

                    var addConditionalButtonGroup = buttonGroup as ConditionalButtonGroup;
                    addConditionalButtonGroup.matchrule = menuMatchRule;
                    result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenuConditional(token, addConditionalButtonGroup);
                    apiName += string.Format("menuid：{0}。", (result as CreateMenuConditionalResult).menuid);
                }
                else
                {
                    //普通接口
                    buttonGroup = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                    result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(token, buttonGroup);
                }

                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = "菜单更新成功。" + apiName
                };
                return Json(json);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = string.Format("更新失败：{0}。{1}", ex.Message, apiName) };
                return Json(json);
            }
        }

        public ActionResult GetMenu(string token)
        {
            var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenu(token);
            if (result == null)
            {
                return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMenu(string token)
        {
            try
            {
                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.DeleteMenu(token);
                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = result.errmsg
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 用户处理（添加）微信自定义菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("AddMenu")]
        public ActionResult AddMenu()
        {
            //获取accessToken
            var accessToken = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret);
            //删除原有菜单
            var result1 = CommonApi.DeleteMenu(accessToken);

            //创建新菜单
            ButtonGroup bg = new ButtonGroup();

            //第1个一级菜单
            var firstButton = new SubButton()
            {
                name = "Ai我家"
            };
            firstButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinHome/Index",
                name = "首页"
            });
            firstButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinPropertyNotice/PropertyNoticeList/",
                name = "物业公告"
            });
            firstButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinQuestion/QuestionList/",
                name = "上报问题"
            });
            firstButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinExpenseNotice/ExpenseNoticeList/",
                name = "缴费通知"
            });
            firstButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinTopic/Index/",
                name = "社区话题"
            });
            bg.button.Add(firstButton);

            //第2个一级菜单
            var secondButton = new SingleViewButton()
            {
                url = "http://weidian.com/s/382033745?wfr=c/",
                name = "绿色直供"
            };
            bg.button.Add(secondButton);

            //第3个一级菜单
            var threeButton = new SubButton()
            {
                name = "更多"
            };
            threeButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinProduct/Index/",
                name = "产品介绍"
            });
            threeButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinProduct/QrCodeShare/",
                name = "分享友邻"
            });
            threeButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinSocialCircle/Index/",
                name = "业主圈子"
            });
            threeButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinPersonalCenter/Index/",
                name = "个人中心"
            });
            threeButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://v.homekeeper.com.cn/WeixinIdentityBind/Index/",
                name = "绑定小区"
            });
            bg.button.Add(threeButton);

            var result2 = CommonApi.CreateMenu(accessToken, bg);
            return Content("创建菜单成功，请查看。" + result1.errmsg + "+44" + result2.errmsg);
        }

        /// <summary>
        /// 微信错误页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Error() 
        {
            return View();
        }
    }
}
