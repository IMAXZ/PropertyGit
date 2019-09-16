using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Controllers;
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
    /// 操作日志控制器
    /// </summary>
    public class OpreateLogController : BaseController
    {
        /// <summary>
        /// 平台操作日志列表
        /// </summary>
        /// <param name="model">操作日志查询模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "操作日志")]
        [HttpGet]
        public ActionResult PlatformLogList(OperateLogSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartDate == null)
                model.StartDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            var user = this.GetSessionModel();
            Expression<Func<T_PlatformOpreateLog, bool>> where = PredicateBuilder.True<T_PlatformOpreateLog>();

            //3.如果是普通用户，只能查看自己的操作
            if (user.IsMgr == ConstantParam.USER_ROLE_DEFAULT)
            {
                where = where.And(l => l.OpreaterId == user.UserID);
            }
            //4.用户姓名查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = where.And(l => (l.Opreater.TrueName != null && l.Opreater.TrueName.Contains(model.Kword) || l.Opreater.UserName.Contains(model.Kword)));
            }
            DateTime endDate = model.EndDate.Value.AddDays(1);
            //5.时间范围查询
            where = where.And(l => l.OpreateTime > model.StartDate && l.OpreateTime < endDate);
            //6.排序
            var sortModel = this.SettingSorting("OpreateTime", false);

            //7.调用BLL层获取日志分页数据
            IPlatformOpreateLogBLL OpreateLogBLL = BLLFactory<IPlatformOpreateLogBLL>.GetBLL("PlatformOpreateLogBLL");
            model.PlatformLogList = OpreateLogBLL.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PlatformOpreateLog>;

            return View(model);
        }


        /// <summary>
        /// 物业操作日志列表
        /// </summary>
        /// <param name="model">操作日志查询模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "操作日志")]
        [HttpGet]
        public ActionResult PropertyLogList(OperateLogSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartDate == null)
                model.StartDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            var user = this.GetSessionModel();

            Expression<Func<T_PropertyOpreateLog, bool>> where = PredicateBuilder.True<T_PropertyOpreateLog>();

            //2.如果是普通用户，只能查看自己的操作
            if (user.IsMgr == ConstantParam.USER_ROLE_DEFAULT)
            {
                where = where.And(l => l.OpreaterId == user.UserID);
            }
            //4.用户姓名查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = where.And(l => (l.Opreater.TrueName != null && l.Opreater.TrueName.Contains(model.Kword) || l.Opreater.UserName.Contains(model.Kword)));
            }

            DateTime endDate = model.EndDate.Value.AddDays(1);
            //4.时间范围查询
            where = where.And(l => l.OpreateTime > model.StartDate && l.OpreateTime < endDate);
            var placeId = GetSessionModel().PropertyPlaceId.Value;
            where = where.And(l => l.Opreater.PropertyPlaceId == placeId);
            //5.排序
            var sortModel = this.SettingSorting("OpreateTime", false);

            //6.调用BLL层获取日志分页数据
            IPropertyOpreateLogBLL OpreateLogBLL = BLLFactory<IPropertyOpreateLogBLL>.GetBLL("PropertyOpreateLogBLL");
            model.PropertyLogList = OpreateLogBLL.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PropertyOpreateLog>;

            return View(model);
        }



        /// <summary>
        /// 物业总公司操作日志列表
        /// </summary>
        /// <param name="model">操作日志查询模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "操作日志")]
        [HttpGet]
        public ActionResult CompanyLogList(OperateLogSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartDate == null)
                model.StartDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            var user = this.GetSessionModel();
            int CurrentCompanyId = user.CompanyId.Value;
            Expression<Func<T_CompanyOpreateLog, bool>> where = l => l.Opreater.CompanyId == CurrentCompanyId;

            //3.如果是普通用户，只能查看自己的操作
            if (user.IsMgr == ConstantParam.USER_ROLE_DEFAULT)
            {
                where = where.And(l => l.OpreaterId == user.UserID);
            }
            //4.用户姓名查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = where.And(l => (l.Opreater.TrueName != null && l.Opreater.TrueName.Contains(model.Kword) || l.Opreater.UserName.Contains(model.Kword)));
            }
            DateTime endDate = model.EndDate.Value.AddDays(1);
            //5.时间范围查询
            where = where.And(l => l.OpreateTime > model.StartDate && l.OpreateTime < endDate);
            //6.排序
            var sortModel = this.SettingSorting("OpreateTime", false);

            //7.调用BLL层获取日志分页数据
            ICompanyOpreateLogBLL OpreateLogBLL = BLLFactory<ICompanyOpreateLogBLL>.GetBLL("CompanyOpreateLogBLL");
            model.CompanyLogList = OpreateLogBLL.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_CompanyOpreateLog>;

            return View(model);
        }
    }
}
