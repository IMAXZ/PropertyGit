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
    /// APP版本管理业务层访问类
    /// </summary>
    public class MobileVersionBLL : BaseBLL<T_MobileVersion>, IMobileVersionBLL
    {
        private const string _Type = "MobileVersionDAL";

        private IMobileVersionDAL _Dal;

        public MobileVersionBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IMobileVersionDAL;
        }


        /// <summary>
        /// 删除指定版本
        /// </summary>
        /// <param name="id">版本id</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteVersion(int id)
        {
            return this._Dal.DeleteVersion(id);
        }
    }
}
