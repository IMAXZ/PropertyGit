using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 业主圈子业务层访问接口
    /// </summary>
    public interface ISocialCircleBLL : IBaseBLL<T_SocialCircle>
    {
        /// <summary>
        /// 解散圈子
        /// </summary>
        /// <param name="sc">要解散的圈子实体</param>
        /// <returns></returns>
        bool Dissolve(T_SocialCircle sc);
    }
}
