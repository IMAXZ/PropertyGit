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
    /// 系统消息业务层访问类
    /// </summary>
    public class SystemMessageBLL : BaseBLL<T_SystemMessage>, ISystemMessageBLL
    {
        private const string _Type = "SystemMessageDAL";

        private ISystemMessageDAL _Dal;

        public SystemMessageBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ISystemMessageDAL;
        }
    }
}
