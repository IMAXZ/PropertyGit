using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 物业小区数据层访问类
    /// </summary>
    public class PropertyPlaceDAL : BaseDAL<T_PropertyPlace>, IPropertyPlaceDAL
    {
        /// <summary>
        /// 添加小区，指定系统角色
        /// </summary>
        /// <param name="place">小区实体对象</param>
        /// <returns></returns>
        public bool AddPlace(T_PropertyPlace place) 
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //添加小区
                    var newPlace = base.Save(place);
                    if (newPlace != null)
                    {
                        //给小区设定系统角色
                        IPropertyRoleBLL propertyRoleBll = BLLFactory<IPropertyRoleBLL>.GetBLL("PropertyRoleBLL");
                        //初始化物业系统角色数据实体
                        T_PropertyRole role = new T_PropertyRole()
                        {
                            RoleName = ConstantParam.SYSTEM_ROLE_NAME,
                            RoleMemo = ConstantParam.SYSTEM_ROLE_MEMO,
                            PropertyPlaceId = newPlace.Id,
                            IsSystem = ConstantParam.USER_ROLE_MGR
                        };
                        //保存
                        propertyRoleBll.Save(role);

                        //提交事务
                        tran.Commit();
                    }
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 删除小区，删除其角色和用户
        /// </summary>
        /// <param name="place">小区实体对象</param>
        /// <returns></returns>
        public bool DeletePlace(T_PropertyPlace place)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //修改用户为删除标识
                    IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                    var users = propertyUserBll.GetList(u => u.PropertyPlaceId == place.Id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList();
                    foreach (var user in users) 
                    {
                        user.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                        propertyUserBll.Update(user);
                    }

                    //改为删除标识
                    base.Update(place);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
