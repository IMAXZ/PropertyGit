using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.ProductUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.ProductUI.Controllers
{
    /// <summary>
    /// 物业首页控制器
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.TenantId = PropertyUtils.GetConfigParamValue("TenantId");
            return View();
        }

        /// <summary>
        /// 申请试用提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public JsonResult ApplyProbation(ApplyModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //如果表单验证成功
                if (ModelState.IsValid)
                {
                    //添加申请信息
                    IApplyInfoBLL applyInfoBll = BLLFactory<IApplyInfoBLL>.GetBLL("ApplyInfoBLL");
                    T_ApplyInfo info = new T_ApplyInfo()
                    {
                        CompanyName = model.CompanyName,
                        Name = model.Name,
                        Tel = model.Tel,
                        OtherContactInfo = model.OtherContactInfo,
                        Memo = model.Memo,
                        ApplyTime = DateTime.Now
                    };
                    applyInfoBll.Save(info);

                    //发送邮件
                    string emailBody = GetMailBody(info);
                    if (!string.IsNullOrEmpty(emailBody))
                    {
                        string ApplyToName = PropertyUtils.GetConfigParamValue("ApplyToName");
                        string ApplyToAddress = PropertyUtils.GetConfigParamValue("ApplyToAddress");
                        bool flag = PropertyUtils.SendEmail(ApplyToAddress, ApplyToName, "物业生活通-试用申请", emailBody);
                        if (!flag)
                        {
                            jm.Msg = "邮件发送失败";
                        }
                    }
                }
                else
                {
                    jm.Msg = "表单验证失败";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 组装邮件内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetMailBody(T_ApplyInfo model)
        {
            string path = HttpContext.Server.MapPath("~/Template/ApplyTemplate.html");
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                var text = System.IO.File.ReadAllText(path);
                text = text.Replace("{{CompanyName}}", model.CompanyName);
                text = text.Replace("{{Name}}", model.Name);
                text = text.Replace("{{Tel}}", model.Tel);
                if (string.IsNullOrEmpty(model.OtherContactInfo))
                {
                    text = text.Replace("{{OtherContactInfo}}", "无");
                }
                else
                {
                    text = text.Replace("{{OtherContactInfo}}", model.OtherContactInfo);
                }
                if (string.IsNullOrEmpty(model.Memo))
                {
                    text = text.Replace("{{Memo}}", "无");
                }
                else
                {
                    text = text.Replace("{{Memo}}", model.Memo);
                }
                text = text.Replace("{{ApplyTime}}", model.ApplyTime.ToString("yyyy-MM-dd HH:mm"));
                return text;
            }
            return null;
        }

    }
}
