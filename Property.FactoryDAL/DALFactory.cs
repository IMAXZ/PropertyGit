using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Property.FactoryDAL
{ /// <summary>
    /// 工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DALFactory<T> where T : class
    {
        /// <summary>
        /// dal的配置地址
        /// </summary>
        public static readonly string DalPath = System.Configuration.ConfigurationManager.AppSettings["DALPath"];

        /// <summary>
        /// 创建Dal
        /// </summary>
        /// <param name="assemblyPath">命名空间路径</param>
        /// <param name="objType">类名</param>
        /// <returns></returns>
        public static object CreateDal(string assemblyPath, string objType)
        {
            var cacheDao = Assembly.Load(assemblyPath).CreateInstance(objType);
            return cacheDao;
        }

        /// <summary>
        /// 获取Dal
        /// </summary>
        /// <param name="type">类名</param>
        /// <returns></returns>
        public static T GetDAL(string type)
        {
            return (T)CreateDal(DalPath, string.Format("{0}.{1}", DalPath, type));
        }
    }
}
