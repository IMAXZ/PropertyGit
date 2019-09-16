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
    /// 楼座单元业务层访问类
    /// </summary>
    public class BuildUnitBLL : BaseBLL<T_BuildUnit>, IBuildUnitBLL
    {
        private const string _Type = "BuildUnitDAL";

        private IBuildUnitDAL _Dal;

        public BuildUnitBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IBuildUnitDAL;
        }


        /// <summary>
        /// 批量添加单元
        /// </summary>
        /// <param name="buildId">楼座id</param>
        /// <param name="list">单元对象列表</param>
        /// <returns>是否分配成功</returns>
        public bool BatchAddUnit(int buildId, List<T_BuildUnit> list)
        {
            return this._Dal.BatchAddUnit(buildId, list);
        }
    }
}
