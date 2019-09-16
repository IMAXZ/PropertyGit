using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 上下文简单工厂
    /// </summary>
    public class ContextFactory
    {

        /// <summary>
        /// 获取当前数据上下文
        /// </summary>
        /// <returns></returns>
        public static PropertyDbContext GetCurrentContext()
        {
            PropertyDbContext _nContext = CallContext.GetData("PropertyDbContext") as PropertyDbContext;
            if (_nContext == null)
            {
                _nContext = new PropertyDbContext();
                CallContext.SetData("PropertyDbContext", _nContext);
            }
            return _nContext;
        }
    }
}
