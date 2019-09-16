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
    public class BuildCompanyBLL : BaseBLL<T_BuildCompany>, IBuildCompanyBLL
    {
        private const string _Type = "BuildCompanyDAL";

        private IBuildCompanyDAL _Dal;

        public BuildCompanyBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IBuildCompanyDAL;
        }
    }
}
