using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 平台管理物业公司模型
    /// </summary>
    public class PropertyCompanyModel
    {
        /// <summary>
        /// 物业公司Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 物业公司名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        [AllowHtml]
        public string Content { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(30)]
        public string Tel { get; set; }

        /// <summary>
        /// 公司图标
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 公司图标
        /// </summary>
        public HttpPostedFileBase UploadImg { get; set; }
    }

    /// <summary>
    /// 平台管理物业公司 编辑模型
    /// </summary>
    public class SetPropertyCompanyModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 图片保存路径
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 设置公司图标
        /// </summary>
        public HttpPostedFileBase UploadImg { get; set; }
    }

    /// <summary>
    /// 平台管理物业公司 查询模型
    /// </summary>
    public class PropertyCompanySearchModel : SearchModel
    {
        /// <summary>
        /// 物业公司名称
        /// </summary>
        public string Name { get; set; }
    }
}