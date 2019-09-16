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
    /// 物业小区贴吧主题分类表 实体类
    /// </summary>
    public class T_PostBarTopicType
    {
        public T_PostBarTopicType()
        {
            this.PostBarTopics = new HashSet<T_PostBarTopic>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属小区ID关联表对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 该分类下所有主题表
        /// </summary>
        public virtual ICollection<T_PostBarTopic> PostBarTopics { get; set; }
    }
}
