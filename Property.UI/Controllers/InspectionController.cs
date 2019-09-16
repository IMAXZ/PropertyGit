using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 巡检管理控制器
    /// </summary>
    public class InspectionController : BaseController
    {
        #region 巡检类别管理

        /// <summary>
        /// 巡检类别列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检类别列表")]
        public ActionResult ContentList(InspectionCategorySearchModel model)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");

            //初始化查询条件：当前小区
            Expression<Func<T_InspectionCategory, bool>> where = c => c.PropertyPlaceId == currentPlaceId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
            //根据巡检类别名称模糊查询
            if (!string.IsNullOrEmpty(model.CategoryName))
            {
                where = PredicateBuilder.And(where, a => a.CategoryName.Contains(model.CategoryName));
            }

            //排序
            var sortModel = this.SettingSorting("id", false);
            //获取分页数据
            var data = categoryBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);

            return View(data);
        }

        /// <summary>
        /// 新增巡检类别）
        /// </summary>
        /// <returns>新增巡检类别界面</returns>
        [HttpGet]
        [BreadCrumb(Label = "新增巡检类别")]
        public ActionResult AddContent()
        {
            return View();
        }

        /// <summary>
        /// 新增巡检类别提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddContent(InspectionCategoryModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //模型赋值
                T_InspectionCategory category = new T_InspectionCategory()
                {
                    CategoryName = model.CategoryName,
                    Memo = model.Memo,
                    PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value,
                    DelFlag = ConstantParam.DEL_FLAG_DEFAULT
                };
                //调用BLL层进行添加处理
                IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
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
        /// 编辑巡检类别
        /// </summary>
        /// <param name="id">巡检类别ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑巡检类别")]
        [HttpGet]
        public ActionResult EditContent(int id)
        {
            JsonModel jm = new JsonModel();

            //调用BLL层获取要编辑的巡检类别
            IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
            var category = categoryBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (category != null)
            {
                //给模型赋值，传递到编辑页面
                InspectionCategoryModel model = new InspectionCategoryModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.CategoryName,
                    Memo = category.Memo
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("ContentList");
            }
        }

        /// <summary>
        /// 编辑巡检类别提交
        /// </summary>
        /// <param name="model">巡检类别表单模型</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult EditContent(InspectionCategoryModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                ///调用BLL层获取正在编辑的巡检类别
                IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
                var category = categoryBll.GetEntity(c => c.Id == model.CategoryId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (category != null)
                {
                    //重新赋值
                    category.CategoryName = model.CategoryName;
                    category.Memo = model.Memo;

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
                    jm.Msg = "该巡检类别不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除巡检类别
        /// </summary>
        /// <param name="id">要删除的巡检类别id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteContent(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //调用BLL层获取要删除的巡检类别
                IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
                var category = categoryBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (category == null)
                {
                    jm.Msg = "该巡检类别不存在";

                }
                else if (category.InspectionPoints.Where(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Count() > 0)
                {
                    jm.Msg = "已有该巡检类别的巡检点，不能删除";
                }
                else
                {
                    category.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                    //删除（修改删除标识）
                    if (categoryBll.Update(category))
                    {
                        //记录日志
                        jm.Content = "删除巡检类别 " + category.CategoryName;
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
        /// 验证巡检类别名称同小区是否存在
        /// </summary>
        /// <param name="categoryId">巡检类别ID</param>
        /// <param name="categoryName">巡检类别名称</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult ContentCheckExist(int categoryId, string categoryName)
        {
            //获取当前小区
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //调用BLL层判断巡检类别是否已存在
            IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
            //如果已存在，验证不通过
            if (categoryBll.Exist(m => m.CategoryName == categoryName && m.PropertyPlaceId == currentPlaceId
                && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.Id != categoryId))
            {
                return Content("false");
            }
            else//不存在，则验证通过
            {
                return Content("true");
            }
        }

        #endregion

        #region 巡检点管理

        /// <summary>
        /// 巡检点列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检点列表")]
        public ActionResult PointList(InspectionPointSearchModel model)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");

            //初始化查询条件：未删除
            Expression<Func<T_InspectionPoint, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && p.InspectionCategory.PropertyPlaceId == currentPlaceId;
            //查询条件：巡检类别
            if (model.CategoryId != null)
            {
                where = PredicateBuilder.And(where, p => p.CategoryId == model.CategoryId);
            }
            //根据巡检点名称模糊查询
            if (!string.IsNullOrEmpty(model.PointName))
            {
                where = PredicateBuilder.And(where, p => p.PointName.Contains(model.PointName));
            }

            //排序
            var sortModel = this.SettingSorting("id", false);
            //获取分页数据
            model.DataList = pointBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_InspectionPoint>;

            model.CategoryList = GetInspectionCategoryList(currentPlaceId);
            return View(model);
        }

        /// <summary>
        /// 新增巡检点
        /// </summary>
        /// <returns>新增巡检点界面</returns>
        [HttpGet]
        [BreadCrumb(Label = "新增巡检点")]
        public ActionResult AddPoint()
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            InspectionPointModel model = new InspectionPointModel();
            model.CategoryList = GetInspectionCategoryList(currentPlaceId);
            return View(model);
        }

        /// <summary>
        /// 新增巡检点提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPoint(InspectionPointModel model)
        {
            JsonModel jm = new JsonModel();
            IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //如果要添加的巡检点已存在
                if (pointBll.Exist(p => p.CategoryId == model.CategoryId && p.PointName == model.PointName && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT)) 
                {
                    jm.Msg = "该巡检点名称已存在";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                //模型赋值
                T_InspectionPoint point = new T_InspectionPoint()
                {
                    PointName = model.PointName,
                    Memo = model.Memo,
                    CategoryId = model.CategoryId,
                    DelFlag = ConstantParam.DEL_FLAG_DEFAULT
                };
                //调用BLL层进行添加处理
                pointBll.Save(point);
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
        /// 编辑巡检点
        /// </summary>
        /// <param name="id">巡检点ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑巡检点")]
        [HttpGet]
        public ActionResult EditPoint(int id)
        {
            JsonModel jm = new JsonModel();

            //调用BLL层获取要编辑的巡检点
            IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");
            var point = pointBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (point != null)
            {
                //获取当前物业小区ID
                int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

                //给模型赋值，传递到编辑页面
                InspectionPointModel model = new InspectionPointModel()
                {
                    PointId = point.Id,
                    PointName = point.PointName,
                    Memo = point.Memo,
                    CategoryId = point.CategoryId,
                    CategoryList = GetInspectionCategoryList(currentPlaceId)
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("PointList");
            }
        }

        /// <summary>
        /// 编辑巡检点提交
        /// </summary>
        /// <param name="model">巡检点表单模型</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult EditPoint(InspectionPointModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");

                //如果要编辑的巡检点名称已存在
                if (pointBll.Exist(p => p.CategoryId == model.CategoryId && p.PointName == model.PointName && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.Id != model.PointId))
                {
                    jm.Msg = "该巡检点名称已存在";
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                ///调用BLL层获取正在编辑的巡检点
                var point = pointBll.GetEntity(c => c.Id == model.PointId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (point != null)
                {
                    //重新赋值
                    point.PointName = model.PointName;
                    point.Memo = model.Memo;
                    point.CategoryId = model.CategoryId;
                    //编辑：如果成功
                    if (pointBll.Update(point))
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
                    jm.Msg = "该巡检点不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除巡检点
        /// </summary>
        /// <param name="id">要删除的巡检点id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeletePoint(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //调用BLL层获取要删除的巡检点
                IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");
                var point = pointBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (point == null)
                {
                    jm.Msg = "该巡检点不存在";

                }
                else if (point.PlanPoints.Count > 0)
                {
                    jm.Msg = "已有巡检任务包含该巡检点，不能删除";
                }
                else
                {
                    point.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                    //删除（修改删除标识）
                    if (pointBll.Update(point))
                    {
                        //记录日志
                        jm.Content = "删除巡检点 " + point.PointName;
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
        /// 验证巡检点名称是否在同一巡检内容中已存在
        /// </summary>
        /// <param name="pointId">巡检点ID</param>
        /// <param name="pointName">巡检点名称</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult PointCheckExist(int PointId, string PointName, int CategoryId)
        {
            //调用BLL层判断巡检点是否已存在
            IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");
            //如果已存在，验证不通过
            if (pointBll.Exist(p => p.PointName == PointName && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && p.CategoryId == CategoryId && p.Id != PointId))
            {
                return Content("false");
            }
            else//不存在，则验证通过
            {
                return Content("true");
            }
        }


        /// <summary>
        /// 获取巡检内容（类别）列表
        /// </summary>
        /// <param name="currentPlaceId">物业小区ID</param>
        /// <returns>巡检内容（类别）下拉列表</returns>
        private List<SelectListItem> GetInspectionCategoryList(int currentPlaceId)
        {
            var sortModel = this.SettingSorting("Id", false);

            //调用BLL层获取巡检内容列表
            IInspectionCategoryBLL categoryBll = BLLFactory<IInspectionCategoryBLL>.GetBLL("InspectionCategoryBLL");
            var list = categoryBll.GetList(c => c.PropertyPlaceId == currentPlaceId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(c => new SelectListItem()
            {
                Text = c.CategoryName,
                Value = c.Id.ToString(),
                Selected = false,
            }).ToList();
        }

        #endregion

        #region 巡检任务管理

        /// <summary>
        /// 巡检任务列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检任务列表")]
        public ActionResult PlanList(InspectionPlanSearchModel model)
        {
            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.BeforeDate == null)
                model.BeforeDate = today.AddDays(-today.Day + 1);
            if (model.EndDate == null)
                model.EndDate = today;

            model.TypeList = GetInspectionTypeList();

            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            DateTime endDate = model.EndDate.Value.AddDays(1);
            //2.查询条件：当前物业小区、未删除、巡检开始日期范围
            Expression<Func<T_InspectionPlan, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && p.PropertyPlaceId == currentPlaceId && p.BeginDate < endDate && p.EndDate >= model.BeforeDate;
            //根据巡检任务名称模糊查询
            if (!string.IsNullOrEmpty(model.PlanName))
            {
                where = PredicateBuilder.And(where, p => p.PlanName.Contains(model.PlanName));
            }
            //根据巡检类型查询
            if (model.Type != null)
            {
                where = PredicateBuilder.And(where, p => p.Type == model.Type.Value);
            }

            //3.根据查询条件调用BLL层 获取分页数据
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            var sortModel = this.SettingSorting("id", false);
            model.DataList = planBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_InspectionPlan>;

            return View(model);
        }

        /// <summary>
        /// 巡检任务详细查看
        /// </summary>
        /// <param name="id">巡检任务ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "查看任务详细")]
        public ActionResult PlanDetail(int id)
        {
            //调用BLL层获取要查看详细的巡检任务
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            var plan = planBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (plan != null)
            {
                var PointCategorys = plan.PlanPoints.Select(p => new PointCategoryModel
                {
                    CategoryName = p.InspectionPoint.InspectionCategory.CategoryName,
                    PointName = p.InspectionPoint.PointName,
                    PointMemo = p.InspectionPoint.Memo
                }).ToList();

                List<string> CategoryList = PointCategorys.Select(p => p.CategoryName).Distinct().ToList();
                ViewBag.PointCategorys = PointCategorys;
                ViewBag.CategoryList = CategoryList;
                return View(plan);
            }
            else
            {
                return RedirectToAction("PlanList");
            }
        }

        /// <summary>
        /// 新增巡检任务
        /// </summary>
        /// <returns>新增巡检任务界面</returns>
        [HttpGet]
        [BreadCrumb(Label = "新增巡检任务")]
        public ActionResult AddPlan()
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //初始化表单模型
            InspectionPlanModel model = new InspectionPlanModel();
            model.BeginDate = DateTime.Today;
            model.EndDate = DateTime.Today;

            model.TypeList = GetInspectionTypeList();
            model.IsRandomList = GetIsRandomList();
            model.HourNumList = GetHourNumList(null);
            model.WeekNumList = GetWeekNumList(null);
            model.DayNumList = GetDayNumList(null);
            model.PointList = GetInspectionPointList(currentPlaceId, null);
            return View(model);
        }

        /// <summary>
        /// 新增巡检任务提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPlan(InspectionPlanModel model)
        {
            JsonModel jm = new JsonModel();

            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                //模型赋值
                T_InspectionPlan plan = new T_InspectionPlan()
                {
                    PlanName = model.PlanName,
                    Memo = model.Memo,
                    PropertyPlaceId = currentPlaceId,
                    BeginDate = model.BeginDate,
                    EndDate = model.EndDate,
                    Type = model.Type,
                    IsRandom = model.IsRandom,
                    DelFlag = ConstantParam.DEL_FLAG_DEFAULT
                };

                //如果不随机
                if (model.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    int Numbers = 0;
                    //如果是日巡检
                    if (model.Type == ConstantParam.INSPECTION_TYPE_DAY)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (model.StartHourNums[i] != null && model.EndHourNums[i] != null)
                            {
                                plan.TimePlans.Add(new T_InspectionTimePlan()
                                {
                                    BeginNum = model.StartHourNums[i].Value,
                                    EndNum = model.EndHourNums[i].Value,
                                    Number = i + 1
                                });
                                Numbers++;
                            }
                        }
                        plan.Number = Numbers;
                    }
                    //如果是周巡检
                    else if (model.Type == ConstantParam.INSPECTION_TYPE_WEEK)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if (model.StartWeekNums[i] != null)
                            {
                                plan.TimePlans.Add(new T_InspectionTimePlan()
                                {
                                    BeginNum = model.StartWeekNums[i].Value,
                                    Number = i + 1
                                });
                                Numbers++;
                            }
                        }
                        plan.Number = Numbers;
                    }
                    else
                    {
                        //月巡检
                        for (int i = 0; i < 10; i++)
                        {
                            if (model.StartDayNums[i] != null)
                            {
                                plan.TimePlans.Add(new T_InspectionTimePlan()
                                {
                                    BeginNum = model.StartDayNums[i].Value,
                                    Number = i + 1
                                });
                                Numbers++;
                            }
                        }
                        plan.Number = Numbers;
                    }
                }
                else
                {
                    //如果是随机巡检
                    plan.Number = model.RandomNum.Value;
                    for (int i = 0; i < model.RandomNum.Value; i++)
                    {
                        plan.TimePlans.Add(new T_InspectionTimePlan()
                        {
                            Number = i + 1
                        });
                    }
                }

                //添加巡检点
                foreach (var item in model.PointIds)
                {
                    plan.PlanPoints.Add(new R_PlanPoint()
                    {
                        PointId = item
                    });
                }
                plan.PublishedFlag = model.IsPublished ? 1 : 0;

                //调用BLL层进行添加处理
                IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
                planBll.Save(plan);
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
        /// 编辑巡检任务
        /// </summary>
        /// <param name="id">巡检任务ID</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑巡检任务")]
        [HttpGet]
        public ActionResult EditPlan(int id)
        {
            JsonModel jm = new JsonModel();

            //调用BLL层获取要编辑的巡检任务
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            var plan = planBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (plan != null && plan.PublishedFlag == ConstantParam.PUBLISHED_FALSE)
            {
                //获取当前物业小区ID
                int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;
                var PointIds = plan.PlanPoints.Select(pp => pp.PointId).ToList();
                //给模型赋值，传递到编辑页面
                InspectionPlanModel model = new InspectionPlanModel()
                {
                    PlanId = plan.Id,
                    PlanName = plan.PlanName,
                    Memo = plan.Memo,
                    BeginDate = plan.BeginDate,
                    EndDate = plan.EndDate,
                    Type = plan.Type,
                    IsRandom = plan.IsRandom,
                    HourNumList = GetHourNumList(null),
                    WeekNumList = GetWeekNumList(null),
                    DayNumList = GetDayNumList(null),
                    TypeList = GetInspectionTypeList(),
                    IsRandomList = GetIsRandomList(),
                    PointList = GetInspectionPointList(currentPlaceId, PointIds)
                };

                //如果是不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    //将当期巡检安排传到页面
                    if (plan.Type == ConstantParam.INSPECTION_TYPE_DAY)
                    {
                        //日巡检
                        var StartHourNums = plan.TimePlans.Select(dp => dp.BeginNum).ToArray();
                        var EndHourNums = plan.TimePlans.Select(dp => dp.EndNum).ToArray();
                        for (int i = 0; i < model.StartHourNums.Length; i++)
                        {
                            if (i < StartHourNums.Length)
                            {
                                model.StartHourNums[i] = StartHourNums[i];
                                model.EndHourNums[i] = EndHourNums[i];
                                model.StartHourNumLists.Add(GetHourNumList(StartHourNums[i]));
                                model.EndHourNumLists.Add(GetHourNumList(EndHourNums[i]));
                            }
                        }
                    }
                    else if (plan.Type == ConstantParam.INSPECTION_TYPE_WEEK)
                    {
                        //周巡检
                        var StartWeekNums = plan.TimePlans.Select(dp => dp.BeginNum).ToArray();
                        var EndWeekNums = plan.TimePlans.Select(dp => dp.EndNum).ToArray();

                        for (int i = 0; i < model.StartWeekNums.Length; i++)
                        {
                            if (i < StartWeekNums.Length)
                            {
                                model.StartWeekNums[i] = StartWeekNums[i];
                                model.EndWeekNums[i] = EndWeekNums[i];
                                model.StartWeekNumLists.Add(GetWeekNumList(StartWeekNums[i]));
                                model.EndWeekNumLists.Add(GetWeekNumList(EndWeekNums[i]));
                            }
                        }
                    }
                    else
                    {
                        //月巡检
                        var StartDayNums = plan.TimePlans.Select(dp => dp.BeginNum).ToArray();
                        var EndDayNums = plan.TimePlans.Select(dp => dp.EndNum).ToArray();
                        for (int i = 0; i < model.StartDayNums.Length; i++)
                        {
                            if (i < StartDayNums.Length)
                            {
                                model.StartDayNums[i] = StartDayNums[i];
                                model.EndDayNums[i] = EndDayNums[i];
                                model.StartDayNumLists.Add(GetDayNumList(StartDayNums[i]));
                                model.EndDayNumLists.Add(GetDayNumList(EndDayNums[i]));
                            }
                        }
                    }
                }
                else
                {
                    model.RandomNum = plan.Number;
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("PlanList");
            }
        }

        /// <summary>
        /// 编辑巡检任务提交
        /// </summary>
        /// <param name="model">巡检任务表单模型</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult EditPlan(InspectionPlanModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                ///调用BLL层获取正在编辑的巡检任务
                IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
                var plan = planBll.GetEntity(c => c.Id == model.PlanId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (plan != null)
                {
                    //重新赋值
                    plan.PlanName = model.PlanName;
                    plan.Memo = model.Memo;
                    plan.BeginDate = model.BeginDate;
                    plan.EndDate = model.EndDate;
                    plan.Type = model.Type;
                    plan.IsRandom = model.IsRandom;
                    plan.PublishedFlag = model.IsPublished ? 1 : 0;
                    //如果是随机巡检
                    if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_TRUE)
                    {
                        plan.Number = model.RandomNum.Value;
                    }

                    //编辑：如果成功
                    if (planBll.UpdateInspectionPlan(plan, model.PointIds, model.StartHourNums, model.EndHourNums,
                        model.StartWeekNums, model.EndWeekNums, model.StartDayNums, model.EndDayNums))
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
                    jm.Msg = "该巡检任务不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除巡检任务
        /// </summary>
        /// <param name="id">要删除的巡检任务id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeletePlan(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                //调用BLL层获取要删除的巡检任务
                IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
                var plan = planBll.GetEntity(c => c.Id == id && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                if (plan == null)
                {
                    jm.Msg = "该巡检任务不存在";
                }
                else
                {
                    //删除（修改删除标识）
                    if (planBll.DeleteInspectionPlan(plan))
                    {
                        //记录日志
                        jm.Content = "删除巡检任务 " + plan.PlanName;
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
        /// 验证巡检任务名称在同小区中是否存在
        /// </summary>
        /// <param name="planId">巡检任务ID</param>
        /// <param name="planName">巡检任务名称</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult PlanCheckExist(int planId, string planName)
        {
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //调用BLL层判断巡检任务是否已存在
            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //如果已存在，验证不通过
            if (planBll.Exist(p => p.PlanName == planName && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && p.PropertyPlaceId == currentPlaceId && p.Id != planId))
            {
                return Content("false");
            }
            else//不存在，则验证通过
            {
                return Content("true");
            }
        }

        /// <summary>
        /// 获取巡检点列表
        /// </summary>
        /// <param name="currentPlaceId">物业小区ID</param>
        /// <param name="PointIds">巡检任务中已有的巡检点</param>
        /// <returns>巡检点列表</returns>
        private List<SelectListItem> GetInspectionPointList(int currentPlaceId, List<int> PointIds)
        {
            var sortModel = this.SettingSorting("Id", false);

            //调用BLL层获取巡检点列表
            IInspectionPointBLL pointBll = BLLFactory<IInspectionPointBLL>.GetBLL("InspectionPointBLL");
            var pointList = pointBll.GetList(c => c.InspectionCategory.PropertyPlaceId == currentPlaceId
                && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT, sortModel.SortName, sortModel.IsAsc).ToList();

            //转换为下拉列表并返回
            return pointList.Select(c => new SelectListItem()
            {
                Text = c.PointName + "（" + c.InspectionCategory.CategoryName + "）",
                Value = c.Id.ToString(),
                Selected = PointIds == null ? false : PointIds.Exists(i => i == c.Id),
            }).ToList();
        }

        /// <summary>
        /// 巡检类型下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetInspectionTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "日巡检",
                Value = ConstantParam.INSPECTION_TYPE_DAY.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "周巡检",
                Value = ConstantParam.INSPECTION_TYPE_WEEK.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "月巡检",
                Value = ConstantParam.INSPECTION_TYPE_MONTH.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 巡检是否随机列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetIsRandomList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "否",
                Value = ConstantParam.DELIVERY_FLAG_FALSE.ToString(),
                Selected = true
            });
            list.Add(new SelectListItem()
            {
                Text = "是",
                Value = ConstantParam.DELIVERY_FLAG_TRUE.ToString(),
                Selected = false
            });
            return list;
        }

        /// <summary>
        /// 获取小时数下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetHourNumList(int? Num)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i <= 24; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = i + " : 00",
                    Value = i.ToString(),
                    Selected = Num == null ? false : (Num == i)
                });
            }
            return list;
        }

        /// <summary>
        /// 获取星期数列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetWeekNumList(int? Num)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "星期一",
                Value = 1 + "",
                Selected = Num == null ? false : (Num == 1)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期二",
                Value = 2 + "",
                Selected = Num == null ? false : (Num == 2)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期三",
                Value = 3 + "",
                Selected = Num == null ? false : (Num == 3)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期四",
                Value = 4 + "",
                Selected = Num == null ? false : (Num == 4)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期五",
                Value = 5 + "",
                Selected = Num == null ? false : (Num == 5)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期六",
                Value = 6 + "",
                Selected = Num == null ? false : (Num == 6)
            });
            list.Add(new SelectListItem()
            {
                Text = "星期日",
                Value = 7 + "",
                Selected = Num == null ? false : (Num == 7)
            });
            return list;
        }

        /// <summary>
        /// 获取月巡检第几条下拉列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetDayNumList(int? Num)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 1; i <= 31; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = "第" + i + "日",
                    Value = i.ToString(),
                    Selected = Num == null ? false : (Num == i)
                });
            }
            return list;
        }
        #endregion

        #region 巡检实施监控

        /// <summary>
        /// 巡检实施监控结果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检实施监控")]
        public ActionResult ResultMonitorList()
        {
            return View();
        }

        /// <summary>
        /// 获取月巡检实施监控的日历数据
        /// </summary>
        /// <param name="Year">日历查询的年</param>
        /// <param name="Month">日历查询的月</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMonthPlanResultMonitorData(int Year, int Month)
        {
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var SelectStartDate = Convert.ToDateTime(Year + " - " + Month);
            var SelectEndDate = SelectStartDate.AddMonths(1);
            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == currentPlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_MONTH && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    //月巡检
                    foreach (var monthPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH))
                    {
                        int EndDayNum = monthPlan.EndNum;
                        int[] smalls = new int[] { 4, 6, 9, 11 };
                        //如果是小月
                        if (smalls.Contains(Month))
                        {
                            if (monthPlan.BeginNum > 30)
                            {
                                continue;
                            }
                        }
                        else if (Month == 2)
                        {
                            //闰年
                            if ((Year % 4 == 0 && Year / 100 != 0) || (Year % 400 == 0))
                            {
                                if (monthPlan.BeginNum > 29)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (monthPlan.BeginNum > 28)
                                {
                                    continue;
                                }
                            }
                        }
                        DateTime start = SelectStartDate.AddDays(monthPlan.BeginNum - 1);

                        //如果不在巡检时间段内
                        if (start > plan.EndDate || start < plan.BeginDate)
                        {
                            continue;
                        }
                        DateTime end = start.AddDays(1);

                        string StatusColor = "";

                        var resultList = monthPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (count == plan.PlanPoints.Count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + monthPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    //月巡检
                    foreach (var monthPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_MONTH))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectEndDate;

                        //如果不在巡检时间段内
                        if (end < plan.BeginDate || start > plan.EndDate)
                        {
                            continue;
                        }
                        if (start < plan.BeginDate)
                        {
                            start = plan.BeginDate;
                        }
                        if (end > plan.EndDate)
                        {
                            end = plan.EndDate.AddDays(1);
                        }

                        string StatusColor = "";

                        var resultList = monthPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + monthPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + monthPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取周巡检实施监控的日历数据
        /// </summary>
        /// <param name="SelectDate">日历周开始日期（周一）</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetWeekPlanResultMonitorData(string SelectDate)
        {
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var d = Convert.ToDateTime(SelectDate);
            var SelectStartDate = d.AddDays(1 - (int)d.DayOfWeek);
            var SelectEndDate = SelectStartDate.AddDays(7);

            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == currentPlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_WEEK && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    foreach (var weekPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK))
                    {
                        DateTime start = SelectStartDate.AddDays(weekPlan.BeginNum - 1);

                        //如果不在巡检时间段内
                        if (start > plan.EndDate || start < plan.BeginDate)
                        {
                            continue;
                        }
                        DateTime end = start.AddDays(1);

                        string StatusColor = "";

                        var resultList = weekPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + weekPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    foreach (var weekPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_WEEK))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectEndDate;

                        //如果不在巡检时间段内
                        if (end < plan.BeginDate || start > plan.EndDate)
                        {
                            continue;
                        }
                        if (start < plan.BeginDate)
                        {
                            start = plan.BeginDate;
                        }
                        if (end > plan.EndDate)
                        {
                            end = plan.EndDate.AddDays(1);
                        }

                        string StatusColor = "";

                        var resultList = weekPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (SelectEndDate < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + weekPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + weekPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取日巡检实施监控的日历数据
        /// </summary>
        /// <param name="Date">日期</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDayPlanResultMonitorData(string Date)
        {
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            var data = new List<InspectionMonitorModel>();

            IInspectionPlanBLL planBll = BLLFactory<IInspectionPlanBLL>.GetBLL("InspectionPlanBLL");
            //获取当前小区有指定年月任务的的巡检
            var SelectStartDate = Convert.ToDateTime(Date);
            var SelectEndDate = SelectStartDate.AddDays(1);

            var planList = planBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.PropertyPlaceId == currentPlaceId
                && p.Type == ConstantParam.INSPECTION_TYPE_DAY && p.BeginDate < SelectEndDate && p.EndDate >= SelectStartDate).ToList();

            foreach (var plan in planList)
            {
                //如果不随机
                if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                {
                    foreach (var dayPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY))
                    {
                        DateTime start = SelectStartDate.AddHours(dayPlan.BeginNum);
                        DateTime end = SelectStartDate.AddHours(dayPlan.EndNum);

                        string StatusColor = "";

                        var resultList = dayPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end.Date.AddDays(1) < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName,
                            start = start.ToString("yyyy-MM-dd HH"),
                            end = end.ToString("yyyy-MM-dd HH"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + dayPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.AddDays(1).ToString("yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    foreach (var dayPlan in plan.TimePlans.Where(p => p.InspectionPlan.Type == ConstantParam.INSPECTION_TYPE_DAY))
                    {
                        DateTime start = SelectStartDate;
                        DateTime end = SelectStartDate.AddDays(1);

                        string StatusColor = "";

                        var resultList = dayPlan.InspectionResults.Where(r => r.PlanDate >= SelectStartDate && r.PlanDate < SelectEndDate);
                        var count = resultList.Select(r => r.PointId).Distinct().Count();
                        //如果存在巡检结果，则为已巡检
                        if (resultList != null && count > 0)
                        {
                            //如果全部巡检点都已巡检
                            if (plan.PlanPoints.Count == count)
                            {
                                //如果所有巡检点的巡检结果都正常
                                if (!resultList.Any(r => r.Status == 1))
                                {
                                    StatusColor = "#1AB394";
                                }
                                else
                                {
                                    StatusColor = "#F8AC59";
                                }
                            }
                            else
                            {
                                //巡检中
                                StatusColor = "#23C6C8";
                            }
                        }
                        //未巡检
                        else
                        {
                            StatusColor = "#D1DADE";
                        }
                        //未巡检完 已过期
                        if (end < DateTime.Now && count < plan.PlanPoints.Count)
                        {
                            StatusColor = "#262626";
                        }
                        data.Add(new InspectionMonitorModel()
                        {
                            title = plan.PlanName + " 随机第" + dayPlan.Number + "次",
                            start = start.ToString("yyyy-MM-dd"),
                            end = end.ToString("yyyy-MM-dd"),
                            color = StatusColor,
                            url = "/Inspection/ResultDetail?PlanTimeId=" + dayPlan.Id + "&StartDate=" + start.ToString("yyyy-MM-dd") + "&EndDate=" + end.ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看巡检结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检结果查看")]
        public ActionResult ResultDetail(InspenctionResultSearchModel model)
        {
            IInspectionTimePlanBLL planTimeBll = BLLFactory<IInspectionTimePlanBLL>.GetBLL("InspectionTimePlanBLL");
            var planTime = planTimeBll.GetEntity(p => p.Id == model.PlanTimeId);
            if (planTime != null)
            {
                InspectionResultModel res = new InspectionResultModel()
                {
                    PlanName = planTime.InspectionPlan.PlanName,
                    PointList = planTime.InspectionPlan.PlanPoints.Select(p => p.InspectionPoint).ToList(),
                    ResultList = planTime.InspectionResults.Where(r => r.PlanDate >= model.StartDate && r.PlanDate < model.EndDate).ToList()
                };
                return View(res);
            }
            else
            {
                return RedirectToAction("ResultMonitorList");
            }
        }
        #endregion

        #region 巡检异常处理

        [HttpGet]
        [BreadCrumb(Label = "巡检异常列表")]
        public ActionResult ExceptionList(InspenctionExceptionSearchModel model)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //1.初始化默认查询模型
            DateTime today = DateTime.Today;
            if (model.StartTime == null)
                model.StartTime = today.AddDays(-today.Day + 1);
            if (model.EndTime == null)
                model.EndTime = today;
            model.StatusList = GetStatusList();

            //根据提报时间查询
            DateTime endTime = model.EndTime.Value.AddDays(1);

            //查询条件：当前小区 异常状态
            Expression<Func<T_InspectionResult, bool>> where = r => r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT
                && r.InspectionTimePlan.InspectionPlan.PropertyPlaceId == currentPlaceId && r.Status == ConstantParam.EXCEPTION
                && r.PlanDate >= model.StartTime.Value && r.PlanDate < endTime;

            //根据处理状态名称查询
            if (model.DisposeStatus != null)
            {
                where = PredicateBuilder.And(where, u => u.DisposeStatus == model.DisposeStatus);
            }

            //根据巡检点名称模糊查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, r => r.InspectionPoint.PointName.Contains(model.Kword));
            }
            //排序
            var sortModel = this.SettingSorting("id", false);

            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            model.DataList = resultBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_InspectionResult>;
            return View(model);
        }

        /// <summary>
        /// 巡检异常详细查看
        /// </summary>
        /// <param name="id">巡检异常结果ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "巡检异常详细")]
        public ActionResult ExceptionDetail(int id)
        {
            //获取要查看详细的异常信息
            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            var result = resultBll.GetEntity(r => r.Id == id && r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return RedirectToAction("ExceptionList");
            }
        }

        /// <summary>
        /// 跳转到指派巡检异常处理人界面
        /// </summary>
        /// <param name="id">巡检异常结果ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "指派处理人")]
        public ActionResult SetDisposer(int id)
        {
            //获取当前物业小区ID
            int currentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //获取要处理的异常结果
            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            var result = resultBll.GetEntity(r => r.Id == id && r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //如果异常结果不为空且未指派处理人
            if (result != null && result.DisposerId == null)
            {
                //初始化异常处理模型
                SetDisposerModel model = new SetDisposerModel()
                {
                    ResultId = result.Id,
                    PlanName = result.InspectionTimePlan.InspectionPlan.PlanName,
                    PointName = result.InspectionPoint.PointName,
                    ExceptionDesc = result.Desc
                };
                model.UserList = GetUserList(currentPlaceId);
                return View(model);
            }
            else
            {
                return RedirectToAction("ExceptionList");
            }
        }

        /// <summary>
        /// 指派巡检异常处理人提交
        /// </summary>
        /// <param name="model">指派处理人模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetDisposer(SetDisposerModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                //获取要指派处理人的巡检异常结果
                IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                T_InspectionResult result = resultBll.GetEntity(m => m.Id == model.ResultId);
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
                            bool flag = PropertyUtils.SendPush("巡检异常处理", "有巡检异常需要您处理,请查看", ConstantParam.MOBILE_TYPE_PROPERTY, registrationId);
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
        /// 巡检异常处理
        /// </summary>
        /// <param name="id">巡检异常结果ID</param>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "处理巡检异常")]
        public ActionResult DisposeException(int id)
        {
            //获取要处理的异常结果
            IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
            var result = resultBll.GetEntity(r => r.Id == id && r.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            if (result != null)
            {
                //初始化异常处理模型
                DisposeExceptionModel model = new DisposeExceptionModel()
                {
                    ResultId = result.Id,
                    PlanName = result.InspectionTimePlan.InspectionPlan.PlanName,
                    PointName = result.InspectionPoint.PointName
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("ExceptionList");
            }
        }

        /// <summary>
        /// 巡检异常处理提交
        /// </summary>
        /// <param name="id">巡检处理模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DisposeException(DisposeExceptionModel model)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                //获取要指派处理人的巡检异常结果
                IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                T_InspectionResult result = resultBll.GetEntity(m => m.Id == model.ResultId);
                if (result != null)
                {
                    //修改处理状态并添加处理记录
                    result.DisposeStatus = ConstantParam.DISPOSED;
                    T_InspectionExceptionDispose exceptionDispose = new T_InspectionExceptionDispose()
                    {
                        DisposeDesc = model.DisposeDesc,
                        DisposeUserId = GetSessionModel().UserID,
                        ExceptionResultId = model.ResultId,
                        DisposeTime = DateTime.Now
                    };
                    //保存到数据库
                    resultBll.DisposeException(result, exceptionDispose);
                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);

                    //推送给巡检提报人
                    IPropertyUserPushBLL userPushBLL = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                    var userPush = userPushBLL.GetEntity(p => p.UserId == result.UploadUserId);
                    if (userPush != null)
                    {
                        string registrationId = userPush.RegistrationId;
                        string alert = "您" + result.UploadTime.ToString("yyyy-MM-dd HH:mm") + "提报的异常已处理";
                        //通知信息
                        bool flag = PropertyUtils.SendPush("巡检异常处理",alert, ConstantParam.MOBILE_TYPE_PROPERTY, registrationId);
                        if (!flag)
                        {
                            jm.Msg = "推送发生异常";
                        }
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
        /// 处理状态下拉列表
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

        #endregion

    }
}
