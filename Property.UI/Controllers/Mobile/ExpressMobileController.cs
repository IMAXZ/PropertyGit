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
    /// 快递公司接口控制类
    /// </summary>
    public class ExpressMobileController : ApiController
    {
        /// <summary>
        /// 获取快递公司列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetExpressCompany([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                ///查询用户Id查找业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(m => m.Id == model.UserId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主不存在
                if (user != null)
                {
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }

                    //更新最近的登录时间和最新的token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    IExpressCompanyBLL expressCompanyBLL = BLLFactory<IExpressCompanyBLL>.GetBLL("ExpressCompanyBLL");
                    Expression<Func<T_ExpressCompany, bool>> where = o => o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

                    var list = expressCompanyBLL.GetList(where, "Id", true).ToList();

                    resultModel.result = list.Select(u => new
                        {
                            Id = u.Id,
                            Img = u.Img,
                            Phone = u.Phone
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