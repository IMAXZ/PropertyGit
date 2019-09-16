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
    /// 客户端版本模型
    /// </summary>
    public class MobileVersionModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int VersionCode { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string VersionName { get; set; }

        /// <summary>
        /// 版本描述
        /// </summary>
        [MaxLength(200)]
        public string Desc { get; set; }

        /// <summary>
        /// 客户端类型 0：业主客户端 1：物业客户端 2：商户客户端
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// APK文件
        /// </summary>
        [Required]
        public HttpPostedFileBase ApkFile { get; set; }
    }

    /// <summary>
    /// 客户端版本查询模型
    /// </summary>
    public class MobileVersionSearchModel : SearchModel
    {
        /// <summary>
        /// 版本描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 客户端类型 0：业主客户端 1：物业客户端
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 客户端类型下拉列表
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// 查询到的结果集
        /// </summary>
        public PagedList<T_MobileVersion> List { get; set; }
    }
}