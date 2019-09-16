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
    /// 业主业务层访问类
    /// </summary>
    public class UserBLL : BaseBLL<T_User>, IUserBLL
    {
        private const string _Type = "UserDAL";

        private IUserDAL _Dal;

        public UserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IUserDAL;
        }

        /// <summary>
        /// 删除业主
        /// </summary>
        /// <param name="plan">要删除的业主</param>
        /// <returns></returns>
        public bool DeleteUser(T_User user)
        {
            return this._Dal.DeleteUser(user);
        }
    }
}
