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
    /// 商家用户推送设备业务层访问类
    /// </summary>
    public class ShopUserPushBLL : BaseBLL<T_ShopUserPush>, IShopUserPushBLL
    {
        private const string _Type = "ShopUserPushDAL";

        private IShopUserPushDAL _Dal;

        public ShopUserPushBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopUserPushDAL;
        }
    }
}
