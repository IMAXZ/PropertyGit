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
    /// 单元户信息表对应实体类
    /// </summary>
    public class T_BuildDoor
    {
        public T_BuildDoor()
        {
            this.HouseUsers = new HashSet<T_HouseUser>();
            this.PropertyExpenseNos = new HashSet<T_PropertyExpenseNo>();
            this.HouseUserExpenseTemplates = new HashSet<T_HouseUserExpenseTemplate>();
            this.HouseUserExpenseDetails = new HashSet<T_HouseUserExpenseDetails>();
            this.PropertyIdentityVerifications = new HashSet<R_PropertyIdentityVerification>();
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 单元户名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string DoorName { get; set; }

        /// <summary>
        /// 平方米数
        /// </summary>
        public double SquareMeter { get; set; }

        /// <summary>
        /// 所属单元ID
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 所属单元ID关联表对象
        /// </summary>
        [ForeignKey("UnitId")]
        public virtual T_BuildUnit BuildUnit { get; set; }

        /// <summary>
        /// 该单元户下的住宅业主
        /// </summary>
        public virtual ICollection<T_HouseUser> HouseUsers { get; set; }

        /// <summary>
        /// 该单元户下所有缴费编号表
        /// </summary>
        public virtual ICollection<T_PropertyExpenseNo> PropertyExpenseNos { get; set; }

        /// <summary>
        /// 该户所有缴费模板表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseTemplate> HouseUserExpenseTemplates { get; set; }

        /// <summary>
        /// 该户所有缴费明细表
        /// </summary>
        public virtual ICollection<T_HouseUserExpenseDetails> HouseUserExpenseDetails { get; set; }

        /// <summary>
        /// 该户下的身份验证表
        /// </summary>
        public virtual ICollection<R_PropertyIdentityVerification> PropertyIdentityVerifications { get; set; }
    }
}
