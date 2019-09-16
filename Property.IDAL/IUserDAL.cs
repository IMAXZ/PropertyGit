using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 业主数据层访问接口
    /// </summary>
    public interface IUserDAL : IBaseDAL<T_User>
    {
        bool DeleteUser(T_User user);
    }
}
