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
    /// 商家订单查询模型
    /// </summary>
    public class StoreOrderSearchModel:SearchModel
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 查询到的商家订单列表
        /// </summary>
        public PagedList<T_Order> DataList { get; set; }

        /// <summary>
        /// 订单状态  0.待付款 1.待确认 2.待收货 3.已退单 4.交易完毕 5.交易关闭
        /// </summary>
        public int? OrderStatus { get; set; }

        /// <summary>
        /// 订单状态下拉列表
        /// </summary>
        public List<SelectListItem> OrderStatusList { get; set; }

        /// <summary>
        /// 支付方式  0：默认未支付 1:微信在线支付 2:支付宝在线支付 3:货到现金付款 4:货到微信付款 5:货到支付宝付款
        /// </summary>
        public int? PayWay { get; set; }

        /// <summary>
        /// 支付方式下拉列表
        /// </summary>
        public List<SelectListItem> PayWayList { get; set; }
    }

    /// <summary>
    /// 订单状态修改模型
    /// </summary>
    public class OrderStatusUpdateModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 2.待收货 3.已退单 4.交易完毕
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 修改原因（退单）
        /// </summary>
        [MaxLength(100)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 退单模型
    /// </summary>
    public class RecedeReasonModel
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 退单原因
        /// </summary>
        public string Reason { get; set; }
    }
}