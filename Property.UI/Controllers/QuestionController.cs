using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Property.IBLL;
using Property.FactoryBLL;
using Property.Entity;
using Property.Common;
using Property.UI.Models;
using MvcBreadCrumbs;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 业主上报问题管理控制器
    /// </summary>
    public class QuestionController : BaseController
    {
        #region 业主上报问题列表

        /// <summary>
        /// 业主上报问题列表
        /// </summary> 
        /// <param name="model">搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "上报问题列表")]
        [HttpGet]
        public ActionResult QuestionList(QuestionSearchModel model)
        {

            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;
            model.StatusList = GetStatusList();

            int CurrentPlaceId = GetSessionModel().PropertyPlaceId ?? 0;

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);
            Expression<Func<T_Question, bool>> where = u => u.UploadTime >= model.StartTime.Value && u.UploadTime < endTime && u.PropertyPlaceId == CurrentPlaceId;

            //根据状态名称查询
            if (model.Status != null)
            {
                where = PredicateBuilder.And(where, u => u.Status == model.Status);
            }

            //根据问题名称模糊查询
            if (!string.IsNullOrEmpty(model.Title))
            {
                where = PredicateBuilder.And(where, u => u.Title.Contains(model.Title));
            }

            //根据查询条件调用BLL层 获取分页数据
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            var sortName = this.SettingSorting("Id", false);
            model.DataList = questionBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<T_Question>;
            return View(model);
        }

        /// <summary>
        /// 删除业主上报问题
        /// </summary>
        /// <param name="id">删除问题的提报人Id</param>
        /// <returns>删除结果返回</returns>
        [HttpPost]
        public JsonResult DeleteQuestion(int id)
        {
            JsonModel jm = new JsonModel();
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            T_Question question = questionBll.GetEntity(m => m.Id == id);
            if (question != null)
            {
                if (questionBll.Delete(question))
                {
                    if (!string.IsNullOrEmpty(question.Imgs))
                    {
                        //删除图片
                        string[] Imgs = question.Imgs.Split(';');
                        for (int i = 0; i < Imgs.Count(); i++)
                        {
                            DelFile(Imgs[i]);
                        }
                    }
                    if (!string.IsNullOrEmpty(question.AudioPath))
                    {
                        //删除语音
                        DelFile(question.AudioPath);
                    }
                    jm.Content = "删除问题" + question.Title + "成功";
                }
            }
            else
            {
                jm.Msg = "该上报问题不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除文件方法
        /// </summary>
        /// <param name="file"></param>
        public void DelFile(string file)
        {
            string filestr = Server.MapPath(file);
            if (System.IO.File.Exists(filestr))
            {
                System.IO.File.Delete(filestr);
            }
        }

        /// <summary>
        /// 上报问题详细
        /// </summary>
        /// <param name="id">要查看的问题</param>
        /// <returns>详细结果返回</returns>
        [HttpGet]
        [BreadCrumb(Label = "上报问题详细")]
        public ActionResult QuestionDetail(int id)
        {
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            //获取要查看的问题
            T_Question question = questionBll.GetEntity(m => m.Id == id);

            if (question != null)
            {
                return View(question);
            }
            else
            {
                return RedirectToAction("QuestionList");
            }
        }

        /// <summary>
        /// 跳转到指派上报问题处理人界面
        /// </summary>
        /// <param name="id">上报问题ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "指派处理人")]
        public ActionResult SetQuestionDisposer(int id)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //获取要处理的上报问题
            IQuestionBLL resultBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            var result = resultBll.GetEntity(r => r.Id == id);

            //如果上报问题结果不为空且未指派处理人
            if (result != null && result.DisposerId == null)
            {
                //初始化异常处理模型
                SetQuestionDisposerModel model = new SetQuestionDisposerModel()
                {
                    Id = result.Id,
                    Title = result.Title,
                    Desc = result.Desc
                };
                model.UserList = GetUserList(currentPlaceId);
                return View(model);
            }
            else
            {
                return RedirectToAction("QuestionList");
            }
        }

        /// <summary>
        /// 指派上报问题处理人提交
        /// </summary>
        /// <param name="model">指派处理人模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetQuestionDisposer(SetQuestionDisposerModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                //获取要指派处理人的上报问题结果
                IQuestionBLL resultBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                T_Question result = resultBll.GetEntity(m => m.Id == model.Id);
                if (result != null)
                {
                    //指派处理人
                    result.DisposerId = model.DisposerId;
                    //保存到数据库
                    if (resultBll.Update(result))
                    {
                        //日志记录
                        jm.Content = PropertyUtils.ModelToJsonString(model);

                        //推送给处理人
                        IPropertyUserPushBLL userPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                        var userPush = userPushBLL.GetEntity(p => p.UserId == model.DisposerId);
                        if (userPush != null)
                        {
                            string registrationId = userPush.RegistrationId;
                            //通知信息
                            bool flag = PropertyUtils.SendPush("业主上报问题","有新的问题需要您处理,请查看", ConstantParam.MOBILE_TYPE_PROPERTY, registrationId);
                            if (!flag)
                            {
                                jm.Msg = "推送发生异常";
                            }
                        }
                    }
                    else 
                    {
                        jm.Msg = "指派处理人失败";
                    }
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上报问题处理状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BreadCrumb(Label = "处理上报问题")]
        [HttpGet]
        public ActionResult DisposeQuestion(int id)
        {
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            T_Question question = questionBll.GetEntity(m => m.Id == id);
            if (question != null)
            {
                DisposeQuestionModel model = new DisposeQuestionModel()
                {
                    Id = question.Id,
                    Title = question.Title
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("QuestionList");
            }
        }

        /// <summary>
        /// 上报问题处理状态
        /// </summary>
        /// <param name="model">处理的上报问题模型</param>
        /// <returns>处理状态返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DisposeQuestion(DisposeQuestionModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                //获取要处理的上报问题
                IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
                T_Question question = questionBll.GetEntity(m => m.Id == model.Id);
                if (question != null)
                {
                    //修改处理状态并添加处理记录
                    question.Status = ConstantParam.DISPOSED;
                    question.IsPublish = model.IsPublish ? 1 : 0;
                    T_QuestionDispose questionDispose = new T_QuestionDispose()
                    {
                        DisposeDesc = model.DisposeDesc,
                        DisposeUserId = GetSessionModel().UserID,
                        QuestionId = model.Id,
                        DisposeTime = DateTime.Now
                    };
                    //保存到数据库
                    questionBll.DisposeQuestion(question, questionDispose);

                    IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    var userPush = userPushBLL.GetEntity(p => p.UserId == question.UploadUserId);
                    if (userPush != null)
                    {
                        string registrationId = userPush.RegistrationId;
                        string alert = "您" + question.UploadTime.ToString("yyyy-MM-dd HH:mm") + "上报的问题已处理";
                        //通知信息
                        bool flag = PropertyUtils.SendPush("上报问题处理", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                        if (!flag)
                        {
                            jm.Msg = "推送发生异常";
                        }
                    }

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该问题不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 业主上报问题-平台列表

        /// <summary>
        /// 业主上报问题-平台列表
        /// </summary> 
        /// <param name="model">平台搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "上报问题列表")]
        [HttpGet]
        public ActionResult QuestionPlatformList(QuestionPlatformSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;
            model.StatusList = GetStatusList();

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);
            Expression<Func<T_Question, bool>> where = u => u.UploadTime >= model.StartTime.Value && u.UploadTime < endTime
                && u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
            //根据小区名称查询
            if (model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == model.PropertyPlaceId.Value);
            }
            //根据状态名称查询
            if (model.Status != null)
            {
                where = PredicateBuilder.And(where, u => u.Status == model.Status);
            }

            //根据问题名称模糊查询
            if (!string.IsNullOrEmpty(model.Title))
            {
                where = PredicateBuilder.And(where, u => u.Title.Contains(model.Title));
            }

            //根据查询条件调用BLL层 获取分页数据
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            var sortName = this.SettingSorting("Id", false);
            model.DataList = questionBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<T_Question>;

            //获取所有物业小区列表
            model.PropertyPlaceList = GetPropertyPlaceList();

            return View(model);
        }

        /// <summary>
        /// 上报问题详细-平台列表
        /// </summary>
        /// <param name="id">要查看的问题</param>
        /// <returns>详细结果返回</returns>
        [HttpGet]
        [BreadCrumb(Label = "上报问题详细")]
        public ActionResult QuestionPlatformDetail(int id)
        {
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            //获取要查看的问题
            T_Question question = questionBll.GetEntity(m => m.Id == id);

            if (question != null)
            {
                return View(question);
            }
            else
            {
                return RedirectToAction("QuestionPlatformList");
            }
        }

        #endregion

        #region 业主上报问题-物业平台列表

        /// <summary>
        /// 业主上报问题-物业平台列表
        /// </summary> 
        /// <param name="model">平台搜索模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "上报问题列表")]
        [HttpGet]
        public ActionResult QuestionCompanyPlatformList(QuestionPlatformSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;
            model.StatusList = GetStatusList();

            int CompanyId = GetSessionModel().CompanyId.Value;

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);
            Expression<Func<T_Question, bool>> where = u => u.UploadTime >= model.StartTime.Value && u.UploadTime < endTime
                && u.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlace.CompanyId == CompanyId;

            //根据小区名称查询
            if (model.PropertyPlaceId != null)
            {
                where = PredicateBuilder.And(where, u => u.PropertyPlaceId == model.PropertyPlaceId.Value);
            }

            //根据状态名称查询
            if (model.Status != null)
            {
                where = PredicateBuilder.And(where, u => u.Status == model.Status);
            }

            //根据问题名称模糊查询
            if (!string.IsNullOrEmpty(model.Title))
            {
                where = PredicateBuilder.And(where, u => u.Title.Contains(model.Title));
            }

            //根据查询条件调用BLL层 获取分页数据
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            var sortName = this.SettingSorting("Id", false);
            model.DataList = questionBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<T_Question>;

            //获取所有物业小区列表
            model.PropertyPlaceList = GetPropertyPlaceList();

            return View(model);
        }

        /// <summary>
        /// 上报问题详细-物业平台列表
        /// </summary>
        /// <param name="id">要查看的问题</param>
        /// <returns>详细结果返回</returns>
        [HttpGet]
        [BreadCrumb(Label = "上报问题详细")]
        public ActionResult QuestionCompanyPlatformDetail(int id)
        {
            IQuestionBLL questionBll = BLLFactory<IQuestionBLL>.GetBLL("QuestionBLL");
            //获取要查看的问题
            T_Question question = questionBll.GetEntity(m => m.Id == id);

            if (question != null)
            {
                return View(question);
            }
            else
            {
                return RedirectToAction("QuestionCompanyPlatformList");
            }
        }

        #endregion



        /// <summary>
        /// 获取物业小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetPropertyPlaceList()
        {
            Expression<Func<T_PropertyPlace, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;

            if (GetSessionModel().CompanyId != null) 
            {
                var CompanyId = GetSessionModel().CompanyId.Value;
                where = PredicateBuilder.And(where, p => p.CompanyId == CompanyId);
            }

            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(where);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 上报问题的状态列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetStatusList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "未处理",
                Value = ConstantParam.NO_DISPOSE.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已处理",
                Value = ConstantParam.DISPOSED.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 获取本小区物业用户列表
        /// </summary>
        /// <param name="currentPlaceId">物业小区ID</param>
        /// <returns>物业用户列表</returns>
        private List<SelectListItem> GetUserList(int currentPlaceId)
        {
            var sortModel = this.SettingSorting("Id", false);

            //调用BLL层获取物业用户列表
            IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
            var pointList = userBll.GetList(c => c.PropertyPlaceId == currentPlaceId
                && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT, sortModel.SortName, sortModel.IsAsc).ToList();

            //转换为下拉列表并返回
            return pointList.Select(c => new SelectListItem()
            {
                Text = string.IsNullOrEmpty(c.TrueName) ? c.UserName : (pointList.Count(p => p.TrueName == c.TrueName) > 1 ? c.TrueName + "(" + c.UserName + ")" : c.TrueName),
                Value = c.Id.ToString(),
                Selected = false,
            }).ToList();
        }
    }
}
