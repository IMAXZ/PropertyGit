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
    /// 门店业务层访问类
    /// </summary>
    public class ShopBLL : BaseBLL<T_Shop>, IShopBLL
    {
        private const string _Type = "ShopDAL";

        private IShopDAL _Dal;

        public ShopBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IShopDAL;
        }

        /// <summary>
        /// 编辑门店，重新设置服务小区
        /// </summary>
        /// <param name="shop">门店记录</param>
        /// <param name="placeIds">服务小区ID</param>
        /// <returns></returns>
        public bool UpdateShop(T_Shop shop, string placeIds) 
        {
            return this._Dal.UpdateShop(shop, placeIds); 
        }

        /// <summary>
        /// 为商家设置支付类型
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SetupPayTypes(T_Shop shop, List<T_ShopPaymentManagement> list)
        {
            return this._Dal.SetupPayTypes(shop, list);
        }
    }
}
