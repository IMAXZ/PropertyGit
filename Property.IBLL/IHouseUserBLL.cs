using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 住宅业主信息业务层访问接口
    /// </summary>
    public interface IHouseUserBLL:IBaseBLL<T_HouseUser>
    {
        bool ImportHouseUsers(List<T_HouseUser> houseUserList);
    }
}
