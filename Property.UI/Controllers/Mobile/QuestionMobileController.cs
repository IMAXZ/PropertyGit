using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;
namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 业主上报问题 手机端接口控制器
    /// </summary>
    public class QuestionMobileController : ApiController
    {
        /// <summary>
        /// 提报的问题列表
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel QuestionList([FromUri]PagedSearchModel model)
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

                    //获取问题总个数和分页数据
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    Expression<Func<T_Question, bool>> where = u => u.UploadUserId == model.UserId;
                    resultModel.Total = questionBll.Count(where);
                    resultModel.result = questionBll.GetPageList(where, "Id", false, model.PageIndex).ToList().Select(q => new
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                        Status = q.Status,
                        UploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[]{}:q.Imgs.Split(';'),
                        AudioPath = q.AudioPath,
                        VoiceDuration = q.VoiceDuration,
                        PlaceName = q.PropertyPlace.Name,
                        DisposesTime = q.Status == ConstantParam.NO_DISPOSE ? null:q.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
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
        /// 问题详细接口
        /// </summary>
        /// <param name="model">详细接口模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel QuestionDetail([FromUri]DetailSearchModel model)
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

                    //获取问题总个数和分页数据
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    var question = questionBll.GetEntity(u => u.Id == model.Id);
                    resultModel.result = new
                    {
                        Id = question.Id,
                        Title = question.Title,
                        Desc = string.IsNullOrEmpty(question.Desc) ? "" : question.Desc,
                        Status = question.Status,
                        UploadTime = question.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Imgs = string.IsNullOrEmpty(question.Imgs) ? new string[]{} : question.Imgs.Split(';'),
                        AudioPath = question.AudioPath,
                        VoiceDuration = question.VoiceDuration,
                        PlaceName = question.PropertyPlace.Name,
                        DisposesTime = question.Status == ConstantParam.NO_DISPOSE ? null : question.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss")
                    };
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
        /// 问题上报
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddQuestion(QuestionModel model)
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

                    //问题基本信息上传
                    T_Question question = new T_Question()
                    {
                        Title = model.Title,
                        Desc = model.Desc,
                        PropertyPlaceId = model.PropertyPlaceId,
                        UploadTime = DateTime.Now,
                        UploadUserId = model.UserId,
                        Status = ConstantParam.NO_DISPOSE,
                        ClientSaveTime = DateTime.Now
                    };

                    //问题上报文件资源保存目录
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.QUESTION_FILE + model.PropertyPlaceId);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    //问题图片上传
                    if (!string.IsNullOrEmpty(model.PicList))
                    {
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
                        question.Imgs = PropertyUtils.UnZip(filepath, dir, ConstantParam.QUESTION_FILE + model.PropertyPlaceId);
                    }

                    //语音文件上传
                    if (!string.IsNullOrEmpty(model.VoiceFile))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".amr";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.VoiceFile);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //语音路径保存
                        question.AudioPath = ConstantParam.QUESTION_FILE + model.PropertyPlaceId + "/" + fileName;
                        question.VoiceDuration = model.VoiceDuration;
                    }

                    //保存提报的问题
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    questionBll.Save(question);
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
        /// 上报问题解决反馈 公示列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel DisposeFeedbackList([FromUri]PagedSearchModel model) 
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

                    //获取要公示的问题解决反馈 总个数和分页数据
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    //条件已处理和要公示
                    var placeList = owner.UserPlaces.Select(m => m.PropertyPlaceId);
                    Expression<Func<T_Question, bool>> where = u => u.Status == ConstantParam.DISPOSED && u.IsPublish == ConstantParam.PUBLISHED_TRUE 
                        && placeList.Contains(u.PropertyPlaceId);
                    resultModel.Total = questionBll.Count(where);
                    resultModel.result = questionBll.GetPageList(where, "Id", false, model.PageIndex).ToList().Select(q => new
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                        UploadUserName = q.UploadUser.UserName,
                        UploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[] { } : q.Imgs.Split(';'),
                        AudioPath = q.AudioPath,
                        VoiceDuration = q.VoiceDuration,
                        PlaceName = q.PropertyPlace.Name,
                        DisposeDesc = q.QuestionDisposes.FirstOrDefault().DisposeDesc,
                        DisposeUserName = string.IsNullOrEmpty(q.QuestionDisposes.FirstOrDefault().DisposeUser.TrueName) ?
                            q.QuestionDisposes.FirstOrDefault().DisposeUser.UserName : q.QuestionDisposes.FirstOrDefault().DisposeUser.TrueName,
                        DisposesTime = q.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
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
