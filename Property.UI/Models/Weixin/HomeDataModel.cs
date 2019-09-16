using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Weixin
{
    /// <summary>
    /// 首页数据模型
    /// </summary>
    public class HomeDataModel
    {
        /// <summary>
        /// 缴费通知
        /// </summary>
        public List<ExpenseNoticeModel> ExpenseList { get; set; }

        /// <summary>
        /// 最新公告列表
        /// </summary>
        public List<NewsModel> NewsList { get; set; }

        /// <summary>
        /// 上报问题列表
        /// </summary>
        public List<QuestionModel> QuestionList { get; set; }
    }

    /// <summary>
    /// 缴费通知模型
    /// </summary>
    public class ExpenseNoticeModel
    {
        /// <summary>
        /// 缴费类型
        /// </summary>
        public string ExpenseType { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 业主单元户
        /// </summary>
        public string OwnerDoor { get; set; }

        /// <summary>
        /// 缴费时间描述
        /// </summary>
        public string ExpenseDateDes { get; set; }

        /// <summary>
        /// 缴费金额
        /// </summary>
        public string Cost { get; set; }
    }

    public class NewsModel
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 物业图标
        /// </summary>
        public string propertyPic { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 上报问题模型
    /// </summary>
    public class QuestionModel
    {
        /// <summary>
        /// 问题ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 问题出现位置
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 状态 0:未处理 1：已处理
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 上报时间
        /// </summary>
        public string UploadTime { get; set; }

        /// <summary>
        /// 问题图片数组
        /// </summary>
        public string[] Imgs { get; set; }

        /// <summary>
        /// 语音路径
        /// </summary>
        public string AudioPath { get; set; }

        /// <summary>
        /// 语音长度
        /// </summary>
        public int? VoiceDuration { get; set; }
    }
}