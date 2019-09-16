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
    /// 楼座单元查询模型
    /// </summary>
    public class BuildUnitSearchModel : SearchModel
    {
        /// <summary>
        /// 单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 楼座单元名称
        /// </summary>
        [Required(ErrorMessage = "不能为空")]
        [MaxLength(50)]
        public string UnitName { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// 所属楼座ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public PagedList<T_BuildUnit> List { get; set; }


        /// <summary>
        /// 楼座列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }
    }

    /// <summary>
    /// 添加单个Unit模型
    /// </summary>
    public class BuildUnitAddModel
    {
        /// <summary>
        /// 楼座单元名称
        /// </summary>
        [Required(ErrorMessage = "不能为空")]
        [MaxLength(50)]
        public string UnitName { get; set; }

        /// <summary>
        /// 所属楼座ID
        /// </summary>
        [Required(ErrorMessage = "不能为空")]
        public int BuildId { get; set; }

        /// <summary>
        /// 楼座列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }
    }


}