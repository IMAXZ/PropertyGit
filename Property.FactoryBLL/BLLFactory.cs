using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Property.FactoryBLL
{
    /// <summary>
    /// 工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BLLFactory<T> where T : class
    {
        /// <summary>
        /// BLL所配置的业务路径
        /// </summary>
        public static readonly string BllPath = System.Configuration.ConfigurationManager.AppSettings["BLLPath"];

        /// <summary>
        /// 通过反射创建对象，并把对象放在缓存中
        /// </summary>
        /// <param name="assemblyPath">命名空间</param>
        /// <param name="objType">实现类的全路径</param>
        /// <returns></returns>
        private static object CreateBLL(string assemblyPath, string objType)
        {
            var cacheBll = Assembly.Load(assemblyPath).CreateInstance(objType);
            return cacheBll;
        }

        /// <summary>
        /// 工厂方法获取反射的实体类
        /// </summary>
        /// <param name="type">类的名字</param>
        /// <returns></returns>
        public static T GetBLL(string type)
        {
            return (T)CreateBLL(BllPath, string.Format("{0}.{1}", BllPath, type));
        }

    }
}
