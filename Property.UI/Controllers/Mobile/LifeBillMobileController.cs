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
    /// 生活记账 手机端接口控制器
    /// </summary>
    public class LifeBillMobileController : ApiController
    {
        /// <summary>
        /// 生活记账列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiPayResultModel LifeBillList([FromUri]PagedSearchModel model)
        {
            ApiPayResultModel resultModel = new ApiPayResultModel();

            try
            {
                //根据用户Id查找业主
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

                    //更新最近登录时间和Token过期时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
                    Expression<Func<T_LifeBill, bool>> where = u => u.UserId == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

                    resultModel.TotalPay = TotalPaySum(owner.Id);
                    resultModel.MonthPay = MonthPaySum(owner.Id);


                    //所有的生活记账
                    var AllList = lifeBillBll.GetList(where, "Id", false).ToList().Select(q => new
                        {
                            Time = q.ConsumptionDate,
                            AccountId = q.Id,
                            Img = q.LifeBillType.Img,
                            Title = q.LifeBillType.Name,
                            Money = q.Money,
                            Mark = q.Memo
                        });

                    //所有的记账时间
                    List<DateTime> Datelist = lifeBillBll.GetList(where, "ConsumptionDate", false).GroupBy(u => u.ConsumptionDate).Select(a => a.Key).ToList();

                    List<AllLifeBillList> alllist = new List<AllLifeBillList>();
                    foreach (var item in Datelist)
                    {
                        AllLifeBillList all = new AllLifeBillList();
                        List<LifeBillDescModel> list = new List<LifeBillDescModel>();

                        var lst = AllList.Where(a => a.Time == item).ToList();
                        all.Time = item.ToString("yyyy-MM-dd");
                        foreach (var i in lst)
                        {
                            LifeBillDescModel LifeBill = new LifeBillDescModel();
                            LifeBill.AccountId = i.AccountId;
                            LifeBill.Title = i.Title;
                            LifeBill.Img = i.Img;
                            LifeBill.Money = i.Money;
                            LifeBill.Mark = i.Mark;
                            list.Add(LifeBill);
                        }
                        all.AccountList = list;
                        alllist.Add(all);
                    }

                    resultModel.result = alllist.Skip((model.PageIndex - 1) * 5).Take(5).ToList();

                    resultModel.Total = alllist.Count();
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
        /// 总支出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static double TotalPaySum(int id)
        {
            ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
            Expression<Func<T_LifeBill, bool>> where = u => u.UserId == id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

            var list = lifeBillBll.GetList(where).ToList();
            double sum = list.Select(u => u.Money).ToArray().Sum();
            return sum;
        }

        /// <summary>
        /// 月支出
        /// </summary>
        /// <param name="id"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static double MonthPaySum(int id)
        {
            DateTime today = DateTime.Today;
            DateTime BeginTime = today.AddDays(-today.Day + 1);
            DateTime EndTime = today.AddDays(1);

            ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
            Expression<Func<T_LifeBill, bool>> where = u => u.UserId == id && u.ConsumptionDate >= BeginTime && u.ConsumptionDate < EndTime && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

            var list = lifeBillBll.GetList(where).ToList();
            double sum = list.Select(u => u.Money).ToArray().Sum();
            return sum;
        }

        /// <summary>
        /// 生活记账分类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel LifeBillType([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户Id查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.NO_USER;
                    }

                    //更新最近登录时间和Token过期时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //获取所有生活记账分类列表
                    ILifeBillTypeBLL lifeBillTypeBll = BLLFactory<ILifeBillTypeBLL>.GetBLL("LifeBillTypeBLL");
                    Expression<Func<T_LifeBillType, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

                    var list = lifeBillTypeBll.GetList(where, "Id", true).ToList().Select(q => new
                        {
                            Id = q.Id,
                            Img = q.Img,
                            Name = q.Name
                        });
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
        /// 记账明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel LifeBillDesc([FromUri]LifeBillModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户Id查找业主
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

                    ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
                    T_LifeBill lifebill = lifeBillBll.GetEntity(u => u.Id == model.Id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                    if (lifebill != null)
                    {
                        //获取记账详细
                        resultModel.result = new
                        {
                            AccountId = lifebill.Id,
                            CategoryId = lifebill.BillTypeId,
                            PayId = lifebill.PayTypeId,
                            Money = lifebill.Money,
                            CategoryName = lifebill.LifeBillType.Name,
                            Img = lifebill.LifeBillType.Img,
                            Payway = lifebill.LifePayType.Name,
                            Time = lifebill.ConsumptionDate.ToString("yyyy-MM-dd"),
                            CreateTime = lifebill.CreateDate.ToString("yyyy-MM-dd"),
                            Mark = lifebill.Memo
                        };
                    }
                    else
                    {
                        resultModel.Msg = "生活记账不存在";
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
        /// 添加记账
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddLifeBill(AddLifeBillModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户Id查找业主
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

                    //更新最近登录时间和Token过期时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //记账信息上传
                    T_LifeBill lifeBill = new T_LifeBill()
                    {
                        BillTypeId = model.CategoryId,
                        Money = model.Money,
                        PayTypeId = model.PayId,
                        ConsumptionDate = Convert.ToDateTime(model.PayDate),
                        CreateDate = Convert.ToDateTime(model.DateStr),
                        Memo = model.Mark,
                        UserId = model.UserId,
                        DelFlag = ConstantParam.DEL_FLAG_DEFAULT
                    };

                    //保存
                    ILifeBillBLL LifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
                    LifeBillBll.Save(lifeBill);
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
        /// 编辑记账
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel EditLifeBill(EditLifeBillModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //通过用户Id查找业主
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

                    ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
                    T_LifeBill lifebill = lifeBillBll.GetEntity(u => u.Id == model.Id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                    //如果该生活记账存在
                    if (lifebill != null)
                    {
                        lifebill.Id = model.Id;
                        lifebill.BillTypeId = model.CategoryId;
                        lifebill.PayTypeId = model.PayId;
                        lifebill.Money = model.Money;
                        lifebill.ConsumptionDate = Convert.ToDateTime(model.PayDate);
                        lifebill.CreateDate = Convert.ToDateTime(model.DateStr);
                        lifebill.Memo = model.Mark;

                        //更新
                        lifeBillBll.Update(lifebill);
                    }
                    else
                    {
                        resultModel.Msg = "生活记账不存在";
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
        /// 删除记账
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteLifeBill(LifeBillModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();

            try
            {
                //根据用户Id查找业主
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

                    ILifeBillBLL lifeBillBll = BLLFactory<ILifeBillBLL>.GetBLL("LifeBillBLL");
                    T_LifeBill lifebill = lifeBillBll.GetEntity(u => u.Id == model.Id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                    if (lifebill != null)
                    {
                        lifeBillBll.Delete(lifebill);
                    }
                    else
                    {
                        resultModel.Msg = "生活记账不存在";
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