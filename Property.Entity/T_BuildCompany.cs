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
    /// 办公楼单位业主信息登记表对应实体类
    /// </summary>
    public class T_BuildCompany
    {
        public T_BuildCompany()
        {
            this.PropertyExpenseNos = new HashSet<T_PropertyExpenseNo>();
            this.HouseUserExpenseTemplates = new HashSet<T_HouseUserExpenseTemplate>();
            this.HouseUserExpenseDetails = new HashSet<T_HouseUserExpenseDetails>();
            this.PropertyIdentityVerification = new HashSet<R_PropertyIdentityVerification>();
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 所属小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属小区关联
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 业主备注
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }

        /// <summary>
        /// 缴费备注
        /// </summary>

        [MaxLength(500)]
        public string PayDesc { get; set; }

        /// <summary>
        /// 服务备注
        /// </summary>

        [MaxLength(500)]
        public string ServiceDesc { get; set; }

        /// <summary>
        /// 删除标识 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 该办公楼单位下所有缴费编号表
        /// </summary>
        public virtual ICollection<T_PropertyExpenseNo> PropertyExpenseNos { get; set; }

        /// <summary>
        /// 该办公楼单位所有缴费模板表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseTemplate> HouseUserExpenseTemplates { get; set; }

        /// <summary>
        /// 该办公楼单位所有缴费明细表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseDetails> HouseUserExpenseDetails { get; set; }

        /// <summary>
        /// 该办公楼单位的身份验证表
        /// </summary>
        public virtual ICollection<R_PropertyIdentityVerification> PropertyIdentityVerification { get; set; }
    }
}
