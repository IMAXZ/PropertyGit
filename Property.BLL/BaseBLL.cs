using Property.FactoryDAL;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.BLL
{
     /// <summary>
    /// 业务处理基类
    /// </summary>
    public abstract class BaseBLL<T> : IBaseBLL<T> where T : class
    {
        /// <summary>
        /// 数据操作接口
        /// </summary>
        protected IBaseDAL<T> CurrentDAL { get; set; }


        /// <summary>
        /// 重载的构造函数
        /// </summary>
        /// <param name="type">类名</param>
        public BaseBLL(string type)
        {
            this.CurrentDAL = DALFactory<IBaseDAL<T>>.GetDAL(type);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>保存后的对象</returns>
        public T Save(T entity)
        {
            return this.CurrentDAL.Save(entity);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>是否更新成功</returns>
        public bool Update(T entity)
        {
            return this.CurrentDAL.Update(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(T entity)
        {
            return this.CurrentDAL.Delete(entity);
        }


        /// <summary>
        /// 记录数
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>个数</returns>
        public int Count(Expression<Func<T, bool>> predicate)
        {
            return this.CurrentDAL.Count(predicate);
        }


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="anyLambda">条件表达式</param>
        /// <returns>是否存在</returns>
        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            return this.CurrentDAL.Exist(anyLambda);
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            return this.CurrentDAL.GetEntity(whereLambda);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList()
        {
            return this.CurrentDAL.GetList();
        }


        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            return this.CurrentDAL.GetList(whereLambda);
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
            return this.CurrentDAL.GetList(whereLambda, orderLamda, isAsc);
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
            return this.CurrentDAL.GetList(whereLambda, orderName, isAsc);
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
            return this.CurrentDAL.GetPageList(whereLambda, pageIndex, pageSize);
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
            return this.CurrentDAL.GetPageList(whereLambda, orderName, isAsc, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据sql 查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string sql)
        {
            return this.CurrentDAL.GetList<T>(sql);
        }


        /// <summary>
        /// 根据sql或者存储过程进行查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string sql, SqlParameter[] param)
        {
            return CurrentDAL.GetList<T>(sql, param);
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            return this.CurrentDAL.ExecuteSql(sql);
        }
    }
}
