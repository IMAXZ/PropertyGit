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
    /// 物业小区单元户缴费编号表对应的实体类
    /// </summary>
    public class T_PropertyExpenseNo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 缴费编号
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ExpenseNumber { get; set; }

        /// <summary>
        /// 外键 缴费种类id
        /// </summary>
        public int ExpenseTypeId { get; set; }

        /// <summary>
        /// 缴费种类关联对象
        /// </summary>
        [ForeignKey("ExpenseTypeId")]
        public virtual T_PropertyExpenseType PropertyExpenseType { get; set; }

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
        /// 创建时间
        /// </summary>
        public Nullable<DateTime> CreatedDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }
    }
}
