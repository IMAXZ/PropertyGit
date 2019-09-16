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
    public class GoodsCategoryBLL : BaseBLL<T_GoodsCategory>, IGoodsCategoryBLL
    {
        private const string _Type = "GoodsCategoryDAL";

        private IGoodsCategoryDAL _Dal;

        public GoodsCategoryBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IGoodsCategoryDAL;
        }
    }
}
