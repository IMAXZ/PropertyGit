using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;
namespace Property.DAL
{
     /// <summary>
    /// 仓储基类
    /// </summary>
    public class BaseDAL<T> : IBaseDAL<T> where T : class
    {

        /// <summary>
        /// 上下文
        /// </summary>
        protected PropertyDbContext nContext = ContextFactory.GetCurrentContext();


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Save(T entity)
        {
            nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
            nContext.SaveChanges();
            return entity;
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            nContext.Set<T>().Attach(entity);
            nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            return nContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            nContext.Set<T>().Attach(entity);
            //nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            nContext.Set<T>().Remove(entity);
            return nContext.SaveChanges() > 0;
        }


        /// <summary>
        /// 记录数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> predicate)
        {
            return nContext.Set<T>().Count(predicate);
        }


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="anyLambda"></param>
        /// <returns></returns>
        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            return nContext.Set<T>().Any(anyLambda);
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            T _entity = nContext.Set<T>().FirstOrDefault<T>(whereLambda);
            return _entity;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList()
        {
            var _list = nContext.Set<T>();
            return _list;
        }


        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            var _list = nContext.Set<T>().Where<T>(whereLambda);
            return _list;
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="orderLamda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda, Expression<Func<T, bool>> orderLamda, bool isAsc)
        {
            var _list = nContext.Set<T>().Where<T>(whereLambda);
            _list = isAsc ? _list.OrderBy(orderLamda) : _list.OrderByDescending(orderLamda);
            return _list;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="orderName"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda, string orderName, bool isAsc)
        {
            var _list = nContext.Set<T>().Where<T>(whereLambda);
            _list = OrderBy(_list, orderName, isAsc);
            return _list;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(int pageIndex, int pageSize)
        {
            return GetPageList(lambda => true, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(Expression<Func<T, bool>> whereLambda, int pageIndex, int pageSize)
        {
            var _list = nContext.Set<T>().Where<T>(whereLambda).OrderBy(t => true).ToPagedList(pageIndex, pageSize);
            return _list;
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderName">排序列(必须填写)</param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(Expression<Func<T, bool>> whereLambda, string orderName, bool isAsc, int pageIndex, int pageSize)
        {
            var _list = nContext.Set<T>().Where<T>(whereLambda);
            var list = OrderBy(_list, orderName, isAsc).ToPagedList(pageIndex, pageSize);
            return list;
        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">原IQueryable</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns>排序后的IQueryable<T></returns>
        private IQueryable<T> OrderBy(IQueryable<T> source, string propertyName, bool isAsc)
        {
            if (source == null) throw new ArgumentNullException("source", "不能为空");
            if (string.IsNullOrEmpty(propertyName)) return source;
            var _parameter = Expression.Parameter(source.ElementType);
            var _property = Expression.Property(_parameter, propertyName);
            if (_property == null) throw new ArgumentNullException("propertyName", "属性不存在");
            var _lambda = Expression.Lambda(_property, _parameter);
            var _methodName = isAsc ? "OrderBy" : "OrderByDescending";
            var _resultExpression = Expression.Call(typeof(Queryable), _methodName, new Type[] { source.ElementType, _property.Type }, source.Expression, Expression.Quote(_lambda));
            return source.Provider.CreateQuery<T>(_resultExpression);
        }

        /// <summary>
        /// 根据sql 查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string sql)
        {
            return nContext.Database.SqlQuery<T>(sql);
        }

        /// <summary>
        /// 根据sql或者存储过程进行查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string sql, SqlParameter[] param)
        {
            return nContext.Database.SqlQuery<T>(sql, param);
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            return nContext.Database.ExecuteSqlCommand(sql);

        }
    }
}
