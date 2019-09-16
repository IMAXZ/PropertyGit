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
    public class BuildCompanyModel : SearchModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Required(ErrorMessage="请输入单位名称")]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required(ErrorMessage="请输入联系电话")]
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 业主备注
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int? PropertyPlaceId { get; set; }

        /// <summary>
        /// 物业小区下拉列表
        /// </summary>
        public List<SelectListItem> CompanyPlaceList { get; set; }

        /// <summary>
        /// 查询到的办公楼单位业主列表
        /// </summary>
        public PagedList<T_BuildCompany> DataList { get; set; }

        /// <summary>
        /// 缴费备注
        /// </summary>
        
        [MaxLength(500)]
        public string PayDesc { get; set; }

        /// <summary>
        /// 服务备注
        /// </summary>
        
        [MaxLength(500)]
        public string ServiceDesc { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }
    }
}