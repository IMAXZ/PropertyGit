using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 物业小区业主缴费模板表对应的实体类
    /// </summary>
    public class T_HouseUserExpenseTemplate
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 外键缴费种类id
        /// </summary>
        public int ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费种类表关联对象
        /// </summary>
        [ForeignKey("ExpenseTypeId")]
        public virtual T_PropertyExpenseType PropertyExpenseType { get; set; }

        /// <summary>
        /// 缴费金额
        /// </summary>
        public double Expense { get; set; }

        /// <summary>
        /// 缴费周期1:每月 2:每两个月 3:每季度 4:每半年 5:每年
        /// </summary>
        public int ExpenseCycleId { get; set; }

        /// <summary>
        /// 推送日期
        /// </summary>
        public DateTime NotificationDate { get; set; }

        /// <summary>
        /// 外键所属单元户id
        /// </summary>
        public Nullable<int> BuildDoorId { get; set; }

        /// <summary>
        /// 所属关联户关联对象
        /// </summary>
        [ForeignKey("BuildDoorId")]
        public virtual T_BuildDoor BuildDoor { get; set; }

        /// <summary>
        /// 办公楼业主ID
        /// </summary>
        public Nullable<int> BuildCompanyId { get; set; }

        /// <summary>
        /// 所属办公楼业主关联对象
        /// </summary>
        [ForeignKey("BuildCompanyId")]
        public virtual T_BuildCompany BuildCompany { get; set; }

        /// <summary>
        /// 模板操作人ID
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// 模板操作人
        /// </summary>
        [ForeignKey("Operator")]
        public virtual T_PropertyUser OperatorUser { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        public Nullable<DateTime> OperatorDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }
    }
}
