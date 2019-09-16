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
    /// 平台操作日志业务层访问类
    /// </summary>
    public class PlatformOpreateLogBLL : BaseBLL<T_PlatformOpreateLog>, IPlatformOpreateLogBLL
    {
        private const string _Type = "PlatformOpreateLogDAL";

        private IPlatformOpreateLogDAL _Dal;

        public PlatformOpreateLogBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPlatformOpreateLogDAL;
        }
    }
}
