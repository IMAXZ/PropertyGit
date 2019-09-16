using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using MvcBreadCrumbs;
using Property.UI.Models;
using Property.IBLL;
using Property.FactoryBLL;
using Property.Entity;
using Property.Common;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 平台角色控制器
    /// </summary>
    public class PlatformRoleController : BaseController
    {

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="model">搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "角色列表")]
        [HttpGet]
        public ActionResult RoleList(RoleSearchModel model)
        {
            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");

            //查询条件
            Expression<Func<T_PlatformRole, bool>> where = u => string.IsNullOrEmpty(model.RoleName) ? true : u.RoleName.Contains(model.RoleName);

            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = platformRoleBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }

        /// <summary>
        /// 跳转到添加平台角色页面
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增角色")]
        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }

        /// <summary>
        /// 平台角色添加提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddRole(RoleModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
                //初始化平台角色数据实体
                T_PlatformRole role = new T_PlatformRole()
                {
                    RoleName = model.RoleName,
                    RoleMemo = model.RoleMemo
                };
                //保存
                platformRoleBll.Save(role);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 跳转到编辑平台角色页面
        /// </summary>
        /// <param name="id">要编辑的角色的ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑角色")]
        [HttpGet]
        public ActionResult EditRole(int id)
        {
            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //获取要编辑的角色 
            T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == id);
            if (role != null)
            {
                //初始化返回页面的模型
                RoleModel model = new RoleModel()
                {
                    RoleId = role.Id,
                    RoleName = role.RoleName,
                    RoleMemo = role.RoleMemo,
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("RoleList");
            }
        }

        /// <summary>
        /// 提交编辑平台角色表单
        /// </summary>
        /// <param name="model">要编辑的平台角色新模型</param>
        /// <returns>编辑结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditRole(RoleModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
                T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == model.RoleId);
                if (role != null)
                {
                    role.RoleName = model.RoleName;
                    role.RoleMemo = model.RoleMemo;

                    //编辑
                    if (platformRoleBll.Update(role))
                    {
                        //日志记录
                        jm.Content = PropertyUtils.ModelToJsonString(model);
                    }
                    else
                    {
                        jm.Msg = "编辑失败";
                    }
                }
                else
                {
                    jm.Msg = "该角色不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 平台角色删除
        /// </summary>
        /// <param name="id">要删除的角色的ID</param>
        /// <returns>删除结果返回</returns>
        [HttpPost]
        public JsonResult DeleteRole(int id)
        {
            JsonModel jm = new JsonModel();

            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //获取要删除的平台角色
            T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == id);
            if (role == null)
            {
                jm.Msg = "该角色不存在";
            }
            else if (role.PlatformUserRoles.Count > 0) 
            {
                jm.Msg = "有配置该角色的用户，不能删除";
            }
            else
            {
                if (platformRoleBll.Delete(role))
                {
                    //操作日志
                    jm.Content = "删除平台角色 " + role.RoleName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 平台角色分配权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "分配权限")]
        [HttpGet]
        public ActionResult ConfigAuth(int id)
        {
            RoleAuthModel model = new RoleAuthModel();

            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //获取要分配角色的平台角色
            T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == id);

            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");

            //获取所有的菜单
            var menuList = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT && m.IsPlatform == ConstantParam.USER_TYPE_PLATFORM && m.ParentId == null).OrderBy(m => m.Order).ToList();

            //Model赋值菜单列表
            model.MenuList = menuList;

            //Model赋值要分配角色的平台角色
            model.Role = new RoleModel() { RoleId = role.Id, RoleName = role.RoleName, RoleMemo = role.RoleMemo };

            //获取该角色已经有的权限ID集合
            model.ActionIds = role.PlatformRoleActions.Select(m => m.ActionId).ToList();
            return View(model);
        }

        /// <summary>
        /// 平台角色分配权限提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfigAuth(RoleConfigAuthModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
                //获取要分配角色的平台角色
                T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == model.RoleId);
                List<R_PlatformRoleAction> actions = new List<R_PlatformRoleAction>();

                //如果设置了首页权限 1:为后台首页权限ID
                if (model.ids != null && model.ids.Length > 0 && model.ids.Contains(1))
                {
                    //没有设置任何权限 则不执行循环操作
                    foreach (var id in model.ids)
                    {
                        R_PlatformRoleAction item = new R_PlatformRoleAction() { ActionId = id, RoleId = model.RoleId };
                        actions.Add(item);
                    }
                    //修改角色对应的权限组
                    if (platformRoleBll.ConfigAuth(role, actions))
                    {
                        jm.Content = "平台角色" + role.RoleName + "分配权限";
                    }
                    else
                    {
                        jm.Msg = "分配权限失败";
                    }
                }
                else
                {
                    jm.Msg = "必须分配后台管理首页权限";
                }
                
            }
            catch
            {
                jm.Msg = "分配权限失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查看平台角色权限
        /// </summary>
        /// <param name="Id">平台角色ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "查看权限")]
        [HttpGet]
        public ActionResult ViewAuth(int id) 
        {
            RoleAuthModel model = new RoleAuthModel();

            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //获取要查看权限的平台角色
            T_PlatformRole role = platformRoleBll.GetEntity(m => m.Id == id);

            //赋值 要查看权限的平台角色
            model.Role = new RoleModel() { RoleId = role.Id, RoleName = role.RoleName, RoleMemo = role.RoleMemo };

            //如果是普通角色
            if (role.IsSystem == ConstantParam.USER_ROLE_DEFAULT)
            {
                //赋值 该角色所有的权限ID集合
                model.ActionIds = role.PlatformRoleActions.Select(m => m.ActionId).ToList();

                //Model赋值 该角色所关联的非重复菜单
                var roleMenuList = role.PlatformRoleActions.Select(m => m.Action.Menu).Distinct().OrderBy(m => m.Order).ToList();

                //新定义展示Model树形菜单
                var menuList = new List<M_Menu>();

                foreach (var menu in roleMenuList)
                {
                    if (menu.ParentId != null)
                    {
                        if (!menuList.Contains(menu.ParentMenu))
                        {
                            menuList.Add(menu.ParentMenu);
                        }
                    }
                    menuList.Add(menu);
                }
                model.MenuList = menuList;
            }
            else 
            {
                IActionBLL actionBll = BLLFactory<IActionBLL>.GetBLL("ActionBLL");
                //赋值 所有的平台权限
                model.ActionIds = actionBll.GetList(a => a.Menu.IsPlatform == ConstantParam.USER_TYPE_PLATFORM).Select(a => a.Id).ToList();

                IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");
                //Model赋值 所有的平台菜单
                model.MenuList = menuBll.GetList(m => m.IsPlatform == ConstantParam.USER_TYPE_PLATFORM).ToList();
            }
            return View(model);
        }


        /// <summary>
        /// 远程验证指定平台角色名称是否存在
        /// </summary>
        /// <param name="RoleName">平台角色名称</param>
        /// <param name="RoleId">角色id,0表示新增时验证，大于0表示编辑时验证</param>
        [HttpPost]
        public ContentResult RemoteCheckExist(int roleId, string roleName)
        {
            IPlatformRoleBLL platformRoleBll = BLLFactory<IPlatformRoleBLL>.GetBLL("PlatformRoleBLL");
            //如果角色存在，验证不通过
            if (platformRoleBll.Exist(m => m.RoleName == roleName && m.Id != roleId))
            {
                return Content("false");
            }
            else//角色不存在，验证通过
            {
                return Content("true");
            }
        }
    }
}
