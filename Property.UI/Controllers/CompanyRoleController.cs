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
    public class CompanyRoleController : BaseController
    {
        /// <summary>
        /// 总公司角色列表
        /// </summary>
        /// <param name="model">搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "总公司角色列表")]
        [HttpGet]
        public ActionResult RoleList(RoleSearchModel model)
        {
            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //查询条件
            Expression<Func<T_CompanyRole, bool>> where = u => string.IsNullOrEmpty(model.RoleName) ? true : u.RoleName.Contains(model.RoleName);
            //获取当前用户所属总公司ID
            int currentCompanyId = GetSessionModel().CompanyId.Value;
            //查询条件：当前用户所在总公司
            where = where.And(u => u.CompanyId == currentCompanyId);
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = companyRoleBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(list);
        }

        /// <summary>
        /// 跳转到添加总公司角色页面                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增总公司角色")]
        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }

        /// <summary>
        /// 总公司角色添加提交
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
                ICompanyRoleBLL propertyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
                //初始化平台角色数据实体
                T_CompanyRole role = new T_CompanyRole()
                {
                    RoleName = model.RoleName,
                    RoleMemo = model.RoleMemo,
                    CompanyId = GetSessionModel().CompanyId.Value
                };
                //保存
                propertyRoleBll.Save(role);

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
        /// 跳转到编辑总公司角色页面
        /// </summary>
        /// <param name="id">要编辑的角色的ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑总公司角色")]
        [HttpGet]
        public ActionResult EditRole(int id)
        {
            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //获取要编辑的总公司角色 
            T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == id);

            if (role != null)
            {
                //初始化返回页面的模型
                RoleModel model = new RoleModel()
                {
                    RoleId = role.Id,
                    RoleName = role.RoleName,
                    RoleMemo = role.RoleMemo
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("RoleList");
            }
        }

        /// <summary>
        /// 提交编辑总公司角色表单
        /// </summary>
        /// <param name="model">要编辑的总公司角色新模型</param>
        /// <returns>编辑结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditRole(RoleModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
                T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == model.RoleId);
                if (role != null)
                {
                    role.RoleName = model.RoleName;
                    role.RoleMemo = model.RoleMemo;

                    //编辑
                    if (companyRoleBll.Update(role))
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
        /// 总公司角色删除
        /// </summary>
        /// <param name="id">要删除的角色的ID</param>
        /// <returns>删除结果返回</returns>
        [HttpPost]
        public JsonResult DeleteRole(int id)
        {
            JsonModel jm = new JsonModel();

            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //获取要删除的总公司角色
            T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == id);
            if (role == null)
            {
                jm.Msg = "该角色不存在";
            }
            else if (role.CompanyUserRoles.Count > 0)
            {
                jm.Msg = "有配置该角色的用户，不能删除";
            }
            else
            {
                if (companyRoleBll.Delete(role))
                {
                    //操作日志
                    jm.Content = "删除总公司角色 " + role.RoleName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 总公司角色分配权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "分配权限")]
        [HttpGet]
        public ActionResult ConfigAuth(int id)
        {
            RoleAuthModel model = new RoleAuthModel();

            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //获取要分配角色的物业角色
            T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == id);

            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");

            //获取所有的菜单
            var menuList = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT && m.IsPlatform == ConstantParam.USER_TYPE_COMPANY && m.ParentId == null).OrderBy(m => m.Order).ToList();

            //Model赋值菜单列表
            model.MenuList = menuList;

            //Model赋值要分配角色的总公司角色
            model.Role = new RoleModel() { RoleId = role.Id, RoleName = role.RoleName, RoleMemo = role.RoleMemo };

            //获取该角色已经有的权限ID集合
            model.ActionIds = role.CompanyRoleActions.Select(m => m.ActionId).ToList();

            return View(model);
        }

        /// <summary>
        /// 总公司角色分配权限提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfigAuth(RoleConfigAuthModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
                //获取要分配角色的总公司角色
                T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == model.RoleId);
                List<R_CompanyRoleAction> actions = new List<R_CompanyRoleAction>();

                //如果设置了首页权限 230:为总公司平台首页权限ID
                if (model.ids != null && model.ids.Length > 0 && model.ids.Contains(230))
                {
                    //没有设置任何权限 则不执行循环操作
                    foreach (var id in model.ids)
                    {
                        R_CompanyRoleAction item = new R_CompanyRoleAction() { ActionId = id, RoleId = model.RoleId };
                        actions.Add(item);
                    }
                    //修改角色对应的权限组
                    if (companyRoleBll.ConfigAuth(role, actions))
                    {
                        jm.Content = "总公司角色" + role.RoleName + "分配权限";
                    }
                    else
                    {
                        jm.Msg = "分配权限失败";
                    }
                }
                else
                {
                    jm.Msg = "必须分配总公司平台首页权限";
                }
            }
            catch
            {
                jm.Msg = "分配权限失败";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看总公司角色权限
        /// </summary>
        /// <param name="Id">物业角色ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "查看权限")]
        [HttpGet]
        public ActionResult ViewAuth(int id)
        {
            RoleAuthModel model = new RoleAuthModel();

            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //获取要查看权限的总公司角色
            T_CompanyRole role = companyRoleBll.GetEntity(m => m.Id == id);

            //赋值 要查看权限的总公司角色
            model.Role = new RoleModel() { RoleId = role.Id, RoleName = role.RoleName, RoleMemo = role.RoleMemo };

            //如果是普通角色
            if (role.IsSystem == ConstantParam.USER_ROLE_DEFAULT)
            {
                //赋值 该角色所有的权限ID集合
                model.ActionIds = role.CompanyRoleActions.Select(m => m.ActionId).ToList();

                //Model赋值 该角色所关联的非重复菜单
                var roleMenuList = role.CompanyRoleActions.Select(m => m.Action.Menu).Distinct().OrderBy(m => m.Order).ToList();
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
                //赋值 所有的总公司权限
                model.ActionIds = actionBll.GetList(a => a.Menu.IsPlatform == ConstantParam.USER_TYPE_COMPANY).Select(a => a.Id).ToList();

                IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");
                //Model赋值 所有的平台菜单
                model.MenuList = menuBll.GetList(m => m.IsPlatform == ConstantParam.USER_TYPE_COMPANY).ToList();
            }

            return View(model);
        }

        /// <summary>
        /// 远程验证指定总公司角色名称是否存在
        /// </summary>
        /// <param name="RoleName">物业角色名称</param>
        /// <param name="RoleId">角色id,0表示新增时验证，大于0表示编辑时验证</param>
        [HttpPost]
        public ContentResult RemoteCheckExist(int roleId, string roleName)
        {
            ICompanyRoleBLL companyRoleBll = BLLFactory<ICompanyRoleBLL>.GetBLL("CompanyRoleBLL");
            //获取当前用户所属总公司ID
            int currentCompanyId = GetSessionModel().CompanyId.Value;
            //如果角色存在，验证不通过
            if (companyRoleBll.Exist(m => m.RoleName == roleName && m.CompanyId == currentCompanyId && m.Id != roleId))
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
