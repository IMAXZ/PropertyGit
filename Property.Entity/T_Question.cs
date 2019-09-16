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
    /// 业主上报问题表 实体类
    /// </summary>
    public class T_Question
    {
        public T_Question() 
        {
            this.QuestionDisposes = new HashSet<T_QuestionDispose>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 提报人Id
        /// </summary>
        public int UploadUserId { get; set; }

        /// <summary>
        /// 外键 业主用户表
        /// </summary>
        [ForeignKey("UploadUserId")]
        public virtual T_User UploadUser { get; set; }

        /// <summary>
        /// 提报的服务器时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 客户端保存时间
        /// </summary>
        public DateTime ClientSaveTime { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 状态  0:未处理  1:已处理
        /// </summary>
        public int Status { get; set; }

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
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 外键 小区表
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 上传图片 用;分开的多个路径图片
        /// </summary>
        [MaxLength(1000)]
        public string Imgs { get; set; }

        /// <summary>
        /// 描述内容 问题描述内容
        /// </summary>
        [MaxLength(1000)]
        public string Desc { get; set; }

        /// <summary>
        /// 语音 语音保存文件
        /// </summary>
        [MaxLength(200)]
        public string AudioPath { get; set; }

        /// <summary>
        /// 语音时长（s）
        /// </summary>
        public Nullable<int> VoiceDuration { get; set; }

        /// <summary>
        /// 是否发布公示 默认：不公示（0）
        /// </summary>
        public int IsPublish { get; set; }

        /// <summary>
        /// 该问题对应的所有问题处理
        /// </summary>
        public virtual ICollection<T_QuestionDispose> QuestionDisposes { get; set; }
    }
}
