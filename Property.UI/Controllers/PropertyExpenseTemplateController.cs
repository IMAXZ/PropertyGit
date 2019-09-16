using MvcBreadCrumbs;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers
{
    public class PropertyExpenseTemplateController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "物业缴费模板设置")]
        [HttpGet]
        public ActionResult ExpenseSettings(ExpenseSettingsModel model)
        {
            var currentPropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            if (model.NotificationDate == null)
            {
                model.NotificationDate = DateTime.Now.AddDays(1);
            }

            IBuildBLL buildBLL = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            //绑定楼座下拉列表
            var buildList = buildBLL.GetList(b => b.PropertyPlaceId == currentPropertyPlaceId, "BuildName", true).ToList();
            model.BuildList = buildList.Select(b => new SelectListItem() { Text = b.BuildName, Value = b.Id.ToString() }).ToList();

            List<SelectListItem> unitList = new List<SelectListItem>();
            unitList.Add(new SelectListItem() { Text = "选择单元", Value = string.Empty });
            //绑定单元下拉列表
            if (model.BuildId > 0)
            {
                IBuildUnitBLL unitBll = FactoryBLL.BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
                var units = unitBll.GetList(u => u.BuildId == model.BuildId).OrderBy(u => u.UnitName).Select(u => new SelectListItem() { Text = u.UnitName, Value = u.Id.ToString(), Selected = (u.Id == model.UnitId) }).ToList();
                unitList.AddRange(units);
            }

            model.UnitList = unitList;

            //绑定缴费种类名称下拉列表
            IPropertyExpenseTypeBLL typeBLL = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            model.ExpenseTypeList = typeBLL.GetList(t => t.IsFixed == model.ExpenseClassId && t.PropertyPlaceId == currentPropertyPlaceId).OrderBy(t => t.Name).Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }).ToList();

            //绑定缴费明细
            IBuildUnitBLL unitBLL = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");

            if (model.BuildId > 0)
            {
                var unit = unitBLL.GetEntity(u => u.BuildId == model.BuildId && u.Id == model.UnitId);

                if (unit != null)
                {
                    var doorList = unit.BuildDoors.OrderBy(u => u.DoorName).ToList();
                    var doorIds = doorList.Select(d => d.Id);

                    IHouseUserExpenseTemplateBLL expenseTemplateBLL = BLLFactory<IHouseUserExpenseTemplateBLL>.GetBLL("HouseUserExpenseTemplateBLL");
                    var allDoorExpenseByExpenseType = expenseTemplateBLL.GetList(e => e.ExpenseTypeId == model.ExpenseTypeId && e.BuildDoorId != null && doorIds.Contains(e.BuildDoorId.Value));

                    var houseUserExpenseTemplateList = new List<HouseUserExpenseTemplateModel>();

                    if (allDoorExpenseByExpenseType.Count() > 0)
                    {
                        var doorExpense = allDoorExpenseByExpenseType.FirstOrDefault();
                        model.ExpenseCircleId = doorExpense.ExpenseCycleId;
                        model.NotificationDate = doorExpense.NotificationDate;
                    }

                    foreach (var door in doorList)
                    {
                        var houseUserExpenseTemplateModel = new HouseUserExpenseTemplateModel();

                        houseUserExpenseTemplateModel.DoorId = door.Id;
                        houseUserExpenseTemplateModel.DoorName = door.DoorName;

                        var doorExpense = allDoorExpenseByExpenseType.FirstOrDefault(e => e.BuildDoorId == door.Id);
                        if (doorExpense != null)
                        {
                            houseUserExpenseTemplateModel.Expense = doorExpense.Expense.ToString();
                        }

                        houseUserExpenseTemplateList.Add(houseUserExpenseTemplateModel);
                    }

                    model.HouseUserExpenseTemplateList = houseUserExpenseTemplateList;
                }
            }

            return View(model);
        }
        /// <summary>
        /// 根据楼座ID加载单元列表
        /// </summary>
        /// <param name="buildId">楼座Id</param>
        /// <returns></returns>
        public JsonResult AjaxGetUnits(int? buildId)
        {
            List<SelectListItem> unitList = new List<SelectListItem>();

            if (buildId.HasValue && buildId.Value > 0)
            {
                IBuildUnitBLL unitBll = FactoryBLL.BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
                unitList.Add(new SelectListItem() { Text = "选择单元", Value = string.Empty, Selected = true });
                var units = unitBll.GetList(u => u.BuildId == buildId.Value).OrderBy(u => u.UnitName).Select(u => new SelectListItem() { Text = u.UnitName, Value = u.Id.ToString() }).ToList();
                unitList.AddRange(units);
            }

            return Json(unitList);
        }
        /// <summary>
        /// 根据缴费类型得到缴费种类名称列表
        /// </summary>
        /// <param name="expenseClassId"></param>
        /// <returns></returns>
        public JsonResult AjaxGetExpenseTypes(int expenseClassId)
        {
            List<SelectListItem> unitList = new List<SelectListItem>();
            int currentPropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            IPropertyExpenseTypeBLL expenseTypeBll = FactoryBLL.BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            var units = expenseTypeBll.GetList(u => u.IsFixed == expenseClassId && u.PropertyPlaceId == currentPropertyPlaceId).OrderBy(e => e.Name).Select(e => new SelectListItem() { Text = e.Name, Value = e.Id.ToString() }).ToList();
            unitList.AddRange(units);

            return Json(unitList);
        }
        /// <summary>
        /// 执行更新缴费模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateExpenseTemplate(ExpenseSettingsModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                List<T_HouseUserExpenseTemplate> allExpenseTemplate = new List<T_HouseUserExpenseTemplate>();
                string[] getExpenseVal = model.GetDoorExpenseVal.Split(',');

                //所有要更新的单元户ID字符串
                string allDoorIds = string.Empty;

                //获得单元户Id和应缴的费用，以及取的所有要更新的单元户Id, 将要更新的单元户信息转成T_HouseUserExpenseTemplate对象，以便用于更新数据库
                foreach (var item in getExpenseVal)
                {
                    string[] expenseVal = item.Split('_');

                    if (expenseVal.Length > 1)
                    {
                        //所有要更新的单元户ID
                        if (string.IsNullOrEmpty(allDoorIds))
                        {
                            allDoorIds = expenseVal[0].ToString();
                        }
                        else
                        {
                            allDoorIds = allDoorIds + "," + expenseVal[0].ToString();
                        }

                        //为T_HouseUserExpenseTemplate对象赋值
                        var houseUserExpenseTemplate = new T_HouseUserExpenseTemplate();

                        houseUserExpenseTemplate.ExpenseTypeId = model.UpdateExpenseTypeId;
                        houseUserExpenseTemplate.Expense = double.Parse(expenseVal[1].ToString());
                        houseUserExpenseTemplate.ExpenseCycleId = model.ExpenseCircleId;
                        houseUserExpenseTemplate.NotificationDate = model.NotificationDate.Value;
                        houseUserExpenseTemplate.Operator = GetSessionModel().UserID;
                        houseUserExpenseTemplate.OperatorDate = DateTime.Now;
                        houseUserExpenseTemplate.BuildDoorId = int.Parse(expenseVal[0].ToString());

                        allExpenseTemplate.Add(houseUserExpenseTemplate);
                    }
                }

                //更新单元户缴费模板
                IHouseUserExpenseTemplateBLL expenseTemplateBLL = BLLFactory<IHouseUserExpenseTemplateBLL>.GetBLL("HouseUserExpenseTemplateBLL");
                bool check = expenseTemplateBLL.UpdateExpenseTemplate(allDoorIds, allExpenseTemplate, model.UpdateExpenseTypeId);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
    }
}
