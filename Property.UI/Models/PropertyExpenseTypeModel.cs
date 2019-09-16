using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    public class PropertyExpenseTypeModel
    {
        /// <summary>
        /// 缴费分类表主键
        /// </summary>
        public int ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费名称
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 缴费备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 是否固定 0.非固定缴费类型 1.固定缴费类型
        /// </summary>
        public int? IsFixed { get; set; }

        /// <summary>
        /// 缴费分类下拉
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }
    }

    /// <summary>
    /// 缴费类别查询模型
    /// </summary>
    public class ExpenseTypeSearchModel : SearchModel
    {
        /// <summary>
        /// 是否固定 0.非固定缴费类型 1.固定缴费类型
        /// </summary>
        public int? IsFixed { get; set; }

        /// <summary>
        /// 是否固定下拉列表
        /// </summary>
        public List<SelectListItem> IsFixedList { get; set; }

        /// <summary>
        /// 查询到的所有缴费分类表的数据
        /// </summary>
        public PagedList<T_PropertyExpenseType> DataList { get; set; }
    }
}