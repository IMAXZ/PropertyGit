using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业缴费一览明细控制器
    /// </summary>
    public class ExpenseDetailsController : BaseController
    {
        /// <summary>
        /// 缴费明细一览
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "物业缴费一览")]
        public ActionResult ExpenseList(ExpenseDetailsSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.BeforeDate == null)
                model.BeforeDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            DateTime endDate = model.EndDate.Value.AddDays(1);
            //2：初始化查询条件
            Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => u.PropertyExpenseType.PropertyPlaceId == CurrentPlaceId
                && u.ExpenseBeginDate < endDate && u.ExpenseEndDate >= model.BeforeDate;
            //缴费类型查询
            if (model.ExpenseTypeId != null)
            {
                where = PredicateBuilder.And(where, u => u.ExpenseTypeId == model.ExpenseTypeId.Value);
            }
            //根据缴费状态查询
            if (model.IsPayed != null)
            {
                where = PredicateBuilder.And(where, u => u.IsPayed == model.IsPayed.Value);
            }
            //楼座 单元 单元户,业主信息查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                IPropertyPlaceBLL placeBLL = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBLL.GetEntity(p => p.Id == CurrentPlaceId);
                if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
                {
                    where = PredicateBuilder.And(where, u => u.BuildDoor.DoorName.Contains(model.Kword)
                        || u.BuildDoor.BuildUnit.Build.BuildName.Contains(model.Kword)
                        || u.BuildDoor.BuildUnit.UnitName.Contains(model.Kword));
                }
                else if (place.PlaceType == ConstantParam.PLACE_TYPE_COMPANY)
                {
                    where = PredicateBuilder.And(where, u => u.BuildCompany.Name.Contains(model.Kword));
                }
            }
            //3.排序
            var sortModel = this.SettingSorting("Id", false);

            //4.调用BLL层获取分页数据
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            model.ResultList = expenseDetailsBLL.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_HouseUserExpenseDetails>;

            //5.初始化缴费类别下拉列表和缴费状态下拉列表
            model.ExpenseTypeList = GetExpenseTypeList();
            model.IsPayedList = GetPayedList();
            return View(model);
        }

        /// <summary>
        /// 获取当前小区缴费种类下拉列表
        /// </summary>
        private List<SelectListItem> GetExpenseTypeList()
        {
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPropertyExpenseTypeBLL expenseTypeBLL = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = expenseTypeBLL.GetList(u => u.PropertyPlaceId == CurrentPlaceId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 缴费状态列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetPayedList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "未缴费",
                Value = ConstantParam.PAYED_FALSE.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已缴费",
                Value = ConstantParam.PAYED_TRUE.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 前台缴费
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "前台缴费")]
        public ActionResult Paying(int id)
        {
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            var expenseDetails = expenseDetailsBLL.GetEntity(u => u.Id == id);
            if (expenseDetails != null)
            {
                ExpenseDetailsModel model = new ExpenseDetailsModel();
                model.Id = expenseDetails.Id;
                if (expenseDetails.BuildDoorId != null) 
                {
                    model.UnitDoor = expenseDetails.BuildDoor.BuildUnit.Build.BuildName + " " 
                        + expenseDetails.BuildDoor.BuildUnit.UnitName + " "+expenseDetails.BuildDoor.DoorName;
                }
                else if (expenseDetails.BuildCompanyId != null) 
                {
                    model.UnitDoor = expenseDetails.BuildCompany.Name;
                }
                model.ExpenseTypeName = expenseDetails.PropertyExpenseType.Name;
                switch (expenseDetails.ExpenseCycleId)
                {
                    case ConstantParam.ExpenseCycle_ONE_MONTH:
                        model.ExpenseCycle = "每月";
                        break;
                    case ConstantParam.ExpenseCycle_TWO_MONTH:
                        model.ExpenseCycle = "每两月";
                        break;
                    case ConstantParam.ExpenseCycle_ONE_QUARTER:
                        model.ExpenseCycle = "每季度";
                        break;
                    case ConstantParam.ExpenseCycle_HARF_YEAR:
                        model.ExpenseCycle = "每半年";
                        break;
                    case ConstantParam.ExpenseCycle_ONE_YEAR:
                        model.ExpenseCycle = "每年";
                        break;
                }
                model.Expense = expenseDetails.Expense;
                model.ExpenseDateDesc = expenseDetails.ExpenseDateDes;
                model.InvoiceTypeList = GetInvoiceTypeList();

                return View(model);
            }
            else 
            {
                return RedirectToAction("ExpenseList");
            }
        }

        /// <summary>
        /// 缴费开票类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetInvoiceTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "未开",
                Value = ConstantParam.InvoiceType_NO.ToString(),
                Selected = true
            });
            list.Add(new SelectListItem()
            {
                Text = "收据",
                Value = ConstantParam.InvoiceType_SJ.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "小票",
                Value = ConstantParam.InvoiceType_XP.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 前台缴费提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Paying(ExpenseDetailsModel model)
        {
            JsonModel jm = new JsonModel();
            //获取要去缴费的缴费明细
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            var expenseDetails = expenseDetailsBLL.GetEntity(u => u.Id == model.Id);
            if (expenseDetails == null)
            {
                jm.Msg = "该缴费记录不存在";
            }
            else if (expenseDetails.IsPayed == ConstantParam.PAYED_TRUE) 
            {
                jm.Msg = "该缴费记录已缴费";
            }
            else
            {
                expenseDetails.IsPayed = ConstantParam.PAYED_TRUE;
                expenseDetails.PaymentType = 1;
                expenseDetails.PayedDate = DateTime.Now;
                expenseDetails.Operator = GetSessionModel().UserID;
                expenseDetails.InvoiceType = model.InvoiceType;

                //编辑成功
                if (expenseDetailsBLL.Update(expenseDetails))
                {
                    //记录操作日志
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 缴费提醒
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PayRemind(int id) 
        {
            JsonModel jm = new JsonModel();

            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            var expenseDetails = expenseDetailsBLL.GetEntity(u => u.Id == id);
            if (expenseDetails == null)
            {
                jm.Msg = "该缴费记录不存在";
            }
            else if (expenseDetails.IsPayed == ConstantParam.PAYED_TRUE)
            {
                jm.Msg = "该缴费记录已缴费";
            }
            else
            {
                List<int> userIds = null;
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                if (expenseDetails.BuildDoorId != null)
                {
                    userIds = expenseDetails.BuildDoor.PropertyIdentityVerifications.Where(v => v.IsVerified == 1 && v.User.DelFlag == ConstantParam.DEL_FLAG_DEFAULT)
                        .Select(v => v.AppUserId).Distinct().ToList();
                }
                else if (expenseDetails.BuildCompanyId != null)
                {
                    userIds = expenseDetails.BuildCompany.PropertyIdentityVerification.Where(v => v.IsVerified == 1 && v.User.DelFlag == ConstantParam.DEL_FLAG_DEFAULT)
                        .Select(v => v.AppUserId).Distinct().ToList();
                }

                //推送缴费提醒
                //推送给业主客户端
                IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                string alert = expenseDetails.ExpenseDateDes + expenseDetails.PropertyExpenseType.Name + "缴费提醒";
                var registrationIds = userPushBLL.GetList(p => userIds.Contains(p.UserId)).Select(p => p.RegistrationId).ToArray();
                bool flag = PropertyUtils.SendPush("缴费提醒",alert, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);
                if (!flag)
                {
                    jm.Msg = "推送发生异常";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
