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
    /// 权限组业务层访问类
    /// </summary>
    public class ActionBLL : BaseBLL<M_Action>, IActionBLL
    {
        private const string _Type = "ActionDAL";

        private IActionDAL _Dal;

        public ActionBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IActionDAL;

        }
    }
}
