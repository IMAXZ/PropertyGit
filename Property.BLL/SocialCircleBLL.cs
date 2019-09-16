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
    public class SocialCircleBLL : BaseBLL<T_SocialCircle>, ISocialCircleBLL
    {
        private const string _Type = "SocialCircleDAL";

        private ISocialCircleDAL _Dal;

        public SocialCircleBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ISocialCircleDAL;
        }

        /// <summary>
        /// 解散圈子
        /// </summary>
        /// <param name="sc">要解散的圈子实体</param>
        /// <returns></returns>
        public bool Dissolve(T_SocialCircle sc) 
        {
            return this._Dal.Dissolve(sc);
        }
    }
}
