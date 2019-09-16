using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 物业小区业务层访问接口
    /// </summary>
    public interface IPropertyPlaceBLL : IBaseBLL<T_PropertyPlace>
    {
        /// <summary>
        /// 添加小区，指定系统角色
        /// </summary>
        /// <param name="place">小区实体对象</param>
        /// <returns></returns>
        bool AddPlace(T_PropertyPlace place);

        /// <summary>
        /// 删除标识改为删除，删除系统角色
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        bool DeletePlace(T_PropertyPlace place);
    }
}
