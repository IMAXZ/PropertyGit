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
    /// 总公司新闻公告管理
    /// </summary>
    public class CompanyNewsController : BaseController
    {
        /// <summary>
        /// 新闻公告列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新闻公告列表")]
        [HttpGet]
        public ActionResult NewsList(CompanyNewsNoticeSearchModel model)
        {
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            int currentCompanyId = GetSessionModel().CompanyId.Value;
            Expression<Func<T_CompanyPost, bool>> where = u => u.CompanyId == currentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && (string.IsNullOrEmpty(model.Kword) ? true : u.Title.Contains(model.Kword)) && (model.PublishedFlag == null ? true : u.PublishStatus == model.PublishedFlag.Value);

            if (model.IsOpen != null) 
            {
                where = PredicateBuilder.And(where, u => u.IsOpen == model.IsOpen.Value);
            }
            //排序
            var sortModel = this.SettingSorting("Id", false);
            model.PostList = postBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_CompanyPost>;
            model.StatusList = getStatueList();
            model.IsOpenList = getIsOpenList();
            return View(model);
        }


        /// <summary>
        /// 返回公告状态类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> getStatueList()
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = "未发布",
                Value = ConstantParam.PUBLISHED_FALSE.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = "已发布",
                Value = ConstantParam.PUBLISHED_TRUE.ToString(),
                Selected = false
            });
            return typeList;
        }

        /// <summary>
        /// 返回公告公开状态列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> getIsOpenList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "未公开",
                Value = ConstantParam.PUBLISHED_FALSE.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已公开",
                Value = ConstantParam.PUBLISHED_TRUE.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "查看新闻公告详细")]
        public ActionResult NewsDetail(int id)
        {
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                return View(post);
            }
            else
            {
                return RedirectToAction("NewsList");
            }
        }

        /// <summary>
        /// 发布新公告
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "发布新闻公告")]
        [HttpGet]
        public ActionResult AddNews()
        {
            CompanyNoticeModel model = new CompanyNoticeModel();
            model.PublishedFlag = false;
            model.IsOpen = false;
            return View(model);
        }


        /// <summary>
        /// 发布新公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNews(CompanyNoticeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
                T_CompanyPost newPost = new T_CompanyPost()
                {
                    Title = model.Title,
                    Content = model.Content,
                    CompanyId = GetSessionModel().CompanyId.Value,
                    SubmitUserId = GetSessionModel().UserID,
                    PublishStatus = model.PublishedFlag ? 1 : 0,
                    IsOpen = model.IsOpen ? 1 : 0
                };
                if (model.PublishedFlag) 
                {
                    newPost.PublishedTime = DateTime.Now;
                }
                // 保存到数据库
                postBll.Save(newPost);

                if (model.PublishedFlag && model.IsOpen)
                {
                    //推送给物业客户端
                    IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                    var PropertyUserIds = userBll.GetList(u => u.PropertyPlace.CompanyId == newPost.CompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(u => u.Id);
                    if (PropertyUserIds != null)
                    {
                        PropertyUserIds = PropertyUserIds.ToList();
                    }
                    IPropertyUserPushBLL propertyUserPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                    var propertyRegistrationIds = propertyUserPushBLL.GetList(p => PropertyUserIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                    bool flag = PropertyUtils.SendPush("总公司新闻公告", model.Title, ConstantParam.MOBILE_TYPE_PROPERTY, propertyRegistrationIds);
                    if (!flag)
                    {
                        jm.Msg = "推送发生异常";
                    }
                }

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 公告修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑新闻公告")]
        [HttpGet]
        public ActionResult EditNews(int id)
        {
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                CompanyNoticeModel postModel = new CompanyNoticeModel();
                postModel.PostId = post.Id;
                postModel.Title = post.Title;
                postModel.Content = post.Content;
                postModel.PublishedFlag = post.PublishStatus == 1;
                postModel.IsOpen = post.IsOpen == 1;
                return View(postModel);
            }
            else
            {
                return RedirectToAction("NewsList");
            }
        }

        /// <summary>
        /// 公告修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditNews(CompanyNoticeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
                T_CompanyPost newPost = postBll.GetEntity(m => m.Id == model.PostId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (newPost != null)
                {
                    newPost.Title = model.Title;
                    newPost.Content = model.Content;
                    newPost.PublishStatus = model.PublishedFlag ? 1 : 0;
                    newPost.IsOpen = model.IsOpen ? 1 : 0;
                };
                if (model.PublishedFlag)
                {
                    newPost.PublishedTime = DateTime.Now;
                }
                // 保存到数据库,如果修改成功
                if (postBll.Update(newPost))
                {
                    //如果已发布并公开
                    if (model.PublishedFlag && model.IsOpen)
                    {
                        //推送给物业客户端
                        IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                        var PropertyUserIds = userBll.GetList(u => u.PropertyPlace.CompanyId == newPost.CompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(u => u.Id);
                        if (PropertyUserIds != null)
                        {
                            PropertyUserIds = PropertyUserIds.ToList();
                        }
                        IPropertyUserPushBLL propertyUserPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                        var propertyRegistrationIds = propertyUserPushBLL.GetList(p => PropertyUserIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                        bool flag = PropertyUtils.SendPush("总公司新闻公告", model.Title, ConstantParam.MOBILE_TYPE_PROPERTY, propertyRegistrationIds);
                        if (!flag)
                        {
                            jm.Msg = "推送发生异常";
                        }
                    }
                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else 
                {
                    jm.Msg = "新闻公告编辑失败";
                }
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除新闻公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteNews(int id)
        {
            JsonModel jm = new JsonModel();

            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                // 修改指定用户记录中的已删除标识
                post.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                postBll.Update(post);
                //操作日志
                jm.Content = "删除总公司新闻公告 " + post.Title;
            }
            else
            {
                jm.Msg = "该新闻公告不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
