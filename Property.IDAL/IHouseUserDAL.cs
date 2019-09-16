using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 住宅业主数据层接口
    /// </summary>
    public interface IHouseUserDAL : IBaseDAL<T_HouseUser>
    {
        bool ImportHouseUsers(List<T_HouseUser> houseUserList);
    }
}
