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
    /// 试用申请业务层访问类
    /// </summary>
    public class ApplyInfoBLL : BaseBLL<T_ApplyInfo>, IApplyInfoBLL
    {
        private const string _Type = "ApplyInfoDAL";

        private IApplyInfoDAL _Dal;

        public ApplyInfoBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IApplyInfoDAL;
        }
    }
}
