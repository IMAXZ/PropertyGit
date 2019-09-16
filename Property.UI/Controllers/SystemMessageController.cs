using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 系统消息管理控制器
    /// </summary>
    public class SystemMessageController : BaseController
    {
        /// <summary>
        /// 系统消息列表
        /// </summary>
        /// <param name="model">查询模型</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "系统消息列表")]
        public ActionResult MessageList(SearchModel model)
        {
            //初始化查询条件
            Expression<Func<T_SystemMessage, bool>> where = PredicateBuilder.True<T_SystemMessage>();
            //根据消息标题模糊查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, a => a.Title.Contains(model.Kword));
            }
            //排序
            var sortModel = this.SettingSorting("id", false);
            //获取分页数据
            ISystemMessageBLL messageBll = BLLFactory<ISystemMessageBLL>.GetBLL("SystemMessageBLL");
            var data = messageBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(data);
        }

        /// <summary>
        /// 新增系统消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "新增系统消息")]
        public ActionResult AddMessage()
        {
            return View();
        }

        /// <summary>
        /// 新增系统消息提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddMessage(SystemMessageModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //模型赋值
                T_SystemMessage msg = new T_SystemMessage()
                {
                    Title = model.Title,
                    Content = model.Content,
                    CreateTime = DateTime.Now
                };
                //调用BLL层进行添加处理
                ISystemMessageBLL messageBll = BLLFactory<ISystemMessageBLL>.GetBLL("SystemMessageBLL");
                messageBll.Save(msg);
                //记录日志
                jm.Content = PropertyUtils.ModelToJsonString(model);

                //推送给所有业主客户端
                IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                var registrationIds = userPushBLL.GetList(p => p.User.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(p => p.RegistrationId).ToArray();
                bool flag = PropertyUtils.SendPush("系统消息", model.Title, ConstantParam.MOBILE_TYPE_OWNER, registrationIds);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除系统消息
        /// </summary>
        /// <param name="id">系统消息ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteMessage(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //调用BLL层获取要删除的系统消息
                ISystemMessageBLL messageBll = BLLFactory<ISystemMessageBLL>.GetBLL("SystemMessageBLL");
                var msg = messageBll.GetEntity(c => c.Id == id);

                if (msg == null)
                {
                    jm.Msg = "该系统消息不存在";

                }
                else
                {
                    //删除
                    if (messageBll.Delete(msg))
                    {
                        //记录日志
                        jm.Content = "删除系统消息 " + msg.Title;
                    }
                    else
                    {
                        jm.Msg = "删除失败";
                    }
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
