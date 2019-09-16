using Property.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{
    public class ApiPayResultModel
    {
          /// <summary>
        /// 返回标示
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        private string _Msg;

        /// <summary>
        /// 总支出
        /// </summary>
        public double TotalPay { get; set; }

        /// <summary>
        /// 月支出
        /// </summary>
        public double MonthPay { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 返回错误消息
        /// </summary>
        public string Msg
        {
            get
            {
                return _Msg;
            }
            set
            {
                this._Msg = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.Code = ConstantParam.JSON_RESULT_NO;
                }
            }
        }

        /// <summary>
        /// 成功返回数据
        /// </summary>
        public object result { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ApiPayResultModel()
        {
            //设定初始值
            this.Code = ConstantParam.JSON_RESULT_OK;
        }
    }
}