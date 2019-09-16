using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{

    /// <summary>
    /// 门店账户模型
    /// </summary>
    public class ShopAccountModel
    {
        /// <summary>
        /// 微信账户Id
        /// </summary>
        public int? WeChatId { get; set; }

        /// <summary>
        /// 支付宝账户Id
        /// </summary>
        public int? AlipayId { get; set; }

        /// <summary>
        /// 微信APP ID
        /// </summary>
        public string WeChatNumber { get; set; }

        /// <summary>
        /// 微信商户号
        /// </summary>
        public string WeChatMerchantNo { get; set; }

        /// <summary>
        /// 微信秘钥
        /// </summary>
        public string WeChatKey { get; set; }

        /// <summary>
        /// 支付宝账号
        /// </summary>
        public string AlipayNumber { get; set; }

        /// <summary>
        /// 支付宝商户号
        /// </summary>
        public string AlipayMerchantNo { get; set; }

        /// <summary>
        /// 支付宝秘钥
        /// </summary>
        public string AlipayKey { get; set; }

        /// <summary>
        /// 私钥文件路径
        /// </summary>
        public HttpPostedFileBase PrivatePath { get; set; }

        /// <summary>
        /// 公钥文件路径
        /// </summary>
        public HttpPostedFileBase PublicPath { get; set; }
    }
}