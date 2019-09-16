using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 巡检相关客户端接口
    /// </summary>
    public class InspectionMobileController : ApiController
    {
        /// <summary>
        /// 巡检任务列表
        /// </summary>
        /// <param name="model">Token模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel InspectionPlans([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
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

                    //获取包含当日的巡检时间安排
                    IInspectionTimePlanBLL planTimeBll = BLLFactory<IInspectionTimePlanBLL>.GetBLL("InspectionTimePlanBLL");

                    //查询条件
                    Expression<Func<T_InspectionTimePlan, bool>> where = p => p.InspectionPlan.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.InspectionPlan.PublishedFlag == ConstantParam.PUBLISHED_TRUE
                        && p.InspectionPlan.PropertyPlaceId == user.PropertyPlaceId && p.InspectionPlan.BeginDate <= DateTime.Today && p.InspectionPlan.EndDate >= DateTime.Today;

                    where = PredicateBuilder.And(where, p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY
                        || (p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK && p.BeginNum == (int)DateTime.Today.DayOfWeek)
                        || (p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH && p.BeginNum == DateTime.Today.Day)
                        || p.InspectionPlan.IsRandom == ConstantParam.DELIVERY_FLAG_TRUE);

                    //获取当前可以巡检的任务列表
                    var planTimeList = planTimeBll.GetList(where).ToList();
                    resultModel.result = planTimeList.Select(pt => new
                        {
                            PlanId = pt.PlanId,
                            PlanName = pt.InspectionPlan.PlanName,
                            Type = pt.InspectionPlan.Type,
                            IsRandom = pt.InspectionPlan.IsRandom,
                            Number = pt.Number,
                            PlanTimeId = pt.Id,
                            PlanDate = DateTime.Today.ToString("yyyy-MM-dd"),
                            TimeSpan = pt.InspectionPlan.IsRandom == ConstantParam.DELIVERY_FLAG_TRUE ? "随机" : (pt.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY ? pt.BeginNum + ":00 - " + pt.EndNum + ":00"
                            : "本日"),
                            PointList = pt.InspectionPlan.PlanPoints.Select(p => new
                            {
                                PointId = p.PointId,
                                PointName = p.InspectionPoint.PointName,
                                Memo = p.InspectionPoint.Memo,
                                IsInspectioned = IsInspectioned(pt, p.PointId)
                            })
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
        /// 判断是否已经巡检过
        /// </summary>
        /// <param name="PlanId">巡检时间安排ID</param>
        /// <param name="PointId">巡检点ID</param>
        /// <returns></returns>
        private bool IsInspectioned(T_InspectionTimePlan TimePlan, int PointId)
        {
            var BeginTime = DateTime.Today;
            var EndTime = DateTime.Today;

            if (TimePlan.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY)
            {
                EndTime = DateTime.Today.AddDays(1);
            }
            else if (TimePlan.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK)
            {
                BeginTime = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek);
                EndTime = BeginTime.AddDays(7);
            }
            else if (TimePlan.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH)
            {
                BeginTime = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                EndTime = BeginTime.AddMonths(1);
            }

            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            return resultBll.Exist(r => r.TimePlanId == TimePlan.Id && r.PointId == PointId && r.PlanDate >= BeginTime && r.PlanDate < EndTime);
        }


        /// <summary>
        /// 上传巡检结果
        /// </summary>
        /// <param name="model">巡检结果模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel UploadResult(InspectionResultModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
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

                    IInspectionTimePlanBLL planTimeBll = BLLFactory<IInspectionTimePlanBLL>.GetBLL("InspectionTimePlanBLL");
                    if (!planTimeBll.Exist(p => p.Id == model.TimePlanId))
                    {
                        resultModel.result = "该巡检安排已不存在";
                        return resultModel;
                    }
                    IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");

                    //单条巡检结果初始化
                    T_InspectionResult result = new T_InspectionResult()
                    {
                        TimePlanId = model.TimePlanId,
                        PointId = model.PointId,
                        Status = model.Status,
                        Desc = model.Desc,
                        UploadUserId = model.UserId,
                        DelFlag = ConstantParam.DEL_FLAG_DEFAULT,
                        Longitude = model.Longitude,
                        Latitude = model.Latitude

                    };
                    //如果状态为异常，则设置处理状态为未处理
                    if (model.Status == ConstantParam.EXCEPTION)
                    {
                        result.DisposeStatus = ConstantParam.NO_DISPOSE;
                    }
                    //巡检异常文件资源保存目录
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.QUESTION_FILE + user.PropertyPlaceId);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    //问题图片上传
                    if (!string.IsNullOrEmpty(model.ImgFiles))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.ImgFiles);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //图片集路径保存
                        result.Imgs = PropertyUtils.UnZip(filepath, dir, ConstantParam.QUESTION_FILE + user.PropertyPlaceId);
                    }

                    //语音文件上传
                    if (!string.IsNullOrEmpty(model.AudioFile))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".amr";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.AudioFile);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }
                        //语音路径保存
                        result.AudioPath = ConstantParam.QUESTION_FILE + user.PropertyPlaceId + "/" + fileName;
                    }

                    result.ClientSaveTime = model.ClientSaveTime;
                    result.PlanDate = model.PlanDate;
                    result.UploadTime = DateTime.Now;

                    //上传巡检结果
                    resultBll.Save(result);
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
