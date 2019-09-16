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
    public class PropertyPlaceBLL : BaseBLL<T_PropertyPlace>, IPropertyPlaceBLL
    {
        private const string _Type = "PropertyPlaceDAL";

        private IPropertyPlaceDAL _Dal;

        public PropertyPlaceBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyPlaceDAL;
        }

        /// <summary>
        /// 添加小区，指定系统角色
        /// </summary>
        /// <param name="place">小区实体对象</param>
        /// <returns></returns>
        public bool AddPlace(T_PropertyPlace place)
        {
            return this._Dal.AddPlace(place);
        }

        /// <summary>
        /// 删除标识改为删除，删除系统角色
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool DeletePlace(T_PropertyPlace place) 
        {
            return this._Dal.DeletePlace(place);
        }
    }  
}
