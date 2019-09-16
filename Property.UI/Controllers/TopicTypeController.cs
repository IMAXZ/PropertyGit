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
    /// 贴吧主题分类控制器
    /// </summary>
    public class TopicTypeController : BaseController
    {
        /// <summary>
        /// 小区沟通主题分类列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "沟通类别列表")]
        public ActionResult CategoryList(SearchModel model)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");

            //初始化查询条件：当前小区
            Expression<Func<T_PostBarTopicType, bool>> where = c => c.PropertyPlaceId == currentPlaceId;
            //根据主题类别名称模糊查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, a => a.Name.Contains(model.Kword));
            }

            //排序
            var sortModel = this.SettingSorting("id", false);
            //获取分页数据
            var data = categoryBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(data);
        }

        /// <summary>
        /// 新增主题类别
        /// </summary>
        /// <returns>跳转到新增主题类别</returns>
        [HttpGet]
        [BreadCrumb(Label = "新增沟通类别")]
        public ActionResult AddCategory()
        {
            return View();
        }

        /// <summary>
        /// 新增主题类别提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddCategory(TopicTypeModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //模型赋值
                T_PostBarTopicType category = new T_PostBarTopicType()
                {
                    Name = model.CategoryName,
                    PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value
                };
                //调用BLL层进行添加处理
                IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
                categoryBll.Save(category);
                //记录日志
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 编辑主题类别
        /// </summary>
        /// <param name="id">主题类别ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑沟通类别")]
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            JsonModel jm = new JsonModel();

            //调用BLL层获取要编辑的沟通主题类别
            IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
            var category = categoryBll.GetEntity(c => c.Id == id);

            if (category != null)
            {
                //给模型赋值，传递到编辑页面
                TopicTypeModel model = new TopicTypeModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CategoryList");
            }
        }

        /// <summary>
        /// 编辑主题类别提交
        /// </summary>
        /// <param name="model">主题类别表单模型</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult EditCategory(TopicTypeModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                ///调用BLL层获取正在编辑的主题类别
                IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
                var category = categoryBll.GetEntity(c => c.Id == model.CategoryId);
                if (category != null)
                {
                    //重新赋值
                    category.Name = model.CategoryName;
                    //编辑：如果成功
                    if (categoryBll.Update(category))
                    {
                        //记录日志
                        jm.Content = PropertyUtils.ModelToJsonString(model);
                    }
                    else
                    {
                        jm.Msg = "编辑失败";
                    }
                }
                else
                {
                    jm.Msg = "该主题类别不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除主题类别
        /// </summary>
        /// <param name="id">要删除的主题类别id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //调用BLL层获取要删除的主题类别
                IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
                var category = categoryBll.GetEntity(c => c.Id == id);

                if (category == null)
                {
                    jm.Msg = "该主题类别不存在";

                }
                else if (category.PostBarTopics.Count() > 0)
                {
                    jm.Msg = "已有该类别的主题，不能删除";
                }
                else
                {
                    //删除
                    if (categoryBll.Delete(category))
                    {
                        //记录日志
                        jm.Content = "删除沟通主题类别 " + category.Name;
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


        /// <summary>
        /// 验证主题类别名称同小区是否存在
        /// </summary>
        /// <param name="categoryId">沟通主题类别ID</param>
        /// <param name="categoryName">沟通主题类别名称</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CategoryCheckExist(string name,int categoryId = 0)
        {
            //获取当前小区
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //调用BLL层判断主题类别是否已存在
            IPostBarTopicTypeBLL categoryBll = BLLFactory<IPostBarTopicTypeBLL>.GetBLL("PostBarTopicTypeBLL");
            //如果已存在，验证不通过
            if (categoryBll.Exist(m => m.Name == name && m.PropertyPlaceId == currentPlaceId && m.Id != categoryId))
            {
                return Content("false");
            }
            else//不存在，则验证通过
            {
                return Content("true");
            }
        }
    }
}
