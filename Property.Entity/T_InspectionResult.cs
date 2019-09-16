using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 巡检结果表对应实体类
    /// </summary>
    public class T_InspectionResult
    {
        public T_InspectionResult() 
        {
            this.ExceptionDisposes = new HashSet<T_InspectionExceptionDispose>();
        }

        /// <summary>
        /// 巡检结果ID：主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 巡检时间安排ID
        /// </summary>
        public int TimePlanId { get; set; }

        /// <summary>
        /// 巡检时间安排ID 关联表对象
        /// </summary>
        [ForeignKey("TimePlanId")]
        public virtual T_InspectionTimePlan InspectionTimePlan { get; set; }

        /// <summary>
        /// 巡检点ID
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// 巡检点ID 关联表对象
        /// </summary>
        [ForeignKey("PointId")]
        public virtual T_InspectionPoint InspectionPoint { get; set; }

        /// <summary>
        /// 提报人ID
        /// </summary>
        public int UploadUserId { get; set; }

        /// <summary>
        /// 提报人ID 关联表对象
        /// </summary>
        [ForeignKey("UploadUserId")]
        public virtual T_PropertyUser UploadUser { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(1000)]
        public string Desc { get; set; }

        /// <summary>
        /// 上传图片集路径 用;分开的多个路径图片
        /// </summary>
        [MaxLength(1000)]
        public string Imgs { get; set; }

        /// <summary>
        /// 语音文件路径
        /// </summary>
        [MaxLength(200)]
        public string AudioPath { get; set; }

        /// <summary>
        /// 巡检任务的计划日期
        /// </summary>
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 提报的服务器时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 客户端保存时间
        /// </summary>
        public DateTime ClientSaveTime { get; set; }

        /// <summary>
        /// 异常状态 0正常，1异常
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 处理状态  0:未处理  1：已处理
        /// </summary>
        public Nullable<int> DisposeStatus { get; set; }

        /// <summary>
        /// 指派的处理人ID
        /// </summary>
        public Nullable<int> DisposerId { get; set; }

        /// <summary>
        /// 指派的处理人 关联表对象
        /// </summary>
        [ForeignKey("DisposerId")]
        public virtual T_PropertyUser Disposer { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public Nullable<double> Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public Nullable<double> Latitude { get; set; }

        /// <summary>
        /// 该巡检结果对应的所有异常处理
        /// </summary>
        public virtual ICollection<T_InspectionExceptionDispose> ExceptionDisposes { get; set; }
    }
}
