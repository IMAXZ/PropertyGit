using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 生活支付方式相关的API接口控制器
    /// </summary>
    public class LifePayMobileController : ApiController
    {
        /// <summary>
        /// 获取生活支付方式列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetLifePayType([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据业主Id查询业主
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

                    //获取所有生活支付方式列表
                    ILifePayTypeBLL lifePayTypeBll = BLLFactory<ILifePayTypeBLL>.GetBLL("LifePayTypeBLL");
                    Expression<Func<T_LifePayType, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

                    var list = lifePayTypeBll.GetList(where, "Id", true).ToList();
                    resultModel.result = list.Select(u => new
                        {
                            Id = u.Id,
                            Img = u.Img,
                            Name = u.Name
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