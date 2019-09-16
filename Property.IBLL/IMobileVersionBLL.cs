using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// APP版本业务层访问接口
    /// </summary>
    public interface IMobileVersionBLL : IBaseBLL<T_MobileVersion>
    {
        /// <summary>
        /// 删除指定版本
        /// </summary>
        /// <param name="id">版本id</param>
        /// <returns>是否删除成功</returns>
        bool DeleteVersion(int id);
    }
}
