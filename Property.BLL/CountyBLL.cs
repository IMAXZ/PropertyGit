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
    /// 县区业务层访问类
    /// </summary>
    public class CountyBLL : BaseBLL<M_County>, ICountyBLL
    {
        private const string _Type = "CountyDAL";

        private ICountyDAL _Dal;

        public CountyBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as ICountyDAL;
        }
    }
}
