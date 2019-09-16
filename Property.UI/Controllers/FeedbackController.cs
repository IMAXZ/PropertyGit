using MvcBreadCrumbs;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    public class FeedbackController : BaseController
    {
        /// <summary>
        /// 意见反馈列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "意见反馈列表")]
        [HttpGet]
        public ActionResult FeedbackList(FeedbackSearchModel model)
        {
            //初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);
            Expression<Func<T_Feedback, bool>> where = u => u.UploadTime >= model.StartTime.Value && u.UploadTime < endTime;

            //根据查询条件调用BLL层 获取分页数据
            IFeedbackBLL feedbackBll = BLLFactory<IFeedbackBLL>.GetBLL("FeedbackBLL");
            var sortName = this.SettingSorting("Id", false);
            model.DataList = feedbackBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<T_Feedback>;
            return View(model);
        }

        /// <summary>
        /// 意见反馈详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "意见反馈详细")]
        public ActionResult FeedbackDetail(int id)
        {
            IFeedbackBLL feedbackBll = BLLFactory<IFeedbackBLL>.GetBLL("FeedbackBLL");
            T_Feedback feedback = feedbackBll.GetEntity(u => u.Id == id);

            if (feedback != null)
            {
                return View(feedback);
            }
            else
            {
                return RedirectToAction("FeedbackList");
            }
        }

        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteFeedback(int id)
        {
            JsonModel jm = new JsonModel();
            IFeedbackBLL feedbackBll = BLLFactory<IFeedbackBLL>.GetBLL("FeedbackBLL");
            T_Feedback feedback = feedbackBll.GetEntity(u => u.Id == id);

            if (feedback != null)
            {
                if (feedbackBll.Delete(feedback))
                {
                    if (!string.IsNullOrEmpty(feedback.Img))
                    {
                        //删除图片
                        string[] Imgs = feedback.Img.Split(';');

                        for (int i = 0; i < Imgs.Count(); i++)
                        {
                            DelFile(Imgs[i]);
                        }
                    }
                    jm.Content = "删除意见反馈" + feedback.Content + "成功";
                }
            }
            else
            {
                jm.Msg = "该意见反馈不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除文件方法
        /// </summary>
        /// <param name="file"></param>
        public void DelFile(string file)
        {
            string filestr = Server.MapPath(file);
            if (System.IO.File.Exists(filestr))
            {
                System.IO.File.Delete(filestr);
            }
        }
    }
}
