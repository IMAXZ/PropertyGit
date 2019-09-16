using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    /// <summary>
    /// 楼座模型
    /// </summary>
    public class BuildModel
    {
        /// <summary>
        /// 楼座ID
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 楼座名称
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// 单元列表
        /// </summary>
        public List<UnitModel> Units { get; set; }
    }

    /// <summary>
    /// 单元模型
    /// </summary>
    public class UnitModel
    {
        /// <summary>
        /// 单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 户列表
        /// </summary>
        public List<DoorModel> Doors { get; set; }
    }

    /// <summary>
    /// 户模型
    /// </summary>
    public class DoorModel
    {
        /// <summary>
        /// 户ID
        /// </summary>
        public int DoorId { get; set; }

        /// <summary>
        /// 户名称
        /// </summary>
        public string DoorName { get; set; }
    }

    /// <summary>
    /// 办公楼单位模型
    /// </summary>
    public class BuildCompanyModel
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}