using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 物业公司表对应实体类
    /// </summary>
    public class T_Company
    {
        public T_Company()
        {
            this.PropertyPlaces = new HashSet<T_PropertyPlace>();
            this.CompanyPosts = new HashSet<T_CompanyPost>();
            this.CompanyUsers = new HashSet<T_CompanyUser>();
            this.PropertyRoles = new HashSet<T_PropertyRole>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 物业公司名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 公司图标保存路径
        /// </summary>
        [MaxLength(100)]
        public string Img { get; set; }

        /// <summary>
        /// 公司图标缩略图保存路径
        /// </summary>
        [MaxLength(100)]
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 公司介绍
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(30)]
        public string Tel { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该物业公司下所有的小区
        /// </summary>
        public virtual ICollection<T_PropertyPlace> PropertyPlaces { get; set; }

        /// <summary>
        /// 物业公司下所有的公告
        /// </summary>
        public virtual ICollection<T_CompanyPost> CompanyPosts{ get; set; }

        /// <summary>
        /// 物业公司下所有的用户
        /// </summary>
        public virtual ICollection<T_CompanyUser> CompanyUsers { get; set; }
        /// <summary>
        /// 物业公司下所有的角色表
        /// </summary>
        public virtual ICollection<T_PropertyRole> PropertyRoles { get; set; }
    }
}
