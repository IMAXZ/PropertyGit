using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 新闻公告移动端接口控制器类
    /// </summary>
    public class NewsMobileController : ApiController
    {
        /// <summary>
        /// 业主获取关联小区的所有公告的列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel NewsList([FromUri] PagedSearchModel model)
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

                    // 获取指定物业小区id的公告列表
                    IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
                    var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);
                    Expression<Func<T_Post, bool>> where = u => placeList.Contains(u.PropertyPlaceId) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PublishedFlag == ConstantParam.PUBLISHED_TRUE;
                    // TODO:此处Content不需要全部返回，待优化
                    var list = postBll.GetPageList(where, "PublishedTime", false, model.PageIndex).Select(s => new
                    {
                        ID = s.Id,
                        propertyName = s.PropertyPlace.Name,
                        propertyPic = string.IsNullOrEmpty(s.PropertyPlace.ImgThumbnail) ? "/Images/news_item_default.png" : s.PropertyPlace.ImgThumbnail,
                        pubDate = s.PublishedTime.ToString(),
                        title = s.Title
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = postBll.GetList(where).Count();
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
        /// 业主获取本小区指定公告详细接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel NewsDetail([FromUri]NewsDetailModel model)
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

                    IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
                    var post = postBll.GetEntity(p => p.Id == model.PostId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PublishedFlag == 1);
                    if (post != null)
                    {
                        // 返回详细页面url
                        resultModel.result = new
                        {
                            PlaceTel = post.PropertyPlace.Tel,
                            Url = "MobilePage/NewsDetail?id=" + model.PostId
                        };
                    }
                    else
                    {
                        resultModel.Msg = "新闻公告不存在";
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
        /// 物业人员获取本小区的公告列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel PropertyNewsList([FromUri] PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                //根据用户ID查找物业用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果物业用户存在
                if (propertyUser != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > propertyUser.TokenInvalidTime || model.Token != propertyUser.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    propertyUser.LatelyLoginTime = DateTime.Now;
                    propertyUser.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    propertyUserBll.Update(propertyUser);

                    // 获取指定物业小区id的公告列表
                    IPostBLL postBll = BLLFactory<IPostBLL>.GetBLL("PostBLL");
                    Expression<Func<T_Post, bool>> where = u => u.PropertyPlaceId == propertyUser.PropertyPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PublishedFlag == ConstantParam.PUBLISHED_TRUE;
                    // TODO:此处Content不需要全部返回，待优化
                    var list = postBll.GetPageList(where, "PublishedTime", false, model.PageIndex).Select(s => new
                    {
                        ID = s.Id,
                        propertyName = s.PropertyPlace.Name,
                        propertyPic = string.IsNullOrEmpty(s.PropertyPlace.ImgThumbnail) ? "/Images/news_item_default.png" : s.PropertyPlace.ImgThumbnail,
                        pubDate = s.PublishedTime.ToString(),
                        title = s.Title,
                        content = s.Content.Replace("\n", "")
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = postBll.GetList(where).Count();
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
        /// 物业获取本小区指定公告详细接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel PropertyNewsDetail([FromUri]NewsDetailModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户ID查找物业用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果物业用户存在
                if (propertyUser != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > propertyUser.TokenInvalidTime || model.Token != propertyUser.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    propertyUser.LatelyLoginTime = DateTime.Now;
                    propertyUser.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    propertyUserBll.Update(propertyUser);

                    // 返回详细页面url
                    resultModel.result = "MobilePage/NewsDetail?id=" + model.PostId;
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
        /// 物业人员获取总公司公告列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel CompanyNewsList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                //根据用户ID查找物业用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果物业用户存在
                if (propertyUser != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > propertyUser.TokenInvalidTime || model.Token != propertyUser.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    propertyUser.LatelyLoginTime = DateTime.Now;
                    propertyUser.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    propertyUserBll.Update(propertyUser);

                    //获取指定公司发布的公开新闻公告
                    ICompanyPostBLL postBll = BLLFactory<ICompanyPostBLL>.GetBLL("CompanyPostBLL");
                    Expression<Func<T_CompanyPost, bool>> where = u => u.CompanyId == propertyUser.PropertyPlace.CompanyId
                        && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PublishStatus == ConstantParam.PUBLISHED_TRUE && u.IsOpen == ConstantParam.PUBLISHED_TRUE;
                    var list = postBll.GetPageList(where, "PublishedTime", false, model.PageIndex).Select(s => new
                    {
                        ID = s.Id,
                        CompanyName = s.Company.Name,
                        CompanyIcon = string.IsNullOrEmpty(s.Company.Img) ? "/Images/news_item_default.png" : s.Company.Img,
                        PublishedTime = s.PublishedTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Title = s.Title
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = postBll.Count(where);
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
        /// 物业获取所属公司公告详细接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel CompanyNewsDetail([FromUri]NewsDetailModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户ID查找物业用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = propertyUserBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果物业用户存在
                if (propertyUser != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > propertyUser.TokenInvalidTime || model.Token != propertyUser.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    propertyUser.LatelyLoginTime = DateTime.Now;
                    propertyUser.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    propertyUserBll.Update(propertyUser);

                    // 返回详细页面url
                    resultModel.result = "MobilePage/CompanyNewsDetail?id=" + model.PostId;
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
