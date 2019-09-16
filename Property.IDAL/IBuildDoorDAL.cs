using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 单元户数据层访问接口
    /// </summary>
    public interface IBuildDoorDAL : IBaseDAL<T_BuildDoor>
    {
        /// <summary>
        /// 批量添加单元户
        /// </summary>
        /// <param name="unitId">单元id</param>
        /// <param name="list">单元户对象列表</param>
        /// <returns>是否分配成功</returns>
        bool BatchAddDoor(int unitId, List<T_BuildDoor> list);
    }
}
