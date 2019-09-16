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
    /// 商家支持付款方式管理表 实体类
    /// </summary>
    public class T_ShopPaymentManagement
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 付款类型  （在线支付）1微信在线支付，2支付宝在线支付（货到支付）3货到现金付款，4货到微信付款，5货到支付宝付款
        /// </summary>
        public int PayTypeId { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 所属商家 关联表对象
        /// </summary>
        [ForeignKey("ShopId")]
        public virtual T_Shop Shop { get; set; }
    }
}
