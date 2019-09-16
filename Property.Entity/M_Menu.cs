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
    /// 菜单表
    /// </summary>
    public class M_Menu
    {
        public M_Menu()
        {
            this.Actions = new HashSet<M_Action>();
            this.ChildMenus = new HashSet<M_Menu>();
        }
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单描述
        /// </summary>
        [MaxLength(300)]
        public string MenuDesc { get; set; }

        /// <summary>
        /// 菜单Code
        /// </summary>
        [Required]
        public string MenuCode { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        [MaxLength(20)]
        public string IconClass { get; set; }

        /// <summary>
        /// 链接Url地址
        /// </summary>
        [MaxLength(200)]
        public string Href { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 菜单标识 0:导航菜单 1:其他菜单
        /// </summary>
        public int MenuFlag { get; set; }

        /// <summary>
        /// 删除菜单 0:默认 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 是否属于平台菜单 0:物业用户菜单 1：平台用户菜单 3:总公司平台菜单
        /// </summary>
        public int IsPlatform { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public Nullable<int> ParentId { get; set; }

        /// <summary>
        /// 外键 关联自身
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual M_Menu ParentMenu { get; set; }

        /// <summary>
        /// 该菜单下所有下属菜单
        /// </summary>
        public virtual ICollection<M_Menu> ChildMenus { get; set; }

        /// <summary>
        /// 该菜单下所有权限
        /// </summary>
        public virtual ICollection<M_Action> Actions { get; set; }
    }
}
