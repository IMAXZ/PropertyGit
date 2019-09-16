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
    /// <summary>
    /// 缴费编号模型
    /// </summary>
    public class PaymentNoModel
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小区类型 0：住宅小区 1：办公楼小区
        /// </summary>
        public int PlaceType { get; set; }

        /// <summary>
        /// 缴费编号
        /// </summary>
        [MaxLength(500)]
        [Required]
        public string ExpenseNumber { get; set; }

        /// <summary>
        /// 外键 缴费种类id
        /// </summary>
        public int ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费种类列表
        /// </summary>
        public List<SelectListItem> ExpenseTypeList { get; set; }

        /// <summary>
        /// 所属楼座Id
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 所属单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 所属单元户id
        /// </summary>
        public int? DoorId { get; set; }

        /// <summary>
        /// 楼座下拉列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }

        /// <summary>
        /// 单元下拉列表
        /// </summary>
        public List<SelectListItem> UnitList { get; set; }

        /// <summary>
        /// 户下拉列表
        /// </summary>
        public List<SelectListItem> DoorList { get; set; }

        /// <summary>
        /// 办公楼业主ID
        /// </summary>
        public int? BuildCompanyId { get; set; }

        /// <summary>
        ///办公楼单位列表
        /// </summary>
        public List<SelectListItem> BuildCompanyList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }
    }

    /// <summary>
    /// 单元户缴费编号查询模型
    /// </summary>
    public class PaymentNoSearchModel : SearchModel
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
        /// 缴费编号
        /// </summary>
        public string ExpenseNumber { get; set; }

        /// <summary>
        /// 缴费编号分页数据
        /// </summary>
        public PagedList<T_PropertyExpenseNo> ResultList { get; set; }
    }
}