using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 微信产品介绍相关页面控制器
    /// </summary>
    public class WeixinProductController : Controller
    {
        /// <summary>
        /// Ai我家产品介绍页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Ai我家商家版产品介绍页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Store()
        {
            return View();
        }

        /// <summary>
        /// 二维码友邻分享页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QrCodeShare()
        {
            return View();
        }
    }
}
