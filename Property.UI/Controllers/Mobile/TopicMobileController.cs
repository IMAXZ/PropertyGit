using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 话题圈相关的API接口控制器
    /// </summary>
    public class TopicMobileController : ApiController
    {
        /// <summary>
        /// 话题圈-发贴
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel PostTopic(PostTopicModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    var postTopic = new T_PostBarTopic()
                    {
                        PostUserId = model.UserId,
                        PostDate = DateTime.Now,
                        PropertyPlaceId = model.PropertyPlaceId,
                        TopicTypeId = model.TopicTypeId,
                        Title = model.Title,
                        Content = model.Content
                    };

                    //图片上传
                    if (!string.IsNullOrEmpty(model.PicList))
                    {
                        //话题文件资源保存目录
                        string dir = HttpContext.Current.Server.MapPath(ConstantParam.Topic_Pictures_DIR + model.PropertyPlaceId);

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.PicList);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //图片集路径保存
                        postTopic.ImgPath = PropertyUtils.UnZip(filepath, dir, ConstantParam.Topic_Pictures_DIR + model.PropertyPlaceId);

                        StringBuilder imgsSB = new StringBuilder();
                        //生成缩略图保存
                        foreach (var path in postTopic.ImgPath.Split(';'))
                        {
                            string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
                            string thumpPath = Path.Combine(HttpContext.Current.Server.MapPath(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId), thumpFile);
                            PropertyUtils.getThumImage(Path.Combine(HttpContext.Current.Server.MapPath(path)), 18, 3, thumpPath);
                            imgsSB.Append(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId + "/" + thumpFile + ";");
                        }

                        postTopic.ImgThumbnail = imgsSB.ToString();
                        postTopic.ImgThumbnail = postTopic.ImgThumbnail.Substring(0, postTopic.ImgThumbnail.Length - 1);
                    }

                    //保存话题
                    IPostBarTopicBLL postTopicBLL = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                    postTopicBLL.Save(postTopic);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 话题圈-回贴
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel ReplyTopic(ReplyTopicModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    var replyTopic = new T_PostBarTopicDiscuss()
                    {
                        Content = model.Content,
                        ParentId = model.ParentId,
                        ReplyId = model.ReplyId,
                        PostUserId = model.UserId,
                        PostTime = DateTime.Now,
                        TopicId = model.Topicid
                    };

                    //图片上传
                    if (!string.IsNullOrEmpty(model.PicList))
                    {
                        //话题文件资源保存目录
                        string dir = HttpContext.Current.Server.MapPath(ConstantParam.Topic_Pictures_DIR + model.PropertyPlaceId);

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.PicList);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //图片集路径保存
                        replyTopic.ImgPath = PropertyUtils.UnZip(filepath, dir, ConstantParam.Topic_Pictures_DIR + model.PropertyPlaceId);

                        StringBuilder imgsSB = new StringBuilder();
                        //生成缩略图保存
                        foreach (var path in replyTopic.ImgPath.Split(';'))
                        {

                            string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
                            string thumpPath = Path.Combine(HttpContext.Current.Server.MapPath(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId), thumpFile);
                            PropertyUtils.getThumImage(Path.Combine(HttpContext.Current.Server.MapPath(path)), 18, 3, thumpPath);
                            imgsSB.Append(ConstantParam.Topic_ThumPictures_DIR + model.PropertyPlaceId + "/" + thumpFile + ";");
                        }

                        replyTopic.ImgThumbnail = imgsSB.ToString();
                        replyTopic.ImgThumbnail = replyTopic.ImgThumbnail.Substring(0, replyTopic.ImgThumbnail.Length - 1);
                    }

                    //保存话题回复
                    IPostBarTopicDiscussBLL replyTopicBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
                    replyTopicBLL.Save(replyTopic);

                    if (model.ReplyId != model.UserId)
                    {
                        //发推送给回复人
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var userPush = userPushBLL.GetEntity(u => u.UserId == model.ReplyId);

                        if (userPush != null)
                        {
                            var title = "你有一条话题圈的回复";
                            var content = replyTopic.Content;

                            Property.Common.PropertyUtils.SendPush(title, content, ConstantParam.MOBILE_TYPE_OWNER, userPush.RegistrationId);
                        }
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 话题圈-收藏主题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel CollectTopic(CollectTopicModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    //如果没有收藏，则收藏主题
                    var check = user.UserPostBarTopics.FirstOrDefault(m => m.PostBarTopicId == model.TopicId && m.UserId == model.UserId);

                    if (check == null)
                    {
                        var userPostBarTopic = new R_UserPostBarTopic()
                        {
                            PostBarTopicId = model.TopicId,
                            UserId = model.UserId
                        };

                        user.UserPostBarTopics.Add(userPostBarTopic);
                    }

                    userBll.Update(user);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 取消收藏过的主题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel CancelCollectedTopic(CollectTopicModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    userBll.Update(user);
                    //如果有收藏主题，取消收藏
                    var userPostBarTopic = user.UserPostBarTopics.FirstOrDefault(m => m.PostBarTopicId == model.TopicId && m.UserId == model.UserId);

                    if (userPostBarTopic != null)
                    {
                        userBll.ExecuteSql(string.Format("delete from R_UserPostBarTopic where userid={0} and postbartopicid={1}", model.UserId, model.TopicId));
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取我的话题圈-我的主题列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetMyTopicList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    var collectedTopicIds = owner.UserPostBarTopics.Select(o => o.PostBarTopicId).ToList();

                    IPostBarTopicBLL topicBLL = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                    // 获取我的主题列表
                    var list = topicBLL.GetSetTopPageList(m => m.PostUserId == owner.Id, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        PostUserId = m.PostUserId,
                        PostDate = m.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsTop = m.IsTop,
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Title = m.Title,
                        Content = m.Content,
                        PicList = m.ImgPath,
                        TopicType = m.PostBarTopicType.Name,
                        CommentCount = m.PostBarTopicDiscusss.Count(),
                        IsCollected = collectedTopicIds.Contains(m.Id) ? 1 : 0,
                        PropertyPlaceId = m.PropertyPlaceId,
                        PlaceName = m.PropertyPlace.Name
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicBLL.Count(m => m.PostUserId == owner.Id);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取我的话题圈-我的主题回复详细列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetMyTopicReplyList([FromUri]TopicDetailsModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                    var level1Ids = topicDiscussBLL.GetList(m => m.TopicId == model.TopicId && m.ParentId == null).OrderBy(m => m.PostTime).Select(m => m.Id).ToList();

                    // 获取我的帖子回复列表
                    var list = topicDiscussBLL.GetPageList(m => m.TopicId == model.TopicId && m.ParentId == null, "PostTime", true, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        UserId = m.PostUserId,
                        PostDate = m.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Content = m.Content,
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
                                Level2UserName = o.PostUser.UserName,
                                Level2Content = o.Content,
                                Level2PostDate = o.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                Level2RepliedUserName = o.ReplyUser.UserName,
                                Level2PicList = o.ImgPath
                            }).OrderBy(o => o.Level2PostDate).Take(2).ToList()
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicDiscussBLL.Count(m => m.TopicId == model.TopicId && m.ParentId == null);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取我的话题圈-我的回复列表（回复别人和别人回复我的）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetMyRepliesList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);


                    IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
                    // 获取我的回复列表
                    var list = topicDiscussBLL.GetPageList(m => m.ReplyId == owner.Id || m.PostUserId == owner.Id, "PostTime", false, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        PostUserId = m.PostUserId,
                        PostDate = m.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Title = m.PostBarTopic.Title,
                        Content = m.Content,
                        PicList = m.ImgPath,
                        TopicType = m.PostBarTopic.PostBarTopicType.Name,
                        CommentCount = m.PostBarTopic.PostBarTopicDiscusss.Count(),
                        PostTopicUserImage = m.PostBarTopic.PostUser.HeadPath,
                        PostTopicUserName = m.PostBarTopic.PostUser.UserName,
                        PostTopicDate = m.PostBarTopic.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsTop = m.PostBarTopic.IsTop,
                        TopicId = m.TopicId,
                        PropertyPlaceId = m.PostBarTopic.PropertyPlaceId,
                        PlaceName = m.PostBarTopic.PropertyPlace.Name
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicDiscussBLL.Count(m => m.ReplyId == owner.Id || m.PostUserId == owner.Id);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取我的话题圈-回复的一二级列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetMyReplyOneTwoLevelsList([FromUri]ReplyOneTwoLevelModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    var collectedTopicIds = owner.UserPostBarTopics.Select(o => o.PostBarTopicId).ToList();

                    IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                    //获取一级
                    var topicDiscuss = topicDiscussBLL.GetEntity(m => m.Id == model.Id);
                    int topicId = topicDiscuss.TopicId;
                    int level1Id = model.Id;

                    var level1Ids = topicDiscussBLL.GetList(m => m.TopicId == topicId && m.ParentId == null).OrderBy(m => m.PostTime).Select(m => m.Id).ToList();

                    if (topicDiscuss.ParentId.HasValue)
                    {
                        level1Id = topicDiscussBLL.GetEntity(m => m.TopicId == topicId && m.ParentId == null && m.Id == topicDiscuss.ParentId.Value).Id;
                        topicDiscuss = topicDiscussBLL.GetEntity(m => m.Id == level1Id);
                    }

                    int FloorNo = level1Ids.FindIndex(m => m == level1Id) + 1;

                    //根据一级获取二级
                    var list = new
                    {
                        Id = topicDiscuss.Id,
                        PostUserId = topicDiscuss.PostUserId,
                        PostDate = topicDiscuss.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserImage = topicDiscuss.PostUser.HeadPath,
                        UserName = topicDiscuss.PostUser.UserName,
                        Content = topicDiscuss.Content,
                        PicList = topicDiscuss.ImgPath,
                        TopicId = topicDiscuss.PostBarTopic.Id,
                        Title = topicDiscuss.PostBarTopic.Title,
                        TopicContent = topicDiscuss.PostBarTopic.Content,
                        TopicUserId = topicDiscuss.PostBarTopic.PostUserId,
                        TopicUserName = topicDiscuss.PostBarTopic.PostUser.UserName,
                        TopicUserImage = topicDiscuss.PostBarTopic.PostUser.HeadPath,
                        TopicPicList = topicDiscuss.PostBarTopic.ImgPath,
                        TopicPostDate = topicDiscuss.PostBarTopic.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Level2Count = topicDiscuss.PostBarTopicDiscusses.Count,
                        FloorNo = FloorNo,
                        TopicIsTop = topicDiscuss.PostBarTopic.IsTop,
                        TopicIsCollected = collectedTopicIds.Contains(topicDiscuss.TopicId) ? 1 : 0,
                        PropertyPlaceId = topicDiscuss.PostBarTopic.PropertyPlaceId,
                        Level2List = topicDiscuss.PostBarTopicDiscusses.Select(o =>
                            new
                            {
                                Level2ParentId = o.ParentId,
                                Level2PostUserId = o.PostUserId,
                                Level2UserName = o.PostUser.UserName,
                                Level2Content = o.Content,
                                Level2PostDate = o.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                Level2RepliedUserName = o.ReplyUser.UserName,
                                Level2PicList = o.ImgPath
                            }).OrderByDescending(o => o.Level2PostDate).Take(2).ToList()
                    };

                    resultModel.result = list;
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 话题圈--根据一级得到下面所有二级回复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetMyLevel2RepliesList([FromUri]Level2RepliedListModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);


                    IPostBarTopicDiscussBLL topicDiscussBLL = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");

                    var level1Disscuss = topicDiscussBLL.GetEntity(m => m.Id == model.Id);
                    // 获取我的二级回复列表
                    var list = topicDiscussBLL.GetPageList(m => m.ParentId == model.Id && m.TopicId == model.TopicId,"PostTime",true, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        PostDate = m.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserId = m.PostUserId,
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Content = m.Content,
                        PicList = m.ImgPath,
                        RepliedUserName = m.ReplyUser.UserName,
                        Level2ParentId = m.ParentId
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicDiscussBLL.Count(m => m.ParentId == model.Id && m.TopicId == model.TopicId);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取我的话题圈-我的收藏列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetMyTopicCollectionsList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    var collectedTopicIds = owner.UserPostBarTopics.Select(o => o.PostBarTopicId).ToList();

                    // 获取我的收藏列表
                    var list = owner.UserPostBarTopics.Select(m => new
                    {
                        Id = m.PostBarTopicId,
                        PostUserId = m.PostBarTopic.PostUserId,
                        PostDate = m.PostBarTopic.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        UserImage = m.PostBarTopic.PostUser.HeadPath,
                        UserName = m.PostBarTopic.PostUser.UserName,
                        Title = m.PostBarTopic.Title,
                        Content = m.PostBarTopic.Content,
                        PicList = m.PostBarTopic.ImgPath,
                        TopicType = m.PostBarTopic.PostBarTopicType.Name,
                        CommentCount = m.PostBarTopic.PostBarTopicDiscusss.Count(),
                        PropertyPlaceId = m.PostBarTopic.PropertyPlaceId,
                        PlaceName = m.PostBarTopic.PropertyPlace.Name,
                        IsTop = m.PostBarTopic.IsTop,
                        IsCollected = collectedTopicIds.Contains(m.PostBarTopicId) ? 1 : 0
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = owner.UserPostBarTopics.Count();
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 获取话题圈-全部帖子列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetAllTopicList([FromUri]AllTopicPagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    var collectedTopicIds = owner.UserPostBarTopics.Select(o => o.PostBarTopicId).ToList();

                    IPostBarTopicBLL topicBLL = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                    // 获取小区所有主题列表
                    var list = topicBLL.GetSetTopPageList(m => m.PropertyPlaceId == model.PropertyPlaceId, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        PostUserId = m.PostUserId,
                        PostDate = m.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsTop = m.IsTop,
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Title = m.Title,
                        Content = m.Content,
                        PicList = m.ImgPath,
                        TopicType = m.PostBarTopicType.Name,
                        CommentCount = m.PostBarTopicDiscusss.Count(),
                        IsCollected = collectedTopicIds.Contains(m.Id) ? 1 : 0
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicBLL.Count(m => m.PropertyPlaceId == model.PropertyPlaceId);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }


        /// <summary>
        /// 获取话题圈-帖子分类列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetTopicSortList([FromUri]TopicSortSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != model.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取小区所有主题分类列表
                    IPostBarTopicTypeBLL postBarTopicTypeBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
                    Expression<Func<T_PostBarTopicType, bool>> where = u => u.PropertyPlaceId == model.PropertyPlaceId;

                    var list = postBarTopicTypeBll.GetList(where, "Id", false).Select(s => new
                        {
                            ID = s.Id,
                            Name = s.Name
                        }).ToList();

                    resultModel.result = list;
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取话题圈-某分类下的帖子列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetAllTopicListByType([FromUri]AllTopicPagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    var collectedTopicIds = owner.UserPostBarTopics.Select(o => o.PostBarTopicId).ToList();

                    IPostBarTopicBLL topicBLL = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                    // 获取小区某分类下的所有主题列表
                    var list = topicBLL.GetSetTopPageList(m => m.PropertyPlaceId == model.PropertyPlaceId && m.TopicTypeId == model.TopicTypeId, model.PageIndex, ConstantParam.PAGE_SIZE).Select(m => new
                    {
                        Id = m.Id,
                        PostDate = m.PostDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsTop = m.IsTop,
                        UserImage = m.PostUser.HeadPath,
                        UserName = m.PostUser.UserName,
                        Title = m.Title,
                        Content = m.Content,
                        PicList = m.ImgPath,
                        CommentCount = m.PostBarTopicDiscusss.Count(),
                        IsCollected = collectedTopicIds.Contains(m.Id) ? 1 : 0,
                        TopicType = m.PostBarTopicType.Name,
                        PostUserId = m.PostUserId
                    }).ToList();

                    resultModel.result = list;
                    resultModel.Total = topicBLL.Count(m => m.PropertyPlaceId == model.PropertyPlaceId && m.TopicTypeId == model.TopicTypeId);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 话题圈-删除主题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteTopic(DeleteTopicModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    userBll.Update(user);

                    //获取要删除的帖子
                    IPostBarTopicBLL postBarTopicBll = BLLFactory<IPostBarTopicBLL>.GetBLL("PostBarTopicBLL");
                    T_PostBarTopic topic = postBarTopicBll.GetEntity(u => u.Id == model.TopicId);

                    if (topic != null)
                    {
                        postBarTopicBll.DeleteTopic(model.TopicId);
                    }
                    else
                    {
                        resultModel.Msg = "该主题不存在";
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }

        /// <summary>
        /// 话题圈-删除回复
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteReply(DeleteReplyModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户ID查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    userBll.Update(user);

                    //获取要删除的回复内容
                    IPostBarTopicDiscussBLL postBarTopicDiscussBll = BLLFactory<IPostBarTopicDiscussBLL>.GetBLL("PostBarTopicDiscussBLL");
                    T_PostBarTopicDiscuss reply = postBarTopicDiscussBll.GetEntity(u => u.Id == model.Id);

                    //如果该回复存在
                    if (reply == null)
                    {
                        resultModel.Msg = "该回复不存在";
                    }
                    else
                    {
                        if (reply.ParentId == null)
                        {
                            postBarTopicDiscussBll.DeleteLevelOneDiscuss(reply.Id);
                        }
                        else
                        {
                            postBarTopicDiscussBll.Delete(reply);
                        }
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }

            return resultModel;
        }
    }
}