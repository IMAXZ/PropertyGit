﻿using Property.Entity;
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
    /// 物业操作日志业务层访问类
    /// </summary>
    public class PropertyOpreateLogBLL : BaseBLL<T_PropertyOpreateLog>, IPropertyOpreateLogBLL
    {
        private const string _Type = "PropertyOpreateLogDAL";

        private IPropertyOpreateLogDAL _Dal;

        public PropertyOpreateLogBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPropertyOpreateLogDAL;
        }
    }
}
