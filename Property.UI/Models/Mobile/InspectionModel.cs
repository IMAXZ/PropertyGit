using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{

    /// <summary>
    /// 巡检任务巡检点详细模型
    /// </summary>
    public class InspectionPlanDetailModel : TokenModel
    {
        /// <summary>
        /// 巡检任务ID
        /// </summary>
        public int PlanId { get; set; }
    }

    /// <summary>
    /// 上传巡检结果模型
    /// </summary>
    public class InspectionResultModel : TokenModel
    {
        /// <summary>
        /// 巡检任务ID
        /// </summary>
        public int TimePlanId { get; set; }

        /// <summary>
        /// 巡检点ID
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        [MaxLength(1000)]
        public string Desc { get; set; }

        /// <summary>
        /// 图片集 Base64数据
        /// </summary>
        public string ImgFiles { get; set; }

        /// <summary>
        /// 语音描述文件 Base64数据
        /// </summary>
        public string AudioFile { get; set; }

        /// <summary>
        /// 异常状态 0正常，1异常
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 客户端保存时间
        /// </summary>
        public DateTime ClientSaveTime { get; set; }

        /// <summary>
        /// 巡检计划日期
        /// </summary>
        public DateTime PlanDate { get; set; }
    }
}