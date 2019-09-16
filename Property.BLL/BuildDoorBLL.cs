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
    /// 楼座业务层访问类
    /// </summary>
    public class BuildDoorBLL : BaseBLL<T_BuildDoor>, IBuildDoorBLL
    {
        private const string _Type = "BuildDoorDAL";

        private IBuildDoorDAL _Dal;

        public BuildDoorBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IBuildDoorDAL;
        }

        /// <summary>
        /// 批量添加单元户
        /// </summary>
        /// <param name="unitId">单元id</param>
        /// <param name="list">单元户对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool BatchAddDoor(int unitId, List<T_BuildDoor> list)
        {
            return this._Dal.BatchAddDoor(unitId, list);
        }
    }
}
