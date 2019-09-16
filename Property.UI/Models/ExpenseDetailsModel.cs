using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 前台缴费模型
    /// </summary>
    public class ExpenseDetailsModel
    {
        /// <summary>
        /// 缴费明细ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所属户或单位
        /// </summary>
        public string UnitDoor { get; set; }

        /// <summary>
        /// 缴费类别名称
        /// </summary>
        public string ExpenseTypeName { get; set; }

        /// <summary>
        /// 缴费周期
        /// </summary>
        public string ExpenseCycle { get; set; }

        /// <summary>
        /// 缴费金额
        /// </summary>
        public double Expense { get; set; }

        /// <summary>
        /// 费用时间描述
        /// </summary>
        public string ExpenseDateDesc { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 开票类型下拉列表
        /// </summary>
        public List<SelectListItem> InvoiceTypeList { get; set; }
    }

    /// <summary>
    /// 缴费明细查询模型
    /// </summary>
    public class ExpenseDetailsSearchModel : SearchModel
    {
        /// <summary>
        /// 缴费种类ID
        /// </summary>
        public int? ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费种类列表
        /// </summary>
        public List<SelectListItem> ExpenseTypeList { get; set; }

        /// <summary>
        /// 是否缴费
        /// </summary>
        public int? IsPayed { get; set; }

        /// <summary>
        /// 缴费状态列表
        /// </summary>
        public List<SelectListItem> IsPayedList { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? BeforeDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 缴费明细分页数据
        /// </summary>
        public PagedList<T_HouseUserExpenseDetails> ResultList { get; set; }
    }
}