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
    /// 业主审批信息 查询模型
    /// </summary>
    public class ApprovalOwnerModel:SearchModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// 是否通过验证 0.无通过 1.通过
        /// </summary>
        public int? IsVerified { get; set; }

        /// <summary>
        /// 查询到的业主审批信息列表
        /// </summary>
        public PagedList<R_PropertyIdentityVerification> DataList { get; set; }

        /// <summary>
        /// 状态下拉列表
        /// </summary>
        public List<SelectListItem> IsVerifiedList { get; set; }
    }

}