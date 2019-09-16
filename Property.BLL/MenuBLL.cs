using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Property.BLL
{
    /// <summary>
    /// 菜单业务层访问类
    /// </summary>
    public class MenuBLL : BaseBLL<M_Menu>, IMenuBLL
    {
        private const string _Type = "MenuDAL";

        private IMenuDAL _Dal;

        public MenuBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IMenuDAL;
        }
    }
}
