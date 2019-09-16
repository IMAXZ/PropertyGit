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
    /// 物业小区业主缴费明细表对应的实体类
    /// </summary>
    public class T_HouseUserExpenseDetails
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 外键缴费种类Id
        /// </summary>
        public int ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费种类对应关联表
        /// </summary>
        [ForeignKey("ExpenseTypeId")]
        public virtual T_PropertyExpenseType PropertyExpenseType { get; set; }

        /// <summary>
        /// 缴费金额
        /// </summary>
        public double Expense { get; set; }

        /// <summary>
        /// 缴费周期 1.每月 2.每两个月 3.每季度 4.每半年 5.每年
        /// </summary>
        public int ExpenseCycleId { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 应缴开始日期
        /// </summary>
        public DateTime ExpenseBeginDate { get; set; }

        /// <summary>
        /// 应缴结束日期
        /// </summary>
        public DateTime ExpenseEndDate { get; set; }

        /// <summary>
        /// 实际缴费日期
        /// </summary>
        public Nullable<DateTime> PayedDate { get; set; }

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
        /// 是否交费0:未缴费 1:已缴费
        /// </summary>
        public int IsPayed { get; set; }

        /// <summary>
        /// 交费方式1:前台缴费 2:在线交费
        /// </summary>
        public Nullable<int> PaymentType { get; set; }

        /// <summary>
        /// 前台物业缴费操作人ID
        /// </summary>
        public Nullable<int> Operator { get; set; }

        /// <summary>
        /// 物业缴费操作人
        /// </summary>
        [ForeignKey("Operator")]
        public virtual T_PropertyUser OperatorUser { get; set; }

        /// <summary>
        /// 前台物业缴费操作日期
        /// </summary>
        public Nullable<DateTime> OperatorDate { get; set; }

        /// <summary>
        /// 费用时间描述
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ExpenseDateDes { get; set; }

        /// <summary>
        /// 开票类型0:未开 1:收据 2:小票
        /// </summary>
        public Nullable<int> InvoiceType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 微信缴费支付订单
        /// </summary>
        [MaxLength(100)]
        public string PayTradeNo { get; set; }
    }
}
