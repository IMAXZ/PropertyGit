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
    /// 市业务层访问类
    /// </summary>
    public class CityBLL : BaseBLL<M_City>, ICityBLL
    {
        private const string _Type = "CityDAL";

        private ICityDAL _Dal;

        public CityBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICityDAL;
        }
    }
}
