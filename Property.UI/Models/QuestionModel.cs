using Property.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Models
{
    /// <summary>
    /// 业主上报问题查询模型
    /// </summary>
    public class QuestionSearchModel : SearchModel
    {
        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 查询到的上报问题列表
        /// </summary>
        public PagedList<T_Question> DataList { get; set; }

        /// <summary>
        /// 状态  0:新建  1:处理中  2:已处理 3：已关闭
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 状态下拉列表
        /// </summary>
        public List<SelectListItem> StatusList { get; set; }

    }


    /// <summary>
    /// 问题处理模型
    /// </summary>
    public class DisposeQuestionModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 处理详情
        /// </summary>
        public string DisposeDesc { get; set; }

        /// <summary>
        /// 是否发布公示
        /// </summary>
        public bool IsPublish { get; set; }
    }

    /// <summary>
    /// 指派上报问题处理人模型
    /// </summary>
    public class SetQuestionDisposerModel
    {
        /// <summary>
        /// 上报问题Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 处理人ID
        /// </summary>
        public int DisposerId { get; set; }

        /// <summary>
        /// 物业用户列表
        /// </summary>
        public List<SelectListItem> UserList { get; set; }
    }

    /// <summary>
    /// 业主上报问题-平台展示列表-搜索模型
    /// </summary>
    public class QuestionPlatformSearchModel : SearchModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int? PropertyPlaceId { get; set; }

        /// <summary>
        /// 查询到的上报问题列表
        /// </summary>
        public PagedList<T_Question> DataList { get; set; }

        /// <summary>
        /// 状态  0:新建  1:处理中  2:已处理 3：已关闭
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 描述内容 问题描述内容
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 状态下拉列表
        /// </summary>
        public List<SelectListItem> StatusList { get; set; }

        /// <summary>
        /// 物业小区下拉列表
        /// </summary>
        public List<SelectListItem> PropertyPlaceList { get; set; }
    }

    /// <summary>
    /// 上报问题提醒数据模型
    /// </summary>
    public class QuestionIntroModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 多久以前
        /// </summary>
        public string TimeAgoStr { get; set; }
    }
}