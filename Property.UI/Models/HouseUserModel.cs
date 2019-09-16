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
    /// 住宅业主管理查询模型
    /// </summary>
    public class HouseUserSearchModel : SearchModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 查询到的住宅业主列表
        /// </summary>
        public PagedList<T_HouseUser> DataList { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int? PropertyPlaceId { get; set; }

        /// <summary>
        /// 物业小区下拉列表
        /// </summary>
        public List<SelectListItem> PropertyPlaceList { get; set; }
    }

    /// <summary>
    /// 新增住宅业主模型
    /// </summary>
    public class HouseUserModel
    {
        /// <summary>
        ///Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///姓名
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// 所属楼座Id
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 所属单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 所属户Id
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 业主备注
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }

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
        /// 性别下拉列表
        /// </summary>
        public List<SelectListItem> GenderList { get; set; }

        /// <summary>
        /// 楼座下拉列表
        /// </summary>
        public List<SelectListItem> BuildList { get; set; }

        /// <summary>
        /// 单元下拉列表
        /// </summary>
        public List<SelectListItem> UnitList { get; set; }

        /// <summary>
        /// 户下拉列表
        /// </summary>
        public List<SelectListItem> DoorList { get; set; }
    }
}