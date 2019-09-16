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
    /// 楼盘业务层访问类
    /// </summary>
    public class BuildBLL : BaseBLL<T_Build>, IBuildBLL
    {
        private const string _Type = "BuildDAL";

        private IBuildDAL _Dal;

        public BuildBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IBuildDAL;
        }
    }
}
