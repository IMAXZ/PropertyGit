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
    /// 业主圈子聊天记录业务层访问类
    /// </summary>
    public class SocialCircleChatBLL : BaseBLL<T_SocialCircleChat>, ISocialCircleChatBLL
    {
        private const string _Type = "SocialCircleChatDAL";

        private ISocialCircleChatDAL _Dal;

        public SocialCircleChatBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ISocialCircleChatDAL;
        }
    }
}
