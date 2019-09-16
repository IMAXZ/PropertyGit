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
    /// 用户加入圈子关联表业务层访问类
    /// </summary>
    public class UserSocialCircleBLL : BaseBLL<R_UserSocialCircle>, IUserSocialCircleBLL
    {
        private const string _Type = "UserSocialCircleDAL";

        private IUserSocialCircleDAL _Dal;

        public UserSocialCircleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IUserSocialCircleDAL;

        }
    }
}
