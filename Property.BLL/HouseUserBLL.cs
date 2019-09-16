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
    /// 住宅业主信息业务层访问类
    /// </summary>
    public class HouseUserBLL : BaseBLL<T_HouseUser>, IHouseUserBLL
    {
        private const string _Type = "HouseUserDAL";

        private IHouseUserDAL _Dal;

        public HouseUserBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IHouseUserDAL;
        }

        public bool ImportHouseUsers(List<T_HouseUser> houseUserList)
        {
            return this._Dal.ImportHouseUsers(houseUserList);
        }
    }
}
