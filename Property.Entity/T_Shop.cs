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
    /// 门店表对应实体类
    /// </summary>
    public class T_Shop
    {
        public T_Shop()
        {
            this.ShopPlaces = new HashSet<R_ShopPlace>();
            this.ShopAccounts = new HashSet<T_ShopAccounts>();
            this.ShopPaymentManagements = new HashSet<T_ShopPaymentManagement>();
            this.ShopShippingCosts = new HashSet<T_ShopShippingCost>();
            this.Orders = new HashSet<T_Order>();
            this.GoodsCategorys = new HashSet<T_GoodsCategory>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 门店图片路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 门店缩略图路径
        /// </summary>
        public string ImgThumbnail { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        public int ShopUserId { get; set; }

        /// <summary>
        /// 所属用户ID关联表对象
        /// </summary>
        [ForeignKey("ShopUserId")]
        public virtual T_ShopUser ShopUser { get; set; }

        /// <summary>
        /// 门店内容介绍
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 门店主营介绍
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string MainSale { get; set; }

        /// <summary>
        /// 是否支持上门送货
        /// </summary>
        public Nullable<int> IsDelivery { get; set; }

        /// <summary>
        /// 门店类型,可多选，多个之间用,分割
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Type { get; set; }

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
        [Required]
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
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 开始营业时间
        /// </summary>
        public int StartBusinessTime { get; set; }

        /// <summary>
        /// 结束营业时间
        /// </summary>
        public int EndBusinessTime { get; set; }

        /// <summary>
        /// 该门店服务的的所有小区关联
        /// </summary>
        public virtual ICollection<R_ShopPlace> ShopPlaces { get; set; }

        /// <summary>
        /// 该门店表下的所有账户
        /// </summary>
        public virtual ICollection<T_ShopAccounts> ShopAccounts { get; set; }

        /// <summary>
        /// 该门店表下的所有支持付款方式
        /// </summary>
        public virtual ICollection<T_ShopPaymentManagement> ShopPaymentManagements { get; set; }

        /// <summary>
        /// 该门店表下的所有运费设置表
        /// </summary>
        public virtual ICollection<T_ShopShippingCost> ShopShippingCosts { get; set; }

        /// <summary>
        /// 该门店表下的所有订单表
        /// </summary>
        public virtual ICollection<T_Order> Orders { get; set; }

        /// <summary>
        /// 该门店下的所有的促销商品分类
        /// </summary>
        public virtual ICollection<T_GoodsCategory> GoodsCategorys { get; set; }
    }
}
