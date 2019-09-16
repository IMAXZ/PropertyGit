using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Property.UI.Models;
using Property.UI.Controllers;
using Property.IBLL;
using Property.FactoryBLL;
using Property.Entity;
using Property.Common;
using MvcBreadCrumbs;
using Property.BLL;
using System.IO;

namespace Property.UI.Controllers
{
    public class UserInformationController : BaseController
    {
        /// <summary>
        /// APP注册用户信息列表
        /// </summary>
        /// <param name="model">搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "APP用户列表")]
        [HttpGet]
        public ActionResult UserInformationList(SearchModel model)
        {
            //获取物业小区
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId ?? 0;
            //查询条件
            Expression<Func<T_User, bool>> where = u => (string.IsNullOrEmpty(model.Kword) ? true : (u.UserName.Contains(model.Kword) || u.Email.Contains(model.Kword)))
                && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.UserPlaces.Any(up => up.PropertyPlaceId == CurrentPlaceId);
            //排序
            var sortModel = this.SettingSorting("Id", false);

            //获取业主分页数据
            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            var list = userBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(int id)
        {
            JsonModel jm = new JsonModel();

            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            // 根据指定id值获取实体对象
            var user = userBll.GetEntity(u => u.Id == id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (user != null)
            {
                Random r = new Random();
                string newPassword = user.UserName + r.Next(100, 1000);
                user.Password = PropertyUtils.GetMD5Str(newPassword);

                // 恢复初始密码值
                userBll.Update(user);

                // 给用户发送邮件
                PropertyUtils.SendEmail(user.Email, user.UserName, "物业生活管理平台 APP用户密码重置", "您的用户密码已重置为：" + newPassword + "，请及时修改密码！");
                //操作日志
                jm.Content = "APP注册用户" + user.UserName + " 密码一键重置";
            }
            else
            {
                jm.Msg = "该用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除APP用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //获取要删除的业主
                IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User user = userBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果该业主存在
                if (user != null)
                {
                    //修改指定业主中的已删除标识
                    user.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                    if (userBll.DeleteUser(user))
                    {
                        //操作日志
                        jm.Content = "删除APP注册用户" + user.UserName;
                    }
                    else
                    {
                        jm.Msg = "删除失败";
                    }
                }
                else
                {
                    jm.Msg = "该APP注册用户不存在";
                }
            }
            catch
            {
                jm.Msg = "删除失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
