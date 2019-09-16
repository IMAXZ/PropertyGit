using Property.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 用户session模型
    /// </summary>
    [Serializable]
    public class UserSessionModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserSessionModel()
        {
            this.MenuList = new List<MenuModel>();
            this.ActionDic = new Dictionary<string, string>();
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string HeadPath { get; set; }

        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<MenuModel> MenuList { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public Dictionary<string, string> ActionDic { get; set; }


        /// <summary>
        /// 用户类型：0：物业用户  1：平台用户 2：门店平台用户 3:总公司用户
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 所属小区
        /// </summary>
        public int? PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属总公司
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// 用户权限类型： 1:管理员  0:普通用户
        /// </summary>
        public int IsMgr { get; set; }
    }

    /// <summary>
    /// 菜单类
    /// </summary>
    public class MenuModel
    {

        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单Code
        /// </summary>
        public string MenuCode { get; set; }

        /// <summary>
        /// 菜单标识-左边导航菜单和顶部导航菜单
        /// </summary>
        public int MenuFlag { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 对应的样式信息
        /// </summary>
        public string MenuCss { get; set; }

        /// <summary>
        /// 菜单顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 菜单所属平台类型 0:物业平台菜单 1：后台菜单 3:总公司平台菜单
        /// </summary>
        public int IsPlatform { get; set; }

    }
}