using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 意见反馈 手机端接口控制器
    /// </summary>
    public class FeedbackMobileController : ApiController
    {
        /// <summary>
        /// 意见反馈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddFeedback(FeedbackModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户Id查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主不存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.result = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //意见反馈信息上传
                    T_Feedback feedback = new T_Feedback()
                    {
                        UserId = model.UserId,
                        UploadTime = DateTime.Now,
                        Content = model.Content
                    };

                    //意见反馈 图片保存目录
                    string directory = HttpContext.Current.Server.MapPath(ConstantParam.FEEDBACK);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    //意见反馈 图片上传
                    if (!string.IsNullOrEmpty(model.PicList))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".zip";
                        string filepath = Path.Combine(directory, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.PicList);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }

                        //图片集保存路径
                        feedback.Img = PropertyUtils.UnZip(filepath, directory, ConstantParam.FEEDBACK);
                    }

                    //保存意见反馈
                    IFeedbackBLL feedbackBll = BLLFactory<IFeedbackBLL>.GetBLL("FeedbackBLL");
                    feedbackBll.Save(feedback);
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