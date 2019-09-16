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
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 楼座单元控制器
    /// </summary>
    public class BuildUnitController : BaseController
    {
        /// <summary>
        /// 单元列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "单元列表")]
        [HttpGet]
        public ActionResult UnitList(BuildUnitSearchModel model)
        {
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_BuildUnit, bool>> where = w => (string.IsNullOrEmpty(model.BuildName) ? true : w.Build.BuildName.Contains(model.BuildName)) && (model.BuildId == 0 ? true : w.BuildId == model.BuildId) && (string.IsNullOrEmpty(model.UnitName) ? true : w.UnitName.Contains(model.UnitName)) && w.Build.PropertyPlaceId == propertyPlaceId;

            // 排序模型
            var sortModel = this.SettingSorting("Id", false);
            model.List = unitBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_BuildUnit>;
            return View(model);
        }

        /// <summary>
        /// 新增单元
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增单元")]
        [HttpGet]
        public ActionResult AddUnit()
        {
            BuildUnitAddModel model = new BuildUnitAddModel();
            model.BuildList = getBuildList();
            return View(model);
        }

        /// <summary>
        /// 获取楼座列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> getBuildList()
        {
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var placeId = GetSessionModel().PropertyPlaceId.Value;
            var list = buildBll.GetList(p => p.PropertyPlaceId == placeId);
            //转换为下拉列表
            List<SelectListItem> buildList = list.Select(c => new SelectListItem()
            {
                Text = c.BuildName,
                Value = c.Id.ToString(),
                Selected = false,
            }).ToList();
            return buildList;
        }

        /// <summary>
        /// 远程验证指定单元名称是否存在
        /// </summary>
        /// <param name="unitName">单元名称</param>
        /// <param name="id">单元id,新增时恒为0，修改单元名称时不为0</param>
        public ContentResult RemoteCheckExist(BuildUnitSearchModel model)
        {
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            // 楼座名称已存在
            if (unitBll.Exist(m => m.UnitName == model.UnitName&&m.Id!=model.UnitId&&m.BuildId == model.BuildId))
            {
                // 校验不通过
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
           /// <summary>
           /// 远程验证批量添加单元
           /// </summary>
           /// <param name="model"></param>
           /// <param name="BuildId"></param>
           /// <returns></returns>
        public string RemoteCheck(BuildUnitBatchAddModel model)
        {
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            var lamdaList = new List<Expression<Func<T_BuildUnit, bool>>>();
            foreach (var name in model.UnitName)
            {
                lamdaList.Add(r => r.UnitName == name);
            }
            Expression<Func<T_BuildUnit, bool>> lamda = p => false;
            Expression<Func<T_BuildUnit, bool>> lamda1 = r => (r.BuildId == model.BuildId);
            foreach (var expression in lamdaList)
            {
                lamda = PredicateBuilder.Or<T_BuildUnit>(lamda, expression);
            }

            lamda = PredicateBuilder.And(lamda, lamda1);

            var unitlist = unitBll.GetList(lamda);//查询

            if (unitlist.Count() > 0)
            {
                // 校验不通过
                return string.Join(",", unitlist.ToList().Select(q => q.UnitName).Distinct().ToArray());
            }
            else
            {
                // 校验通过
                return "";
            }

        }
        /// <summary>
        /// 新增单元
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUnit(BuildUnitSearchModel model)
        {
            JsonModel jm = new JsonModel();
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            if (unitBll.Exist(m => m.UnitName == model.UnitName && m.Id != model.UnitId && m.BuildId == model.BuildId))
            {
                jm.Msg = "该单元名称已经存在";
            }
            //如果表单模型验证成功
            else if (ModelState.IsValid)
            {
                T_BuildUnit newUnit = new T_BuildUnit()
                {
                    UnitName = model.UnitName,
                    BuildId = model.BuildId
                };
                // 保存到数据库
                unitBll.Save(newUnit);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 批量添加单元
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "批量添加单元")]
        [HttpGet]
        public ActionResult BatchAddUnit(int buildId)
        {
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var entity = buildBll.GetEntity(m => m.Id == buildId);
            BuildUnitSearchModel model = new BuildUnitSearchModel();
            model.BuildName = entity.BuildName;
            model.BuildId = buildId;
            return View(model);
        }


        /// <summary>
        /// 批量添加单元
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchAddUnit(BuildUnitBatchAddModel model)
        {
            JsonModel jm = new JsonModel();
            var unitId = model.BuildId;
            var result = RemoteCheck(model);
            if (!string.IsNullOrEmpty(result))
            {
                jm.Msg = result + "单元已存在";
                return Json(jm, JsonRequestBehavior.AllowGet);
            }
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
                List<T_BuildUnit> list = new List<T_BuildUnit>();
                for (int i = 0; i < model.UnitName.Length; i++)
                {
                    T_BuildUnit newUnit = new T_BuildUnit()
                    {
                        UnitName = model.UnitName[i],
                        BuildId = model.BuildId
                    };
                    list.Add(newUnit);
                }
                // 批量保存
                unitBll.BatchAddUnit(model.BuildId, list);
                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除单元
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteUnit(int id)
        {
            JsonModel jm = new JsonModel();
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            // 根据指定id值获取实体对象
            var unit = unitBll.GetEntity(index => index.Id == id);
            if (unit == null)
            {
                jm.Msg = "该单元不存在";
            }
            else
            {
                if (unitBll.Delete(unit))
                {
                    //操作日志
                    jm.Content = "删除单元 " + unit.UnitName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 编辑单元
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑单元")]
        [HttpGet]
        public ActionResult EditUnit(int id)
        {
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            var unitInfo = unitBll.GetEntity(index => index.Id == id);

            if (unitInfo != null)
            {
                BuildUnitSearchModel unitModel = new BuildUnitSearchModel();
                unitModel.BuildName = unitInfo.Build.BuildName;
                unitModel.UnitName = unitInfo.UnitName;
                unitModel.UnitId = id;
                unitModel.BuildId = unitInfo.Build.Id;
                return View(unitModel);
            }
            else
            {
                return RedirectToAction("UnitList");
            }
        }

        /// <summary>
        /// 编辑单元
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUnit(BuildUnitSearchModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
                T_BuildUnit unitInfo = unitBll.GetEntity(m => m.Id == model.UnitId);
                if (unitInfo != null)
                {
                    unitInfo.UnitName = model.UnitName;
                    // 保存到数据库
                    unitBll.Update(unitInfo);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该单元不存在";
                }
            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
