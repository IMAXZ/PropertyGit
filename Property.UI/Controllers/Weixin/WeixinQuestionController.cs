using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    public class WeixinQuestionController : WeixinBaseController
    {


        /// <summary>
        /// 上报问题列表
        /// </summary>
        /// <returns></returns>
        public ActionResult QuestionList()
        {
            var userId = GetCurrentUser().Id;
            var placeIds = GetVerifiedPlaceIds();
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            ViewBag.QuestionTotal = questionBll.Count(m => m.UploadUserId == userId && placeIds.Contains(m.PropertyPlaceId));

            return View();
        }

        /// <summary>
        /// 上报问题Json方式获取
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult QuestionJsonList(int pageIndex)
        {
            //获取当前用户
            var userId = GetCurrentUser().Id;
            var placeIds = GetVerifiedPlaceIds();

            PageResultModel model = new PageResultModel();

            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");

            model.Total = questionBll.Count(m => m.UploadUserId == userId && placeIds.Contains(m.PropertyPlaceId));
            model.Result = questionBll.GetPageList(m => m.UploadUserId == userId && placeIds.Contains(m.PropertyPlaceId), "UploadTime", false, pageIndex).Select(q => new
            {
                Id = q.Id,
                Title = q.Title,
                Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                Status = q.Status,
                StatusImage = q.Status == 0 ? "/Images/WeiXin/unhandled.png" : "/Images/WeiXin/handled.png",
                UploadTime = q.UploadTime,
                strUploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[] { } : q.Imgs.Contains(";") ? q.Imgs.Split(';') : new string[] { q.Imgs },
                ImgCount = string.IsNullOrEmpty(q.Imgs) ? 0 : q.Imgs.Contains(";") ? q.Imgs.Split(';').Count() : 1,
                AudioPath = q.AudioPath,
                VoiceDuration = q.VoiceDuration,
                PropertyName = q.PropertyPlace.Name,
                DisposesTime = q.Status == ConstantParam.NO_DISPOSE ? null : q.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上报问题详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult QuestionDetail(int id)
        {
            WeixinApiInit();

            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            var question = questionBll.GetEntity(m => m.Id == id);

            var model = new QuestionDetailModel
            {
                Id = question.Id,
                Title = question.Title,
                Desc = string.IsNullOrEmpty(question.Desc) ? "" : question.Desc,
                Status = question.Status,
                UploadTime = question.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Imgs = string.IsNullOrEmpty(question.Imgs) ? new string[] { } : question.Imgs.Split(';'),
                AudioPath = question.AudioPath,
                VoiceDuration = question.VoiceDuration,
                PropertyName = question.PropertyPlace.Name,
                DisposesTime = question.Status == ConstantParam.NO_DISPOSE ? null : question.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss"),
                DisposeDesc = question.Status == ConstantParam.DISPOSED ? question.QuestionDisposes.FirstOrDefault().DisposeDesc : ""
            };

            return View(model);
        }

        /// <summary>
        /// 上报问题
        /// </summary>
        /// <returns></returns>
        public ActionResult QuestionReport()
        {

            ViewBag.propertylist = InitPropertyList();

            WeixinApiInit();
            return View();
        }

        /// <summary>
        /// 上报问题提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QuestionReport(T_Question model)
        {
            //点击图片input触发的提交事件，不进行提交
            if (model.Id == 1)
            {
                return null;
            }
            else
            {
                int userId = GetCurrentUser().Id;
                JsonModel jm = new JsonModel();
                try
                {
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");

                    T_Question question = new T_Question();
                    question.UploadUserId = userId;
                    question.UploadTime = DateTime.Now;
                    question.ClientSaveTime = DateTime.Now;
                    question.Title = model.Title;
                    question.Status = 0;
                    question.PropertyPlaceId = model.PropertyPlaceId;

                    if (string.IsNullOrEmpty(model.Imgs))
                    {
                        question.Imgs = "";
                    }
                    else
                    {
                        question.Imgs = GetMultimedia(ConstantParam.QUESTION_DIR, model.Imgs);
                    }
                    question.Desc = model.Desc;
                    question.IsPublish = 0;

                    questionBll.Save(question);
                }
                catch
                {
                    jm.Msg = "提交失败";
                }
                return Json(jm, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 小区列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> InitPropertyList()
        {
            string ids = string.Join(",", GetVerifiedPlaceIds().ToArray());
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var placelist = placeBll.GetList(x => ids.Contains(x.Id.ToString()));

            List<SelectListItem> itemList = new List<SelectListItem>();
            foreach (var item in placelist)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return itemList;
        }

    }
}
