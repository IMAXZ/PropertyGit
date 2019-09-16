using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    public class WeixinExpenseNoticeController : WeixinBaseController
    {
        /// <summary>
        /// 缴费通知列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpenseNoticeList()
        {
            var owner = GetCurrentUser();
            //初始化查询条件
            var DoorIds = owner.PropertyIdentityVerification.Where(v => v.DoorId != null && v.IsVerified == 1).Select(m => m.DoorId);
            var CompanyIds = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.IsVerified == 1).Select(m => m.BuildCompanyId);
            Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => (DoorIds.Contains(u.BuildDoorId) || CompanyIds.Contains(u.BuildCompanyId));
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            ViewBag.ExpenseNoticeCount = expenseDetailsBLL.Count(where);

            return View();
        }

        /// <summary>
        /// 缴费通知Json方式获取
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult ExpenseNoticeJsonList(int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            var owner = GetCurrentUser();
            //初始化查询条件
            var DoorIds = owner.PropertyIdentityVerification.Where(v => v.DoorId != null && v.IsVerified == 1).Select(m => m.DoorId);
            var CompanyIds = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.IsVerified == 1).Select(m => m.BuildCompanyId);
            Expression<Func<T_HouseUserExpenseDetails, bool>> where = u => (DoorIds.Contains(u.BuildDoorId) || CompanyIds.Contains(u.BuildCompanyId));

            // 获取当前用户对应业主的缴费记录
            IHouseUserExpenseDetailsBLL expenseDetailsBLL = BLLFactory<IHouseUserExpenseDetailsBLL>.GetBLL("HouseUserExpenseDetailsBLL");
            model.Result = expenseDetailsBLL.GetPageList(where, "CreateDate", false, pageIndex).Select(m => new
            {
                Id = m.Id,
                ExpenseTitle = string.Format("{0}缴费通知", m.PropertyExpenseType.Name),
                ExpenseContent = string.Format("{0}{1}{2}{3}的业主你好, 您{4}总计{5}元。请及时到物业办公室或登录Ai我家App缴费, 谢谢!",
                m.BuildDoor.BuildUnit.Build.PropertyPlace.Name,
                m.BuildDoor.BuildUnit.Build.BuildName,
                m.BuildDoor.BuildUnit.UnitName,
                m.BuildDoor.DoorName,
                m.ExpenseDateDes,
                m.Expense),
                CreateDate = m.CreateDate,
                strCreateDate = m.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            model.Total = expenseDetailsBLL.Count(where);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
