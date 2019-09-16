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
    /// 物业小区表对应实体类
    /// </summary>
    public class T_PropertyPlace
    {
        public T_PropertyPlace()
        {
            this.PropertyRoles = new HashSet<T_PropertyRole>();
            this.InspectionCategorys = new HashSet<T_InspectionCategory>();
            this.InspectionPlans = new HashSet<T_InspectionPlan>();
            this.Questions = new HashSet<T_Question>();
            this.Posts = new HashSet<T_Post>();
            this.UserPlaces = new HashSet<R_UserPlace>();
            this.ShopPlaces = new HashSet<R_ShopPlace>();
            this.PropertyUsers = new HashSet<T_PropertyUser>();
            this.Builds = new HashSet<T_Build>();
            this.HouseUsers = new HashSet<T_HouseUser>();
            this.BuildCompanys = new HashSet<T_BuildCompany>();
            this.PostBarTopicTypes = new HashSet<T_PostBarTopicType>();
            this.PostBarTopics = new HashSet<T_PostBarTopic>();
            this.PropertyAccounts = new HashSet<T_PropertyAccount>();
            this.PropertyExpenseTypes = new HashSet<T_PropertyExpenseType>();
            this.SocialCircles = new HashSet<T_SocialCircle>();
        }

        /// <summary>
        /// 主键：小区ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 所属物业公司ID
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 所属公司ID关联表对象
        /// </summary>
        [ForeignKey("CompanyId")]
        public virtual T_Company Company { get; set; }

        /// <summary>
        /// 所属省
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 所属省份ID关联表对象
        /// </summary>
        [ForeignKey("ProvinceId")]
        public virtual M_Province Province { get; set; }

        /// <summary>
        /// 所属市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 所属市ID关联表对象
        /// </summary>
        [ForeignKey("CityId")]
        public virtual M_City City { get; set; }

        /// <summary>
        /// 所属县区
        /// </summary>
        public Nullable<int> CountyId { get; set; }

        /// <summary>
        /// 所属县区ID关联表对象
        /// </summary>
        [ForeignKey("CountyId")]
        public virtual M_County County { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 小区图片保存路径
        /// </summary>
        [MaxLength(100)]
        public string Img { get; set; }

        /// <summary>
        /// 小区缩略图保存路径
        /// </summary>
        [MaxLength(100)]
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 小区介绍
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        public string Tel { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public Nullable<double> Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public Nullable<double> Latitude { get; set; }

        /// <summary>
        /// 小区类型 0：住宅小区 1：办公楼小区
        /// </summary>
        public int PlaceType { get; set; }

        /// <summary>
        /// 是否需验证业主 0：验证 1：不需要验证
        /// </summary>
        public int IsValidate { get; set; }

        /// <summary>
        /// 删除标识  0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该物业小区下所有的角色
        /// </summary>
        public virtual ICollection<T_PropertyRole> PropertyRoles { get; set; }

        /// <summary>
        /// 该物业小区下所有的物业用户
        /// </summary>
        public virtual ICollection<T_PropertyUser> PropertyUsers { get; set; }

        /// <summary>
        /// 该物业小区下所有的巡检分类
        /// </summary>
        public virtual ICollection<T_InspectionCategory> InspectionCategorys { get; set; }

        /// <summary>
        /// 该物业小区下所有的巡检任务
        /// </summary>
        public virtual ICollection<T_InspectionPlan> InspectionPlans { get; set; }

        /// <summary>
        /// 该物业小区提报的所有问题
        /// </summary>
        public virtual ICollection<T_Question> Questions { get; set; }

        /// <summary>
        /// 该物业小区的所有公告
        /// </summary>
        public virtual ICollection<T_Post> Posts { get; set; }

        /// <summary>
        /// 该物业小区的所有用户业主关联
        /// </summary>
        public virtual ICollection<R_UserPlace> UserPlaces { get; set; }

        /// <summary>
        /// 该物业小区的所有门店关联
        /// </summary>
        public virtual ICollection<R_ShopPlace> ShopPlaces { get; set; }

        /// <summary>
        /// 该物业小区的所有楼座
        /// </summary>
        public virtual ICollection<T_Build> Builds { get; set; }

        /// <summary>
        /// 该物业小区的所有住宅业主
        /// </summary>
        public virtual ICollection<T_HouseUser> HouseUsers { get; set; }

        /// <summary>
        /// 该物业小区的所有办公楼单位
        /// </summary>
        public virtual ICollection<T_BuildCompany> BuildCompanys { get; set; }

        /// <summary>
        /// 该物业小区的所有贴吧主题分类表
        /// </summary>
        public virtual ICollection<T_PostBarTopicType> PostBarTopicTypes { get; set; }

        /// <summary>
        /// 该物业小区的所有贴吧主题表
        /// </summary>
        public virtual ICollection<T_PostBarTopic> PostBarTopics { get; set; }

        /// <summary>
        /// 该物业小区的所有账户表
        /// </summary>
        public virtual ICollection<T_PropertyAccount> PropertyAccounts { get; set; }

        /// <summary>
        /// 该物业小区下所有的缴费分类表
        /// </summary>
        public virtual ICollection<T_PropertyExpenseType> PropertyExpenseTypes { get; set; }

        /// <summary>
        /// 该物业小区下所有的业主圈子
        /// </summary>
        public virtual ICollection<T_SocialCircle> SocialCircles { get; set; }
    }
}
