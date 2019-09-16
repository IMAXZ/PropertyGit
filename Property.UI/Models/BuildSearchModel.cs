using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 楼盘查询模型
    /// </summary>
    public class BuildSearchModel : SearchModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        [Required]
        public string BuildName { get; set; }

        /// <summary>
        /// 所属小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        public string PropertyPlace { get; set; }

        /// <summary>
        /// 楼座描述
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }
    }

    /// <summary>
    /// 批量添加单元模型
    /// </summary>
    public class BuildUnitBatchAddModel
    {
        /// <summary>
        /// 楼座id
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 单元名列表
        /// </summary>
        public string[] UnitName { get; set; }
    }
}