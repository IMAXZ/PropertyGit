using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IDAL
{
    /// <summary>
    /// 业主圈子数据层访问接口
    /// </summary>
    public interface ISocialCircleDAL : IBaseDAL<T_SocialCircle>
    {
        /// <summary>
        /// 解散圈子
        /// </summary>
        /// <param name="sc">要解散的圈子实体</param>
        /// <returns></returns>
        bool Dissolve(T_SocialCircle sc);
    }
}
