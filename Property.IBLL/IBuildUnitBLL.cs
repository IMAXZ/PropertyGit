using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 楼座单元业务层访问接口
    /// </summary>
    public interface IBuildUnitBLL : IBaseBLL<T_BuildUnit>
    {
        /// <summary>
        /// 批量添加单元
        /// </summary>
        /// <param name="buildId">楼座id</param>
        /// <param name="list">单元对象列表</param>
        /// <returns>是否分配成功</returns>
        bool BatchAddUnit(int buildId, List<T_BuildUnit> list);
    }
}
