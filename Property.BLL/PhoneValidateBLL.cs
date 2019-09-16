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
    /// 手机验证记录业务层访问类
    /// </summary>
    public class PhoneValidateBLL : BaseBLL<T_PhoneValidate>, IPhoneValidateBLL
    {
        private const string _Type = "PhoneValidateDAL";

        private IPhoneValidateDAL _Dal;

        public PhoneValidateBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPhoneValidateDAL;
        }
    }
}
