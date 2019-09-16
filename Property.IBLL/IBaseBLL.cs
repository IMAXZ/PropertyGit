using Property.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 接口基类
    /// </summary>
    public interface IBaseBLL<T> where T : class
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>添加后的对象实体</returns>
        T Save(T entity);


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>是否成功</returns>
        bool Update(T entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">删除的实体</param>
        /// <returns>是否成功</returns>
        bool Delete(T entity);


        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>记录数</returns>
        int Count(Expression<Func<T, bool>> predicate);


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="anyLambda">查询表达式</param>
        /// <returns>是否存在</returns>
        bool Exist(Expression<Func<T, bool>> anyLambda);

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>数据实体</returns>
        T GetEntity(Expression<Func<T, bool>> whereLambda);


        /// <summary>
        /// 查询全部数据列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetList();

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="orderLamda">排序表达式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda, Expression<Func<T, bool>> orderLamda, bool isAsc);


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="orderName">排序名称</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetList(Expression<Func<T, bool>> whereLambda, string orderName, bool isAsc);



        /// <summary>
        /// 查找分页数据列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        IEnumerable<T> GetPageList(int pageIndex, int pageSize = ConstantParam.PAGE_SIZE);

        /// <summary>
        /// 查找分页数据列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns></returns>
        IEnumerable<T> GetPageList(Expression<Func<T, bool>> whereLambda, int pageIndex, int pageSize = ConstantParam.PAGE_SIZE);



        /// <summary>
        /// 查找分页数据列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="ordername">排序列</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> GetPageList(Expression<Func<T, bool>> whereLambda, string ordername, bool isAsc, int pageIndex, int pageSize = ConstantParam.PAGE_SIZE);


        /// <summary>
        /// 根据sql 查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        IEnumerable<T> GetList<T>(string sql);


        /// <summary>
        /// 根据sql或者存储过程进行查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        IEnumerable<T> GetList<T>(string sql, SqlParameter[] param);



        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        int ExecuteSql(string sql);
    }
}
