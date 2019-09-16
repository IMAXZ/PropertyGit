using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 异常处理控制器
    /// </summary>
    public class DisposeMobileController : ApiController
    {
        /// <summary>
        /// 获取要指派处理人的问题
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel QuestionList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    Expression<Func<T_Question, bool>> where = u => u.PropertyPlaceId == user.PropertyPlaceId && u.Status == ConstantParam.NO_DISPOSE;
                    
                    //获取问题总个数和分页数据
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    resultModel.Total = questionBll.Count(where);

                    resultModel.result = new
                    {
                        IsHaveAssignAction = IsHaveAction(user, "/Question/SetQuestionDisposer"),
                        questionList = questionBll.GetQuestionPageList(where, model.PageIndex, ConstantParam.PAGE_SIZE).Select(q => new
                        {
                            Id = q.Id,
                            Title = q.Title,
                            Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                            Status = q.Status,
                            UploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Uploader = q.UploadUser.UserName,
                            Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[] { } : q.Imgs.Split(';'),
                            AudioPath = q.AudioPath,
                            VoiceDuration = q.VoiceDuration,
                            DisposeDesc = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeDesc : "",
                            DisposeTime = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            DisposeUser = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeUser.UserName : (q.DisposerId != null ? q.Disposer.UserName : ""),
                            IsPublish = q.IsPublish
                        })
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
        /// 获取处理人为自己的问题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel OwnQuestionList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

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

                    Expression<Func<T_Question, bool>> where = u => u.PropertyPlaceId == user.PropertyPlaceId && u.DisposerId == user.Id;

                    //获取问题总个数和分页数据
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    resultModel.Total = questionBll.Count(where);

                    resultModel.result = new
                    {
                        ownQuestionList = questionBll.GetOwnQuestionPageList(where, model.PageIndex, ConstantParam.PAGE_SIZE).Select(q => new
                        {
                            Id = q.Id,
                            Title = q.Title,
                            Desc = string.IsNullOrEmpty(q.Desc) ? "" : q.Desc,
                            Status = q.Status,
                            UploadTime = q.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Uploader = q.UploadUser.UserName,
                            Imgs = string.IsNullOrEmpty(q.Imgs) ? new string[] { } : q.Imgs.Split(';'),
                            AudioPath = q.AudioPath,
                            VoiceDuration = q.VoiceDuration,
                            DisposeDesc = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeDesc : "",
                            DisposeTime = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            DisposeUser = q.Status == ConstantParam.DISPOSED ? q.QuestionDisposes.FirstOrDefault().DisposeUser.UserName : (q.DisposerId != null ? q.Disposer.UserName : ""),
                            IsPublish = q.IsPublish
                        })
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
        /// 获取要指派处理人的巡检异常
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel InspectionExceptionList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    Expression<Func<T_InspectionResult, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                        && u.InspectionTimePlan.InspectionPlan.PropertyPlaceId == user.PropertyPlaceId && u.Status == ConstantParam.EXCEPTION && u.DisposeStatus == ConstantParam.NO_DISPOSE;
                    
                    //获取巡检异常总个数和分页数据
                    IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                    resultModel.Total = resultBll.Count(where);
                    resultModel.result = new
                    {
                        IsHaveAssignAction = IsHaveAction(user, "/Inspection/SetDisposer"),
                        ExceptionList = resultBll.GetInspectionResultPageList(where, model.PageIndex, ConstantParam.PAGE_SIZE).Select(r => new
                       {
                           Id = r.Id,
                           PlanName = r.InspectionTimePlan.InspectionPlan.PlanName,
                           PointName = r.InspectionPoint.PointName,
                           Desc = string.IsNullOrEmpty(r.Desc) ? "" : r.Desc,
                           DisposeStatus = r.DisposeStatus == null ? ConstantParam.NO_DISPOSE : r.DisposeStatus.Value,
                           UploadTime = r.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                           Uploader = r.UploadUser.UserName,
                           Imgs = string.IsNullOrEmpty(r.Imgs) ? new string[] { } : r.Imgs.Split(';'),
                           DisposeDesc = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeDesc : "",
                           DisposeTime = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                           DisposeUser = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeUser.UserName : (r.DisposerId != null ? r.Disposer.UserName : "")
                       })
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
        /// 获取处理人为自己的巡检异常
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel OwnInspectionExceptionList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();

            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (user != null)
                {
                    //验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    Expression<Func<T_InspectionResult, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.InspectionTimePlan.InspectionPlan.PropertyPlaceId == user.PropertyPlaceId && u.Status == ConstantParam.EXCEPTION && u.DisposerId == user.Id;

                    //获取巡检异常总个数和分页数据
                    IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                    resultModel.Total = resultBll.Count(where);
                    resultModel.result = new
                    {
                        ExceptionList = resultBll.GetOwnInspectionResultPageList(where, model.PageIndex, ConstantParam.PAGE_SIZE).Select(r => new
                        {
                            Id = r.Id,
                            PlanName = r.InspectionTimePlan.InspectionPlan.PlanName,
                            PointName = r.InspectionPoint.PointName,
                            Desc = string.IsNullOrEmpty(r.Desc) ? "" : r.Desc,
                            DisposeStatus = r.DisposeStatus == null ? ConstantParam.NO_DISPOSE : r.DisposeStatus.Value,
                            UploadTime = r.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Uploader = r.UploadUser.UserName,
                            Imgs = string.IsNullOrEmpty(r.Imgs) ? new string[] { } : r.Imgs.Split(';'),
                            DisposeDesc = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeDesc : "",
                            DisposeTime = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            DisposeUser = r.DisposeStatus == ConstantParam.DISPOSED ? r.ExceptionDisposes.FirstOrDefault().DisposeUser.UserName : (r.DisposerId != null ? r.Disposer.UserName : "")
                        })
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
        /// 获取物业用户(处理人)列表
        /// </summary>
        /// <param name="model">Token模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetDisposerList([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    //获取物业用户列表
                    Expression<Func<T_PropertyUser, bool>> where = u => u.PropertyPlaceId == user.PropertyPlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
                    var userList = userBll.GetList(where, "Id", false).ToList();

                    resultModel.result = userList.Select(u => new
                    {
                        UserId = u.Id,
                        Name = string.IsNullOrEmpty(u.TrueName) ? u.UserName : (userList.Count(p => p.TrueName == u.TrueName) > 1 ? u.TrueName + "(" + u.UserName + ")" : u.TrueName)
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
        /// 用户是否拥有指定权限
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="href">权限对应的Href地址</param>
        /// <returns></returns>
        private bool IsHaveAction(T_PropertyUser user, string href)
        {
            if (user.IsMgr == ConstantParam.USER_ROLE_MGR)
            {
                return true;
            }
            var roleActions = user.PropertyUserRoles.Select(ur => ur.PropertyRole.PropertyRoleActions);
            foreach (var roleAction in roleActions)
            {
                var actions = roleAction.Select(obj => obj.Action);
                foreach (var action in actions)
                {
                    if (action.Href == href)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 上报问题指派处理人
        /// </summary>
        /// <param name="model">指派处理人模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetQuestionDisposer(SetDisposerModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    //获取要指派处理人的问题
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    T_Question question = questionBll.GetEntity(m => m.Id == model.Id);
                    if (question != null)
                    {
                        //指派处理人
                        question.DisposerId = model.DisposerId;
                        //保存到数据库
                        if (!questionBll.Update(question))
                        {
                            resultModel.Msg = APIMessage.SET_DISPOSER_FAIL;
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.QUESTION_NOEXIST;
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
        /// 巡检异常指派处理人
        /// </summary>
        /// <param name="model">指派处理人模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetExceptionDisposer(SetDisposerModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    //获取要指派处理人的巡检异常
                    IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                    T_InspectionResult result = resultBll.GetEntity(m => m.Id == model.Id);
                    if (result != null)
                    {
                        //指派处理人
                        result.DisposerId = model.DisposerId;
                        //保存到数据库
                        if (!resultBll.Update(result))
                        {
                            resultModel.Msg = APIMessage.SET_DISPOSER_FAIL;
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.EXCEPTION_NOEXIST;
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
        /// 处理上报问题
        /// </summary>
        /// <param name="model">问题处理模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DisposeQuestion(DisposerModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    //获取要处理的问题
                    IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                    T_Question question = questionBll.GetEntity(m => m.Id == model.Id);
                    if (question != null)
                    {
                        //修改处理状态并添加处理记录
                        question.Status = ConstantParam.DISPOSED;
                        question.IsPublish = model.IsPublish;
                        T_QuestionDispose questionDispose = new T_QuestionDispose()
                        {
                            DisposeDesc = model.DisposeDesc,
                            DisposeUserId = user.Id,
                            QuestionId = model.Id,
                            DisposeTime = DateTime.Now
                        };
                        //保存到数据库
                        questionBll.DisposeQuestion(question, questionDispose);

                        //推送通知
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var userPush = userPushBLL.GetEntity(p => p.UserId == question.UploadUserId);
                        if (userPush != null)
                        {
                            string registrationId = userPush.RegistrationId;
                            string alert = "您" + question.UploadTime.ToString("yyyy-MM-dd HH:mm") + "上报的问题已处理";
                            //通知信息
                            bool flag = PropertyUtils.SendPush("上报问题处理", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                            if (!flag)
                            {
                                resultModel.Msg = "问题处理完成，推送失败";
                            }
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.QUESTION_NOEXIST;
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
        /// 处理巡检异常
        /// </summary>
        /// <param name="model">指派处理人模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DisposeException(DisposerModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
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

                    //获取要处理的巡检异常
                    IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                    T_InspectionResult result = resultBll.GetEntity(m => m.Id == model.Id);
                    if (result != null)
                    {
                        //修改处理状态并添加处理记录
                        result.DisposeStatus = ConstantParam.DISPOSED;
                        T_InspectionExceptionDispose exceptionDispose = new T_InspectionExceptionDispose()
                        {
                            DisposeDesc = model.DisposeDesc,
                            DisposeUserId = user.Id,
                            ExceptionResultId = model.Id,
                            DisposeTime = DateTime.Now
                        };
                        //保存到数据库
                        resultBll.DisposeException(result, exceptionDispose);
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.EXCEPTION_NOEXIST;
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
