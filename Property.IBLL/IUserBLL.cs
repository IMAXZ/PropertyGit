using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 业主业务层访问接口
    /// </summary>
    public interface IUserBLL : IBaseBLL<T_User>
    {
        /// <summary>
        /// 删除业主
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool DeleteUser(T_User user);
    }
}
