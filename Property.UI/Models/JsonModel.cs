using Property.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{

    /// <summary>
    /// Ajax请求返回的json模型
    /// </summary>
    public class JsonModel
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
        /// 操作的内容，可以将model返回转成string进行填充
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg
        {
            get {
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
        /// 默认构造函数
        /// </summary>
        public JsonModel()
        {
            //设定初始值
            this.Code = ConstantParam.JSON_RESULT_OK;
            this.Msg = string.Empty;
            
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="code">标示</param>
        /// <param name="msg">消息</param>
        public JsonModel(string code, string msg)
        {
            //设定初始值
            this.Code = code;
            this.Msg = msg;
        }


        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="code">标示</param>
        /// <param name="msg">消息</param>
        /// <param name="content">操作内容</param>
        public JsonModel(string code, string msg, string content)
        {
            //设定初始值
            this.Code = code;
            this.Msg = msg;
            this.Content = content;
        }

      
    }
}