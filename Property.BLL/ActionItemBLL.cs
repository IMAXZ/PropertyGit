using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.BLL
{
    /// <summary>
    /// 权限子项业务层访问类
    /// </summary>
    public class ActionItemBLL : BaseBLL<M_ActionItem>, IActionItemBLL
    {
        private const string _Type = "ActionItemDAL";

        private IActionItemDAL _Dal;

        public ActionItemBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IActionItemDAL;

        }
    }
}
