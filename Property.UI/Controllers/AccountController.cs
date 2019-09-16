using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 整个平台的账户控制器
    /// </summary>
    public class AccountController : Controller
    {
        #region 后台登录与退出
        /// <summary>
        /// 平台后台的登陆入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult PlatformLogin()
        {
            return View();
        }


        /// <summary>
        /// 平台后台的登陆入口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult PlatformLogin(AccountModel model)
        {

            //判断提交模型数据是否正确
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string code = (string)Session["ValidateCode"];
            if (model.CheckCode != code)
            {
                ModelState.AddModelError("CheckCode", "验证码不正确");
                return View(model);
            }

            //根据用户名查找用户
            IPlatformUserBLL UserInfoBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            T_PlatformUser user = UserInfoBll.GetEntity(u => u.UserName == model.UserName.Trim()
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //1.判断用户名是否正确
            if (user == null)
            {
                ModelState.AddModelError("UserName", "用户名不存在");
                return View(model);
            }

            //2.判断密码是否正确
            string md5Str = PropertyUtils.GetMD5Str(model.Password);
            if (user.Password != md5Str)
            {
                ModelState.AddModelError("Password", "密码不正确");
                return View(model);
            }
            //3.如果未设置角色
            if (user.PlatformUserRoles.Count == 0)
            {
                ModelState.AddModelError("UserName", "该用户未设置角色，请联系管理员");
                return View(model);
            }

            //4.获取用户对象信息（拥有电站，权限菜单，Action等）保存基本信息到session中
            this.SetUserSessiong(user, UserInfoBll);

            //5.判断是否拥有访问首页的权限
            UserSessionModel session = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            if (session.IsMgr == ConstantParam.USER_ROLE_DEFAULT && !session.ActionDic.ContainsKey("/Platform/Index"))
            {
                ModelState.AddModelError("UserName", "该用户无访问权限，请联系管理员");
                return View(model);
            }
            BreadCrumb.ClearState();
            //6.跳转到
            return RedirectToAction("Index", "Platform");
        }

        /// <summary>
        /// 保存用户的session信息
        /// </summary>
        /// <param name="user"></param>
        private void SetUserSessiong(T_PlatformUser user, IPlatformUserBLL bll)
        {
            //用户session模型
            UserSessionModel sessionInfo = new UserSessionModel();

            //设置基本信息
            sessionInfo.UserID = user.Id;
            sessionInfo.UserName = user.UserName;
            sessionInfo.TrueName = user.TrueName;
            sessionInfo.IsMgr = user.IsMgr;
            sessionInfo.UserType = ConstantParam.USER_TYPE_PLATFORM;
            sessionInfo.HeadPath = user.HeadPath;

            //构造菜单业务对象
            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");


            #region 设置平台用户菜单以及权限
            //平台管理员
            if (user.IsMgr == ConstantParam.USER_ROLE_MGR)
            {
                //获取菜单
                var list = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT
                    && m.IsPlatform == ConstantParam.USER_TYPE_PLATFORM).Select(m => new MenuModel
                    {
                        MenuId = m.Id,
                        MenuName = m.MenuName,
                        MenuCode = m.MenuCode,
                        MenuUrl = m.Href,
                        MenuFlag = m.MenuFlag,
                        MenuCss = m.IconClass,
                        ParentId = m.ParentId,
                        Order = m.Order,
                        IsPlatform = m.IsPlatform

                    }).ToList();

                //设置左边菜单
                sessionInfo.MenuList = list;
            }
            else
            {
                //获取平台用户对应的角色权限表
                var roleActions = user.PlatformUserRoles.Select(ur => ur.PlatformRole.PlatformRoleActions);

                //菜单字典
                Dictionary<string, MenuModel> menuDic = new Dictionary<string, MenuModel>();
                Dictionary<string, string> actionDic = new Dictionary<string, string>();

                foreach (var item in roleActions)
                {
                    var actions = item.Select(obj => obj.Action);
                    foreach (var action in actions)
                    {
                        //添加权限
                        if (!actionDic.ContainsKey(action.Href))
                        {
                            actionDic.Add(action.Href, action.ActionName);
                        }

                        foreach (var li in action.ActionItems)
                        {
                            //添加权限
                            if (!actionDic.ContainsKey(li.Href))
                            {
                                actionDic.Add(li.Href, li.ItemName);
                            }
                        }

                        var menu = action.Menu;
                        if (menu.ParentId != null)
                        {
                            if (!menuDic.ContainsKey(menu.ParentMenu.MenuCode))
                            {
                                menuDic.Add(menu.ParentMenu.MenuCode, GetMenuModel(menu.ParentMenu));
                            }
                        }
                        if (!menuDic.ContainsKey(menu.MenuCode))
                        {
                            menuDic.Add(menu.MenuCode, GetMenuModel(menu));
                        }
                    }
                }

                //设置菜单和权限
                sessionInfo.MenuList.AddRange(menuDic.Values.ToList());
                sessionInfo.ActionDic = actionDic;
            }
            #endregion

            //设置session信息
            Session[ConstantParam.SESSION_USERINFO] = sessionInfo;
        }

        /// <summary>
        /// 转到物业平台
        /// </summary>
        /// <param name="id">小区ID</param>
        /// <returns></returns>
        public ActionResult GotoProperty(int id)
        {
            //获取session对象
            UserSessionModel model = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];

            //构造菜单业务对象
            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");
            //获取菜单
            var list = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT
                && m.IsPlatform == ConstantParam.USER_TYPE_PROPERTY).Select(m => new MenuModel
                {
                    MenuId = m.Id,
                    MenuName = m.MenuName,
                    MenuCode = m.MenuCode,
                    MenuUrl = m.Href,
                    MenuFlag = m.MenuFlag,
                    MenuCss = m.IconClass,
                    ParentId = m.ParentId,
                    Order = m.Order,
                    IsPlatform = m.IsPlatform

                }).ToList();

            if (model.PropertyPlaceId == null)
            {
                //设置左边菜单
                model.MenuList.AddRange(list);
            }
            //设置当前小区
            model.PropertyPlaceId = id;

            //设置session信息
            Session[ConstantParam.SESSION_USERINFO] = model;
            BreadCrumb.ClearState();

            return RedirectToAction("Index", "Property");
        }

        /// <summary>
        /// 平台退出系统
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PlatformLogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            BreadCrumb.ClearState();
            return RedirectToAction("PlatformLogin", "Account");
        }

        #endregion

        #region 物业平台登录与退出

        /// <summary>
        /// 跳转到物业平台登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult PropertyLogin()
        {
            return View();
        }


        /// <summary>
        /// 物业平台登录提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult PropertyLogin(AccountModel model)
        {
            //判断提交模型数据是否正确
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string code = (string)Session["ValidateCode"];
            if (model.CheckCode != code)
            {
                ModelState.AddModelError("CheckCode", "验证码不正确");
                return View(model);
            }

            //根据用户名查找用户
            IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            T_PropertyUser user = propertyUserBll.GetEntity(u => u.UserName == model.UserName.Trim()
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //1.判断用户名是否正确
            if (user == null)
            {
                ModelState.AddModelError("UserName", "用户名不存在");
                return View(model);
            }

            //2.判断密码是否正确
            string md5Str = PropertyUtils.GetMD5Str(model.Password);
            if (user.Password != md5Str)
            {
                ModelState.AddModelError("Password", "密码不正确");
                return View(model);
            }

            //3.如果未设置角色
            if (user.PropertyUserRoles.Count == 0)
            {
                ModelState.AddModelError("UserName", "该用户未设置角色，请联系管理员");
                return View(model);
            }
            //4.获取用户对象信息（权限菜单，Action等）保存基本信息到session中
            this.SetUserSessiong(user, propertyUserBll);

            //5.判断是否拥有访问首页的权限
            UserSessionModel session = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            if (session.IsMgr == ConstantParam.USER_ROLE_DEFAULT && !session.ActionDic.ContainsKey("/Property/Index"))
            {
                ModelState.AddModelError("UserName", "该用户无访问权限，请联系管理员");
                return View(model);
            }
            BreadCrumb.ClearState();
            //5.跳转到
            return RedirectToAction("Index", "Property");
        }


        /// <summary>
        /// 保存物业用户的session信息
        /// </summary>
        /// <param name="user"></param>
        private void SetUserSessiong(T_PropertyUser user, IPropertyUserBLL bll)
        {
            //用户session模型
            UserSessionModel sessionInfo = new UserSessionModel();

            //设置基本信息
            sessionInfo.UserID = user.Id;
            sessionInfo.UserName = user.UserName;
            sessionInfo.TrueName = user.TrueName;
            sessionInfo.IsMgr = user.IsMgr;
            sessionInfo.UserType = ConstantParam.USER_TYPE_PROPERTY;
            sessionInfo.PropertyPlaceId = user.PropertyPlaceId;
            sessionInfo.HeadPath = user.HeadPath;

            //构造菜单业务对象
            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");

            #region 设置物业用户菜单以及权限

            //管理员
            if (user.IsMgr == ConstantParam.USER_ROLE_MGR)
            {
                //获取菜单
                var list = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT
                    && m.IsPlatform == ConstantParam.USER_TYPE_PROPERTY).Select(m => new MenuModel
                {
                    MenuId = m.Id,
                    MenuName = m.MenuName,
                    MenuCode = m.MenuCode,
                    MenuUrl = m.Href,
                    MenuFlag = m.MenuFlag,
                    MenuCss = m.IconClass,
                    ParentId = m.ParentId,
                    Order = m.Order,
                    IsPlatform = m.IsPlatform

                }).ToList();

                //设置左边菜单
                sessionInfo.MenuList = list;
            }
            else
            {
                //获取物业用户对应的角色权限表
                var roleActions = user.PropertyUserRoles.Select(ur => ur.PropertyRole.PropertyRoleActions);

                //菜单字典
                Dictionary<string, MenuModel> menuDic = new Dictionary<string, MenuModel>();
                //权限字典
                Dictionary<string, string> actionDic = new Dictionary<string, string>();

                foreach (var item in roleActions)
                {
                    var actions = item.Select(obj => obj.Action);
                    foreach (var action in actions)
                    {
                        //添加权限
                        if (!actionDic.ContainsKey(action.Href))
                        {
                            actionDic.Add(action.Href, action.ActionName);
                        }

                        foreach (var li in action.ActionItems)
                        {
                            //添加权限
                            if (!actionDic.ContainsKey(li.Href))
                            {
                                actionDic.Add(li.Href, li.ItemName);
                            }
                        }

                        var menu = action.Menu;
                        if (menu.ParentId != null)
                        {
                            if (!menuDic.ContainsKey(menu.ParentMenu.MenuCode))
                            {
                                menuDic.Add(menu.ParentMenu.MenuCode, GetMenuModel(menu.ParentMenu));
                            }
                        }
                        if (!menuDic.ContainsKey(menu.MenuCode))
                        {
                            menuDic.Add(menu.MenuCode, GetMenuModel(menu));
                        }
                    }
                }
                //设置菜单和权限
                sessionInfo.MenuList.AddRange(menuDic.Values.ToList());
                sessionInfo.ActionDic = actionDic;
            }
            #endregion

            //设置session信息
            Session[ConstantParam.SESSION_USERINFO] = sessionInfo;
        }

        /// <summary>
        /// 物业退出系统
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PropertyLogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            BreadCrumb.ClearState();
            return RedirectToAction("PropertyLogin", "Account");
        }
        #endregion
        
        #region 门店平台登录与退出

        /// <summary>
        /// 跳转到门店平台登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ShopPlatformLogin()
        {
            return View();
        }


        /// <summary>
        /// 门店平台登录提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ShopPlatformLogin(AccountModel model)
        {
            //判断提交模型数据是否正确
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string code = (string)Session["ValidateCode"];
            if (model.CheckCode != code)
            {
                ModelState.AddModelError("CheckCode", "验证码不正确");
                return View(model);
            }

            //根据用户名查找门店平台用户
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            T_ShopUser user = shopUserBll.GetEntity(u => u.UserName == model.UserName.Trim()
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //1.判断用户名是否正确
            if (user == null)
            {
                ModelState.AddModelError("UserName", "用户名不存在");
                return View(model);
            }

            //2.判断密码是否正确
            string md5Str = PropertyUtils.GetMD5Str(model.Password);
            if (user.Password != md5Str)
            {
                ModelState.AddModelError("Password", "密码不正确");
                return View(model);
            }

            //3.保存基本信息到session中
            this.SetUserSessiong(user, shopUserBll);
            BreadCrumb.ClearState();

            //4.跳转到
            return RedirectToAction("Index", "ShopPlatform");
        }

        /// <summary>
        /// 保存门店用户的session信息
        /// </summary>
        /// <param name="user">登录的时候取出数据再赋值</param>
        private void SetUserSessiong(T_ShopUser user, IShopUserBLL bll)
        {
            //用户session模型
            UserSessionModel sessionInfo = new UserSessionModel();

            //设置基本信息
            sessionInfo.UserID = user.Id;
            sessionInfo.UserName = user.UserName;
            sessionInfo.TrueName = user.TrueName;
            sessionInfo.UserType = ConstantParam.USER_TYPE_SHOP;
            sessionInfo.HeadPath = user.HeadPath;
            //设置session信息
            Session[ConstantParam.SESSION_USERINFO] = sessionInfo;
        }

        /// <summary>
        /// 门店平台退出系统
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShopPlatformLogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            BreadCrumb.ClearState();
            return RedirectToAction("ShopPlatformLogin", "Account");
        }
        #endregion

        #region 总公司平台登录与退出

        /// <summary>
        /// 跳转到总公司平台登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CompanyPlatformLogin()
        {
            return View();
        }


        /// <summary>
        /// 物业总公司平台登录提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CompanyPlatformLogin(AccountModel model)
        {
            //判断提交模型数据是否正确
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string code = (string)Session["ValidateCode"];
            if (model.CheckCode != code)
            {
                ModelState.AddModelError("CheckCode", "验证码不正确");
                return View(model);
            }

            //根据用户名查找用户
            ICompanyUserBLL companyUserBll = BLLFactory<ICompanyUserBLL>.GetBLL("CompanyUserBLL");
            T_CompanyUser user = companyUserBll.GetEntity(u => u.UserName == model.UserName.Trim()
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //1.判断用户名是否正确
            if (user == null)
            {
                ModelState.AddModelError("UserName", "用户名不存在");
                return View(model);
            }

            //2.判断密码是否正确
            string md5Str = PropertyUtils.GetMD5Str(model.Password);
            if (user.Password != md5Str)
            {
                ModelState.AddModelError("Password", "密码不正确");
                return View(model);
            }

            //3.如果未设置角色
            if (user.CompanyUserRoles.Count == 0)
            {
                ModelState.AddModelError("UserName", "该用户未设置角色，请联系管理员");
                return View(model);
            }
            //4.获取用户对象信息（权限菜单，Action等）保存基本信息到session中
            this.SetUserSessiong(user, companyUserBll);

            //5.判断是否拥有访问首页的权限
            UserSessionModel session = (UserSessionModel)Session[ConstantParam.SESSION_USERINFO];
            if (session.IsMgr == ConstantParam.USER_ROLE_DEFAULT && !session.ActionDic.ContainsKey("/CompanyPlatform/Index"))
            {
                ModelState.AddModelError("UserName", "该用户无访问权限，请联系管理员");
                return View(model);
            }
            BreadCrumb.ClearState();
            //5.跳转到
            return RedirectToAction("Index", "CompanyPlatform");
        }


        /// <summary>
        /// 保存总公司用户的session信息
        /// </summary>
        /// <param name="user"></param>
        private void SetUserSessiong(T_CompanyUser user, ICompanyUserBLL bll)
        {
            //用户session模型
            UserSessionModel sessionInfo = new UserSessionModel();

            //设置基本信息
            sessionInfo.UserID = user.Id;
            sessionInfo.UserName = user.UserName;
            sessionInfo.TrueName = user.TrueName;
            sessionInfo.IsMgr = user.IsMgr;
            sessionInfo.UserType = ConstantParam.USER_TYPE_COMPANY;
            sessionInfo.CompanyId = user.CompanyId;
            sessionInfo.HeadPath = user.HeadPath;

            //构造菜单业务对象
            IMenuBLL menuBll = BLLFactory<IMenuBLL>.GetBLL("MenuBLL");

            #region 设置总公司用户菜单以及权限

            //管理员
            if (user.IsMgr == ConstantParam.USER_ROLE_MGR)
            {
                //获取菜单
                var list = menuBll.GetList(m => m.MenuFlag == ConstantParam.MENU_LEFT
                    && m.IsPlatform == ConstantParam.USER_TYPE_COMPANY).Select(m => new MenuModel
                    {
                        MenuId = m.Id,
                        MenuName = m.MenuName,
                        MenuCode = m.MenuCode,
                        MenuUrl = m.Href,
                        MenuFlag = m.MenuFlag,
                        MenuCss = m.IconClass,
                        ParentId = m.ParentId,
                        Order = m.Order,
                        IsPlatform = m.IsPlatform

                    }).ToList();

                //设置左边菜单
                sessionInfo.MenuList = list;
            }
            else
            {
                //获取总公司用户对应的角色权限表
                var roleActions = user.CompanyUserRoles.Select(ur => ur.CompanyRole.CompanyRoleActions);

                //菜单字典
                Dictionary<string, MenuModel> menuDic = new Dictionary<string, MenuModel>();
                //权限字典
                Dictionary<string, string> actionDic = new Dictionary<string, string>();

                foreach (var item in roleActions)
                {
                    var actions = item.Select(obj => obj.Action);
                    foreach (var action in actions)
                    {
                        //添加权限
                        if (!actionDic.ContainsKey(action.Href))
                        {
                            actionDic.Add(action.Href, action.ActionName);
                        }

                        foreach (var li in action.ActionItems)
                        {
                            //添加权限
                            if (!actionDic.ContainsKey(li.Href))
                            {
                                actionDic.Add(li.Href, li.ItemName);
                            }
                        }

                        var menu = action.Menu;
                        if (menu.ParentId != null)
                        {
                            if (!menuDic.ContainsKey(menu.ParentMenu.MenuCode))
                            {
                                menuDic.Add(menu.ParentMenu.MenuCode, GetMenuModel(menu.ParentMenu));
                            }
                        }
                        if (!menuDic.ContainsKey(menu.MenuCode))
                        {
                            menuDic.Add(menu.MenuCode, GetMenuModel(menu));
                        }
                    }
                }
                //设置菜单和权限
                sessionInfo.MenuList.AddRange(menuDic.Values.ToList());
                sessionInfo.ActionDic = actionDic;
            }
            #endregion

            //设置session信息
            Session[ConstantParam.SESSION_USERINFO] = sessionInfo;
        }

        /// <summary>
        /// 总公司平台退出系统
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CompanyPlatformLogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            BreadCrumb.ClearState();
            return RedirectToAction("CompanyPlatformLogin", "Account");
        }
        #endregion

        /// <summary>
        /// 根据菜单实体对象获取菜单model
        /// </summary>
        /// <param name="menu">菜单实体对象</param>
        /// <returns></returns>
        private MenuModel GetMenuModel(M_Menu m)
        {
            MenuModel model = new MenuModel();
            model.MenuId = m.Id;
            model.MenuName = m.MenuName;
            model.MenuCode = m.MenuCode;
            model.MenuUrl = m.Href;
            model.MenuFlag = m.MenuFlag;
            model.MenuCss = m.IconClass;
            model.ParentId = m.ParentId;
            model.Order = m.Order;
            model.IsPlatform = m.IsPlatform;
            return model;
        }

        #region 验证码相关

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            string code = PropertyUtils.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = this.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateNum">验证码</param>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 25);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        #endregion
    }
}