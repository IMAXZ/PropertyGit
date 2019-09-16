using MvcBreadCrumbs;
using Property.Common;
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
    /// <summary>
    /// 沟通列表 控制器
    /// </summary>
    public class LinkupTypeController : BaseController
    {
        /// <summary>
        /// 沟通列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "沟通列表")]
        [HttpGet]
        public ActionResult LinkupTypeList(LinkupTypeSearchModel model)
        {
            //初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.BeforeDate == null)
                model.BeforeDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today; 

            //获取当前小区ID
            var propertyplaceid = GetSessionModel().PropertyPlaceId.Value;

            //根据发帖时间查询
            DateTime endTime = model.EndDate.Value.AddDays(1);
            Expression<Func<T_PostBarTopic, bool>> where = u => u.PropertyPlaceId == propertyplaceid && u.PostDate >= model.BeforeDate.Value && u.PostDate < endTime;

            //根据发帖人进行查询
            if (!string.IsNullOrEmpty(model.UserName))
            {
                where = PredicateBuilder.And(where, u => u.PostUser.UserName.Contains(model.UserName));
            }

            //根据查询条件调用BLL层 获取分页数据
            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            model.DataList = postBarTopicBll.GetSetTopPageList(where, model.PageIndex, ConstantParam.PAGE_SIZE) as PagedList<T_PostBarTopic>;

            return View(model);
        }

        /// <summary>
        /// 沟通内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "沟通内容")]
        [HttpGet]
        public ActionResult LinkupTypeContent(int id)
        {
            var propertyplaceId = GetSessionModel().PropertyPlaceId.Value;

            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            //获取要查看的主题表
            T_PostBarTopic postBarTopic = postBarTopicBll.GetEntity(u => u.Id == id && u.PropertyPlaceId == propertyplaceId);

            if (postBarTopic != null)
            {
                return View(postBarTopic);
            }

            return RedirectToAction("LinkupTypeList");
        }


        /// <summary>
        /// 删除沟通
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteLinkupType(int id)
        {
            JsonModel jm = new JsonModel();

            //获取要删除的帖子
            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            T_PostBarTopic post = postBarTopicBll.GetEntity(u => u.Id == id);

            //如果该帖子存在
            if (post == null)
            {
                jm.Msg = "该沟通不存在";
            }
            else
            {
                postBarTopicBll.DeleteTopic(id);

                //操作日志
                jm.Content = "删除沟通 " + post.Title;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除回复
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteReply(int id)
        {
            JsonModel jm = new JsonModel();

            //获取要删除的回复内容
            IPostBarTopicDiscussBLL postBarTopicDiscussBll = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
            T_PostBarTopicDiscuss discuss = postBarTopicDiscussBll.GetEntity(u => u.Id == id);

            //如果该回复存在
            if (discuss == null)
            {
                jm.Msg = "该回复不存在";
            }
            else
            {
                if (discuss.ParentId == null)
                {
                    postBarTopicDiscussBll.DeleteLevelOneDiscuss(discuss.Id);
                }
                else
                {
                    postBarTopicDiscussBll.Delete(discuss);
                }

                //操作日志
                jm.Content = "删除回复" + discuss.Content;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckTop()
        {
            JsonModel jm = new JsonModel();
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            var list = postBarTopicBll.GetList(p => p.IsTop == 1 && p.PropertyPlaceId == propertyPlaceId).ToList();

            if (list.Count >= 3)
            {
                jm.Msg = "置顶数量已达上限，请重新设置";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置主题置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetTopicTop(int id)
        {
            JsonModel jm = new JsonModel();

            //获取要操作的主题
            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            T_PostBarTopic post = postBarTopicBll.GetEntity(u => u.Id == id);

            if (post != null)
            {
                post.IsTop = 1;
                postBarTopicBll.Update(post);
                //操作日志
                jm.Content = "置顶主题" + post.Title;
            }
            else
            {
                jm.Msg = "该主题不存在";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消主题置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CancelTopicTop(int id)
        {
            JsonModel jm = new JsonModel();

            //获取要操作的主题
            IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            T_PostBarTopic post = postBarTopicBll.GetEntity(u => u.Id == id);

            if (post != null)
            {
                post.IsTop = 0;
                postBarTopicBll.Update(post);
                //操作日志
                jm.Content = "置顶主题" + post.Title;
            }
            else
            {
                jm.Msg = "该主题不存在";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
