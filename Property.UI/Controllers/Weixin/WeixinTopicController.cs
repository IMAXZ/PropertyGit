using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    public class WeixinTopicController : WeixinBaseController
    {
        /// <summary>
        /// 话题首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new TopicHomePageModel();

            //获取当前用户信息
            var owner = GetCurrentUser();

            //获取验证过的小区Ids
            var verifiedPlaceIds = GetVerifiedPlaceIds();

            //获取当前用户的小区信息列表
            foreach (var propertyInfo in owner.UserPlaces.Where(m => verifiedPlaceIds.Contains(m.PropertyPlaceId)).ToList())
            {
                model.PropertyList.Add(new PropertyInfoModel()
                {
                    PropertyId = propertyInfo.PropertyPlaceId,
                    PropertyName = propertyInfo.PropertyPlace.Name,
                    PropertyHeadImg = propertyInfo.PropertyPlace.Img
                });
            }

            //获取当前用户发表的主题信息列表（默认前三条，置顶优先，再按发表时间降序）
            IPostBarTopicBLL topicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            var topicList = topicBll.GetSetTopPageList(m => m.PostUserId == owner.Id && verifiedPlaceIds.Contains(m.PropertyPlaceId), 1, 3).ToList();

            foreach (var topicInfo in topicList)
            {
                model.TopicList.Add(new TopicInfoModel()
                {
                    TopicId = topicInfo.Id,
                    TopicType = topicInfo.PostBarTopicType.Name,
                    TopicTitle = topicInfo.Title,
                    TopicContent = topicInfo.Content,
                    PostDate = TimeFormat(topicInfo.PostDate),
                    PostUserName = topicInfo.PostUser.UserName,
                    PostUserHeadImg = topicInfo.PostUser.HeadPath,
                    TopicImgList = topicInfo.ImgPath,
                    TopicDiscussTotal = topicInfo.PostBarTopicDiscusss.Count(m => m.ParentId == null)
                });
            }

            return View(model);
        }
        /// <summary>
        /// 小区话题圈
        /// </summary>
        /// <returns></returns>
        public ActionResult PlaceTopicList(int placeId)
        {
            PropertyUtils.WriteLogInfo("test");

            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var model = placeBll.GetEntity(m => m.Id == placeId);
            if (model != null)
            {
                @ViewBag.Title = model.Name;
            }
            @ViewBag.PlaceId = placeId;
            return View(model);
        }
        /// <summary>
        /// 小区话题圈Json方式获取
        /// </summary>
        /// <param name="placeId"></param>
        /// <param name="index"></param>
        /// <param name="topicType"></param>
        /// <returns></returns>
        public JsonResult PlaceJsonTopicList(int placeId, int index, int topicType)
        {
            PageResultModel model = new PageResultModel();

            IPostBarTopicBLL topicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            var topicList = topicBll.GetSetTopPageList(m => m.PropertyPlaceId == placeId, index, ConstantParam.PAGE_SIZE).ToList();

            Expression<Func<T_PostBarTopic, bool>> where = m => m.PropertyPlaceId == placeId;

            if (topicType > 0)
            {
                where = PredicateBuilder.And(where, m => m.TopicTypeId == topicType);
            }

            model.Total = topicBll.Count(where);
            model.Result = topicBll.GetSetTopPageList(where, index, ConstantParam.PAGE_SIZE).Select(m => new
            {
                TopicId = m.Id,
                TopicType = m.PostBarTopicType.Name,
                TopicTitle = m.Title,
                TopicContent = m.Content,
                PostDate = TimeFormat(m.PostDate),
                PostUserName = m.PostUser.UserName,
                PostUserHeadImg = m.PostUser.HeadPath,
                TopicImgList = m.ImgPath,
                TopicDiscussTotal = m.PostBarTopicDiscusss.Count(o => o.ParentId == null),
                IsTop = m.IsTop
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 主题详细列表
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="isTag"> 0表示所有楼层都显示 其它Id代表具体那楼层Id。</param>
        /// <returns></returns>
        public ActionResult TopicDetailList(int topicId, int floorId)
        {
            int CurrentUserId = GetCurrentUser().Id;
            WeixinApiInit();

            @ViewBag.TopicId = topicId;
            IPostBarTopicBLL topicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            var topic = topicBll.GetEntity(m => m.Id == topicId);

            var model = new TopicDetailModel()
            {
                Id = topic.Id,
                Title = topic.Title,
                Content = topic.Content,
                PostUserId = topic.PostUserId,
                PostUserName = topic.PostUser.UserName,
                PostUserHeadPath = topic.PostUser.HeadPath,
                PostDate = TimeFormat(topic.PostDate),
                TopicImgPath = topic.ImgPath,
                PropertyPlaceId = topic.PropertyPlaceId,
                LevelOneReplyCount = topic.PostBarTopicDiscusss.Count(m => m.ParentId == null),
                CurrentUserId = CurrentUserId,
                IsTop = topic.IsTop,
                FloorId = floorId // 0表示所有楼层都显示 其它Id代表具体那楼层Id。
            };

            return View(model);
        }
        /// <summary>
        /// 主题详细列表Json方式获取
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="isTag"> 0表示所有楼层都显示 其它Id代表具体那楼层Id。</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult TopicDetailJsonList(int topicId, int floorId, int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
            //主题下面所有的一级Ids
            var level1Ids = topicDiscussBLL.GetList(m => m.TopicId == topicId && m.ParentId == null, "PostTime", true).Select(m => m.Id).ToList();

            Expression<Func<T_PostBarTopicDiscuss, bool>> where = m => true;

            if (floorId > 0)
            {
                where = PredicateBuilder.And(where, m => m.TopicId == topicId && m.Id == floorId);
            }
            else
            {
                where = PredicateBuilder.And(where, m => m.TopicId == topicId && m.ParentId == null);
            }

            // 获取我的帖子回复列表
            var list = topicDiscussBLL.GetPageList(where, "PostTime", true, pageIndex, ConstantParam.PAGE_SIZE).Select(m => new
            {
                Id = m.Id,
                UserId = m.PostUserId,
                PostDate = TimeFormat(m.PostTime),
                UserImage = m.PostUser.HeadPath,
                UserName = m.PostUser.UserName,
                Content = m.Content == null ? "" : m.Content,
                PicList = m.ImgPath,
                RepliedUserName = m.ReplyUser.UserName,
                Level2Count = m.PostBarTopicDiscusses.Count(),
                PropertyPlaceId = m.PostBarTopic.PropertyPlaceId,
                FloorNo = level1Ids.FindIndex(l => l == m.Id) + 1,

                Level2DiscussList = m.PostBarTopicDiscusses.Select(o =>
                    new
                    {
                        Level2Id = o.Id,
                        Level2ParentId = o.ParentId,
                        Level2UserId = o.PostUserId,
                        Level2UserName = o.PostUser.UserName,
                        Level2Content = o.Content,
                        Level2PostDate = TimeFormat(o.PostTime),
                        Level2RepliedUserName = o.ReplyUser.UserName,
                        Level2PicList = o.ImgPath
                    }).OrderByDescending(o => o.Level2PostDate).Take(2).ToList(),
                Level2DiscussListCount = m.PostBarTopicDiscusses.Count()
            }).ToList();

            model.Result = list;
            model.Total = topicDiscussBLL.Count(m => m.TopicId == topicId && m.ParentId == null);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 话题圈发布
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicReport(int id)
        {
            ViewBag.topicTypelist = GetTopicTypeList(id);
            ViewBag.propertyPlaceId = id;
            WeixinApiInit();

            return View();
        }
        /// <summary>
        /// 发布话题圈提交
        /// </summary>
        /// <param name="model">话题圈实体</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TopicReport(T_PostBarTopic model)
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
                    IPostBarTopicBLL topicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");

                    T_PostBarTopic topic = new T_PostBarTopic();
                    topic.PostUserId = userId;
                    topic.PostDate = DateTime.Now;
                    topic.Title = model.Title;
                    topic.IsTop = 0;
                    topic.PropertyPlaceId = model.PropertyPlaceId;
                    if (string.IsNullOrEmpty(model.ImgPath))
                    {
                        topic.ImgPath = "";
                    }
                    else
                    {
                        topic.ImgPath = GetMultimedia(ConstantParam.TOPIC_DIR, model.ImgPath);
                    }
                    topic.Content = model.Content;
                    topic.TopicTypeId = model.TopicTypeId;
                    topic.IsTop = 0;

                    topicBll.Save(topic);
                }
                catch
                {
                    jm.Msg = "发布失败";
                }
                return Json(jm, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 话题类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetTopicTypeList(int placeId)
        {

            IPostBarTopicTypeBLL typeBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
            var typelist = typeBll.GetList(x => x.PropertyPlaceId == placeId);

            List<SelectListItem> itemList = new List<SelectListItem>();
            foreach (var item in typelist)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            itemList.Add(new SelectListItem()
            {
                Text = "话题分类",
                Value = "0",
                Selected = true
            });

            return itemList;
        }
        /// <summary>
        /// 回复主题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReplyTopic(TopicDetailModel model)
        {
            JsonModel jm = new JsonModel();

            try
            {
                int CurrentUserId = GetCurrentUser().Id;
                IPostBarTopicDiscussBLL replyTopicBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                var replyTopic = new T_PostBarTopicDiscuss()
                {
                    Content = model.ReplyTopicContent,
                    ReplyId = model.PostUserId,
                    PostUserId = CurrentUserId,
                    PostTime = DateTime.Now,
                    TopicId = model.Id
                };

                //图片上传
                if (!string.IsNullOrEmpty(model.ReplyTopicImgList))
                {

                    //图片集路径保存
                    replyTopic.ImgPath = GetMultimedia(ConstantParam.TOPIC_DIR, model.ReplyTopicImgList);

                    StringBuilder imgsSB = new StringBuilder();
                    //生成缩略图保存
                    foreach (var path in replyTopic.ImgPath.Split(';'))
                    {
                        string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
                        string thumpPath = Path.Combine(Server.MapPath(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId), thumpFile);
                        PropertyUtils.getThumImage(Path.Combine(Server.MapPath(path)), 18, 3, thumpPath);
                        imgsSB.Append(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId + "/" + thumpFile + ";");
                    }

                    replyTopic.ImgThumbnail = imgsSB.ToString();
                    replyTopic.ImgThumbnail = replyTopic.ImgThumbnail.Substring(0, replyTopic.ImgThumbnail.Length - 1);
                }

                replyTopicBLL.Save(replyTopic);
            }
            catch (Exception ex)
            {
                jm.Msg = "回复主题失败";
                PubFunction.ErrorLogPrint("话题圈回复错误：", ex.ToString());
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 楼层详细列表
        /// </summary>
        /// <param name="floorId">一级回复的主键Id</param>
        /// <param name="replyId">准备要回复对方的Id</param>
        /// <returns></returns>
        public ActionResult FloorDetailList(int floorId, int replyId)
        {
            int CurrentUserId = GetCurrentUser().Id;

            WeixinApiInit();

            //获取当前楼层（也就是一级回复的）信息
            IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
            var topicDiscuss = topicDiscussBLL.GetEntity(m => m.Id == floorId);
            //主题下面所有的一级Ids
            var level1Ids = topicDiscussBLL.GetList(m => m.TopicId == topicDiscuss.TopicId && m.ParentId == null).OrderBy(m => m.PostTime).Select(m => m.Id).ToList();
            //当前是第几楼
            int FloorNo = level1Ids.FindIndex(m => m == floorId) + 1;
            ViewBag.FloorNo = string.Format("{0}楼", FloorNo);

            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");

            var replidUserName = "";

            var replyUser = userBll.GetEntity(m => m.Id == replyId);
            replidUserName = string.Format("回复{0}:", replyUser.UserName);

            var model = new TopicFloorDetailModel()
            {
                FloorId = topicDiscuss.Id,
                FloorNo = string.Format("第{0}楼", FloorNo),
                PostUserId = topicDiscuss.PostUserId,
                PostUserName = topicDiscuss.PostUser.UserName,
                PostUserHeadPath = topicDiscuss.PostUser.HeadPath,
                ImgPath = topicDiscuss.ImgPath,
                ImgThumbnail = topicDiscuss.ImgThumbnail,
                PostDate = string.Format("第{0}楼 {1}", FloorNo, TimeFormat(topicDiscuss.PostTime)),
                Content = topicDiscuss.Content,
                PropertyPlaceId = topicDiscuss.PostBarTopic.PropertyPlaceId,
                TopicId = topicDiscuss.PostBarTopic.Id,
                LevelTwoReplyCount = topicDiscussBLL.Count(m => m.ParentId == topicDiscuss.Id && m.TopicId == topicDiscuss.TopicId),
                ReplyId = replyId,
                ReplidUserName = replidUserName,
                CurrentUserId = CurrentUserId,
                CanDelete = topicDiscuss.PostUserId == CurrentUserId
            };

            return View(model);
        }
        /// <summary>
        /// 楼层详细列表Json方式获取
        /// </summary>
        /// <param name="floorId"></param>
        /// <param name="topicId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult FloorDetailJsonList(int floorId, int topicId, int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
            // 获取我的帖子回复列表
            var list = topicDiscussBLL.GetPageList(m => m.TopicId == topicId && m.ParentId == floorId, "PostTime", true, pageIndex, ConstantParam.PAGE_SIZE).Select(m => new
            {
                Level2Id = m.Id,
                Level2UserName = m.PostUser.UserName,
                Level2Content = m.Content,
                Level2PostDate = TimeFormat(m.PostTime),
                Level2PostUserId = m.PostUserId,
                Level2RepliedUserName = string.Format("回复 {0}:", m.ReplyUser.UserName),
                CanDelete = GetCurrentUser().Id == m.PostUserId
            }).ToList();

            model.Result = list;
            model.Total = topicDiscussBLL.Count(m => m.TopicId == topicId && m.ParentId == floorId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReplyFloor(TopicFloorDetailModel model)
        {
            JsonModel jm = new JsonModel();

            try
            {
                int CurrentUserId = GetCurrentUser().Id;
                IPostBarTopicDiscussBLL replyTopicBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                var replyTopic = new T_PostBarTopicDiscuss()
                {
                    Content = model.ReplyContent,
                    ReplyId = model.ReplyId,
                    PostUserId = CurrentUserId,
                    PostTime = DateTime.Now,
                    TopicId = model.TopicId,
                    ParentId = model.FloorId
                };

                replyTopicBLL.Save(replyTopic);
            }
            catch
            {
                jm.Msg = "回复楼层失败";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        public string GetRepliedName(int replyId)
        {
            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            var user = userBll.GetEntity(m => m.Id == replyId);
            var repliedName = string.Format("回复{0}:", user.UserName);
            return repliedName;
        }

        /// <summary>
        /// 我的话题列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MyTopicList()
        {
            ViewBag.CurrentUserId = GetCurrentUser().Id;
            return View();
        }

        /// <summary>
        /// 我的话题列表Json方式获取
        /// </summary>
        /// <returns></returns>
        public JsonResult MyTopicJsonList(int pageIndex)
        {
            int CurrentUserId = GetCurrentUser().Id;
            PageResultModel model = new PageResultModel();

            //获取验证过的小区Ids
            var verifiedPlaceIds = GetVerifiedPlaceIds();

            IPostBarTopicBLL topicBLL = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
            // 获取我的主题列表
            model.Result = topicBLL.GetSetTopPageList(m => m.PostUserId == CurrentUserId && verifiedPlaceIds.Contains(m.PropertyPlaceId), pageIndex, ConstantParam.PAGE_SIZE).Select(m => new
            {
                Id = m.Id,
                PostUserId = m.PostUserId,
                PostDate = TimeFormat(m.PostDate),
                IsTop = m.IsTop,
                UserImage = m.PostUser.HeadPath,
                UserName = m.PostUser.UserName,
                Title = m.Title,
                Content = m.Content,
                PicList = m.ImgPath,
                TopicType = m.PostBarTopicType.Name,
                CommentCount = m.PostBarTopicDiscusss.Count(o => o.ParentId == null),
                PropertyPlaceId = m.PropertyPlaceId,
                PropertyName = m.PropertyPlace.Name
            }).ToList();

            model.Total = topicBLL.Count(m => m.PostUserId == CurrentUserId && verifiedPlaceIds.Contains(m.PropertyPlaceId));

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 我的回复列表Json方式获取
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult MyReplyJsonList(int pageIndex)
        {
            int CurrentUserId = GetCurrentUser().Id;
            PageResultModel model = new PageResultModel();

            //获取验证过的小区Ids
            var verifiedPlaceIds = GetVerifiedPlaceIds();

            IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
            // 获取我的回复列表
            var list = topicDiscussBLL.GetPageList(m => (m.ReplyId == CurrentUserId || m.PostUserId == CurrentUserId) && verifiedPlaceIds.Contains(m.PostBarTopic.PropertyPlaceId), "PostTime", false, pageIndex, ConstantParam.PAGE_SIZE).Select(m => new
            {
                Id = m.Id,
                PostUserId = m.PostUserId,
                PostDate = TimeFormat(m.PostTime),
                UserImage = m.PostUser.HeadPath,
                UserName = m.PostUser.UserName,
                Title = m.PostBarTopic.Title,
                Content = m.Content,
                PicList = m.ImgPath,
                TopicType = m.PostBarTopic.PostBarTopicType.Name,
                CommentCount = m.PostBarTopic.PostBarTopicDiscusss.Count(o => o.ParentId == null),
                ParentId = m.ParentId,
                TopicId = m.TopicId,
                PropertyPlaceId = m.PostBarTopic.PropertyPlaceId,
                FloorId = m.ParentId == null ? m.Id : m.ParentId,
                PropertyName = m.PostBarTopic.PropertyPlace.Name,
                ReplyId = m.ParentId == null ? m.PostUserId : m.ParentReply.PostUserId
            }).ToList();

            model.Result = list;
            model.Total = topicDiscussBLL.Count(m => (m.ReplyId == CurrentUserId || m.PostUserId == CurrentUserId) && verifiedPlaceIds.Contains(m.PostBarTopic.PropertyPlaceId));

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public JsonResult DelReply(int id, int level)
        {
            JsonModel jm = new JsonModel();
            try
            {
                IPostBarTopicDiscussBLL topicDiscussBll = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                var topicDiscuss = topicDiscussBll.GetEntity(p => p.Id == id);

                if (topicDiscuss == null)
                {
                    jm.Msg = "该回复不存在";
                }
                else
                {
                    if (level == 1)
                    {
                        topicDiscussBll.DeleteLevelOneDiscuss(id);
                    }
                    else
                    {
                        topicDiscussBll.Delete(topicDiscuss);
                    }
                }
            }
            catch
            {
                jm.Msg = "删除发生异常";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 话题圈-删除主题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelTopic(int id)
        {
            JsonModel jm = new JsonModel();

            try
            {
                //获取要删除的帖子
                IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                T_PostBarTopic topic = postBarTopicBll.GetEntity(u => u.Id == id);

                if (topic != null)
                {
                    postBarTopicBll.DeleteTopic(id);
                }
                else
                {
                    jm.Msg = "该主题不存在";
                }
            }
            catch
            {
                jm.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
