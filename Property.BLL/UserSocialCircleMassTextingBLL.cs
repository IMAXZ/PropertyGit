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
    /// 圈子群发成员业务层访问类
    /// </summary>
    public class UserSocialCircleMassTextingBLL : BaseBLL<R_UserSocialCircleMassTexting>, IUserSocialCircleMassTextingBLL
    {
        private const string _Type = "UserSocialCircleMassTextingDAL";

        private IUserSocialCircleMassTextingDAL _Dal;

        public UserSocialCircleMassTextingBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IUserSocialCircleMassTextingDAL;

        }
    }
}
