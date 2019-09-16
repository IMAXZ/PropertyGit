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
    public class SocialCircleMassTextingBLL : BaseBLL<T_SocialCircleMassTexting>, ISocialCircleMassTextingBLL
    {
        private const string _Type = "SocialCircleMassTextingDAL";

        private ISocialCircleMassTextingDAL _Dal;

        public SocialCircleMassTextingBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ISocialCircleMassTextingDAL;
        }
    }
}
