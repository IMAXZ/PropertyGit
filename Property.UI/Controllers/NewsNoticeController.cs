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
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 新闻公告控制器类
    /// </summary>
    public class NewsNoticeController : BaseController
    {
        /// <summary>
        /// 新闻公告列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "物业新闻公告列表")]
        [HttpGet]
        public ActionResult NoticeList(NewsNoticeSearchModel model)
        {
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_Post, bool>> where = u => u.PropertyPlaceId == propertyPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && (string.IsNullOrEmpty(model.Title) ? true : u.Title.Contains(model.Title)) && (model.PublishedFlag == null ? true : u.PublishedFlag == model.PublishedFlag.Value);

            //排序
            var sortModel = this.SettingSorting("Id", false);
            model.PostList = postBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_Post>;
            model.StatueList = getStatueList();
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
        /// 发布新公告
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增新闻公告")]
        [HttpGet]
        public ActionResult AddNews()
        {
            NewsNoticeModel model = new NewsNoticeModel();
            model.PublishedFlag = false;
            return View(model);
        }


        /// <summary>
        /// 发布新公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNews(NewsNoticeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                var sessionModel = GetSessionModel();
                IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
                T_Post newPost = new T_Post()
                {
                    Title = model.Title,
                    Content = model.Content,
                    PropertyPlaceId = sessionModel.PropertyPlaceId.Value,
                    SubmitUserId = sessionModel.UserID,
                    SubmitTime = DateTime.Now.ToLocalTime(),
                    PublishedFlag = model.PublishedFlag ? 1 : 0,
                    PublishedTime = DateTime.Now.ToLocalTime()
                };
                // 保存到数据库
                postBll.Save(newPost);

                // 若选中“发布选项”则即时推送
                if (model.PublishedFlag)
                {
                    // 公告推送
                    //推送给业主客户端
                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    var userIds = placeBll.GetEntity(p => p.Id == newPost.PropertyPlaceId).UserPlaces.Select(m => m.UserId);
                    if (userIds != null)
                    {
                        userIds = userIds.ToList();
                    }
                    IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    var registrationIds = userPushBLL.GetList(p => userIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                    bool flag = PropertyUtils.SendPush("新闻公告", model.Title, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);

                    //推送给物业客户端
                    IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                    var PropertyUserIds = userBll.GetList(u => u.PropertyPlaceId == newPost.PropertyPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(u => u.Id);
                    if (PropertyUserIds != null)
                    {
                        PropertyUserIds = PropertyUserIds.ToList();
                    }
                    IPropertyUserPushBLL propertyUserPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                    var propertyRegistrationIds = propertyUserPushBLL.GetList(p => PropertyUserIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                    bool flag1 = PropertyUtils.SendPush("新闻公告", model.Title, ConstantParam.MOBILE_TYPE_PROPERTY, propertyRegistrationIds);
                    if (!flag || !flag1)
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
        /// 查看公告详细
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新闻公告查看")]
        [HttpGet]
        public ActionResult ScanPost(int id)
        {
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                NewsNoticeModel postModel = new NewsNoticeModel();
                postModel.PostId = post.Id;
                postModel.Title = post.Title;
                postModel.Content = post.Content;
                postModel.SubmitUserId = post.SubmitUserId;
                postModel.SubmitUser = post.SubmitUser.TrueName;
                postModel.SubmitTime = post.SubmitTime.ToString();
                postModel.PuslishedTime = post.PublishedTime.ToString();
                postModel.PublishedFlag = post.PublishedFlag == 0 ? false : true;
                postModel.SubmitUserHeadPath = post.SubmitUser.HeadPath;
                return View(postModel);
            }
            else
            {
                return RedirectToAction("NoticeList");
            }
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePost(int id)
        {
            JsonModel jm = new JsonModel();

            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            // 根据指定id值获取实体对象
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (post != null)
            {
                // 修改指定用户记录中的已删除标识
                post.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                postBll.Update(post);
                //操作日志
                jm.Content = "删除公告 " + post.Title;
            }
            else
            {
                jm.Msg = "该公告不存在";
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
            IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                NewsNoticeModel postModel = new NewsNoticeModel();
                postModel.PostId = post.Id;
                postModel.Title = post.Title;
                postModel.Content = post.Content;
                postModel.SubmitUserId = post.SubmitUserId;
                postModel.SubmitUser = post.SubmitUser.TrueName;
                postModel.SubmitTime = post.SubmitTime.ToString();
                postModel.SubmitUserHeadPath = post.SubmitUser.HeadPath;
                postModel.PublishedFlag = post.PublishedFlag == 1 ? true : false;
                postModel.PuslishedTime = post.PublishedTime.ToString();
                return View(postModel);
            }
            else
            {
                return RedirectToAction("NoticeList");
            }
        }

        /// <summary>
        /// 公告修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditNews(NewsNoticeModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                var sessionModel = GetSessionModel();
                IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
                T_Post newPost = postBll.GetEntity(m => m.Id == model.PostId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (newPost != null)
                {
                    newPost.Title = model.Title;
                    newPost.Content = model.Content;
                    newPost.PropertyPlaceId = sessionModel.PropertyPlaceId.Value;
                    newPost.SubmitUserId = sessionModel.UserID;
                    newPost.PublishedTime = DateTime.Now.ToLocalTime();
                    newPost.PublishedFlag = model.PublishedFlag ? 1 : 0;
                };
                // 保存到数据库
                if (postBll.Update(newPost))
                {
                    // 若选中“发布选项”则即时推送
                    if (model.PublishedFlag)
                    {
                        // 公告推送
                        //推送给业主客户端
                        IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                        var userIds = placeBll.GetEntity(p => p.Id == newPost.PropertyPlaceId).UserPlaces.Select(m => m.UserId);
                        if (userIds != null)
                        {
                            userIds = userIds.ToList();
                        }
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var registrationIds = userPushBLL.GetList(p => userIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                        bool flag = PropertyUtils.SendPush("新闻公告", model.Title, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);

                        //推送给物业客户端
                        IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                        var PropertyUserIds = userBll.GetList(u => u.PropertyPlaceId == newPost.PropertyPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(u => u.Id);
                        if (PropertyUserIds != null)
                        {
                            PropertyUserIds = PropertyUserIds.ToList();
                        }
                        IPropertyUserPushBLL propertyUserPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                        var propertyRegistrationIds = propertyUserPushBLL.GetList(p => PropertyUserIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                        bool flag1 = PropertyUtils.SendPush("新闻公告", model.Title, ConstantParam.MOBILE_TYPE_PROPERTY, propertyRegistrationIds);
                        if (!flag || !flag1)
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
        /// 总公司新闻公告一览
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "总公司新闻公告一览")]
        [HttpGet]
        public ActionResult CompanyNewsList(SearchModel model)
        {
            //获取当前小区所属公司ID
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = placeBll.GetEntity(p => p.Id == CurrentPlaceId);
            int currentCompanyId = place.CompanyId;

            //查询条件初始化
            Expression<Func<T_CompanyPost, bool>> where = u => u.CompanyId == currentCompanyId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && (string.IsNullOrEmpty(model.Kword) ? true : u.Title.Contains(model.Kword)) && u.PublishStatus == ConstantParam.PUBLISHED_TRUE
                && u.IsOpen == ConstantParam.PUBLISHED_TRUE;
            //排序
            var sortModel = this.SettingSorting("Id", false);

            //获取分页数据
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var PostList = postBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(PostList);
        }


        /// <summary>
        /// 查看新闻公告详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "查看新闻公告详细")]
        public ActionResult CompanyNewsDetail(int id)
        {
            ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
            var post = postBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (post != null)
            {
                return View(post);
            }
            else
            {
                return RedirectToAction("CompanyNewsList");
            }
        }
    }
}
