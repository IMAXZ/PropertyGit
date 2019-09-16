using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 微信生活办事模块
    /// </summary>
    public class WeixinBanshiController : Controller
    {
        /// <summary>
        /// 微信生活办事首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}
