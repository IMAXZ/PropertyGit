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
    /// 业主客户端推送业务层访问类
    /// </summary>
    public class UserPushBLL : BaseBLL<T_UserPush>, IUserPushBLL
    {
        private const string _Type = "UserPushDAL";

        private IUserPushDAL _Dal;

        public UserPushBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IUserPushDAL;
        }
    }
}
