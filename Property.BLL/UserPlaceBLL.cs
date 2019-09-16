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
    /// 物业小区业务层访问类
    /// </summary>
    public class UserPlaceBLL : BaseBLL<R_UserPlace>, IUserPlaceBLL
    {
        private const string _Type = "UserPlaceDAL";

        private IUserPlaceDAL _Dal;

        public UserPlaceBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IUserPlaceDAL;
        }


    }  
}
