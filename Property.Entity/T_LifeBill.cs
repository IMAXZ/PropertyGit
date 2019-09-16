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
    /// 生活记账表 实体类
    /// </summary>
    public class T_LifeBill
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 记账日期
        /// </summary>
        public DateTime ConsumptionDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }

        /// <summary>
        /// 删除标识 0.未删除 1.删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 所属业主用户Id关联表对象
        /// </summary>
        [ForeignKey("UserId")]
        public virtual T_User User { get; set; }

        /// <summary>
        /// 记账分类Id
        /// </summary>
        public int BillTypeId { get; set; }

        /// <summary>
        /// 所属分类Id关联表对象
        /// </summary>
        [ForeignKey("BillTypeId")]
        public virtual T_LifeBillType LifeBillType { get; set; }

        /// <summary>
        /// 支付方式Id
        /// </summary>
        public int PayTypeId { get; set; }

        /// <summary>
        /// 所属支付方式Id关联表对象
        /// </summary>
        [ForeignKey("PayTypeId")]
        public virtual T_LifePayType LifePayType { get; set; }
    }
}
