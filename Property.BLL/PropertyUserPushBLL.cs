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
    /// 物业客户端推送业务层访问类
    /// </summary>
    public class PropertyUserPushBLL : BaseBLL<T_PropertyUserPush>, IPropertyUserPushBLL
    {
        private const string _Type = "PropertyUserPushDAL";

        private IPropertyUserPushDAL _Dal;

        public PropertyUserPushBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyUserPushDAL;
        }
    }
}
