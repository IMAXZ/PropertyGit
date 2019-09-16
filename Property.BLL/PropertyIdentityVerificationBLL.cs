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
    /// 审批业主信息 业务层访问类
    /// </summary>
    public class PropertyIdentityVerificationBLL : BaseBLL<R_PropertyIdentityVerification>, IPropertyIdentityVerificationBLL
    {
        private const string _Type = "PropertyIdentityVerificationDAL";

        private IPropertyIdentityVerificationDAL _DAL;

        public PropertyIdentityVerificationBLL()
            : base(_Type)
        {
            this._DAL = base.CurrentDAL as IPropertyIdentityVerificationDAL;
        }
    }
}
