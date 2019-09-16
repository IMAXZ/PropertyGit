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
    /// 单元户查询模型
    /// </summary>
    public class BuildDoorSearchModel : SearchModel
    {
        /// <summary>
        /// 单元户ID
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 所属单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 所属楼座ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 单元户名称
        /// </summary>
        public string DoorName { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public PagedList<T_BuildDoor> List { get; set; }
    }

    /// <summary>
    /// 单元户添加单一对象模型
    /// </summary>
    public class BuildDoorAddModel 
    {
        public int Id { get; set; }
        /// <summary>
        /// 楼座ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 所属单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 单元户名称
        /// </summary>
        public string DoorName { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// 楼座列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }

        /// <summary>
        /// 单元列表
        /// </summary>
        public List<SelectListItem> UnitList { get; set; }

    }

    /// <summary>
    /// 批量添加单元户模型
    /// </summary>
    public class BuildDoorBatchAddModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 单元id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 楼座id
        /// </summary>
        public int BuildId { get; set; }
        public string[] DoorName { get; set; }
    }
}