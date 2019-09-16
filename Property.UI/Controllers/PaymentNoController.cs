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
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 单用户缴费编号管理控制器
    /// </summary>
    public class PaymentNoController : BaseController
    {
        /// <summary>
        /// 缴费编号列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "缴费编号列表")]
        public ActionResult NumberList(PaymentNoSearchModel model)
        {
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //1：初始化查询条件
            Expression<Func<T_PropertyExpenseNo, bool>> where = u => u.PropertyExpenseType.PropertyPlaceId == CurrentPlaceId;
            //缴费类型
            if (model.ExpenseTypeId != null)
            {
                where = PredicateBuilder.And(where, u => u.ExpenseTypeId == model.ExpenseTypeId.Value);
            }
            //缴费编号
            if (!string.IsNullOrEmpty(model.ExpenseNumber))
            {
                where = PredicateBuilder.And(where, u => u.ExpenseNumber.Contains(model.ExpenseNumber));
            }
            //楼座 单元 单元户信息查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                IPropertyPlaceBLL placeBLL = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBLL.GetEntity(p => p.Id == CurrentPlaceId);
                if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
                {
                    where = PredicateBuilder.And(where, u => u.BuildDoor.DoorName.Contains(model.Kword)
                        || u.BuildDoor.BuildUnit.Build.BuildName.Contains(model.Kword)
                        || u.BuildDoor.BuildUnit.UnitName.Contains(model.Kword));
                }
                else if (place.PlaceType == ConstantParam.PLACE_TYPE_COMPANY)
                {
                    where = PredicateBuilder.And(where, u => u.BuildCompany.Name.Contains(model.Kword));
                }
            }
            //2.排序
            var sortModel = this.SettingSorting("Id", false);

            //3.调用BLL层获取分页数据
            IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
            model.ResultList = expenseNoBLL.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PropertyExpenseNo>;

            //4.初始化缴费类别下拉列表
            model.ExpenseTypeList = GetExpenseTypeList();
            return View(model);
        }

        /// <summary>
        /// 新增缴费编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "新增缴费编号")]
        public ActionResult AddNumber()
        {
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPropertyPlaceBLL placeBLL = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = placeBLL.GetEntity(p => p.Id == CurrentPlaceId);

            PaymentNoModel model = new PaymentNoModel();
            model.PlaceType = place.PlaceType;
            if (model.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                model.BuildList = GetBuildList(CurrentPlaceId);
                model.UnitList = new List<SelectListItem>();
                model.DoorList = new List<SelectListItem>();
            }
            else if (model.PlaceType == ConstantParam.PLACE_TYPE_COMPANY)
            {
                model.BuildCompanyList = GetBuildCompanyList(CurrentPlaceId);
            }
            model.ExpenseTypeList = GetExpenseTypeList();
            return View(model);
        }

        /// <summary>
        /// 添加缴费编号提交
        /// </summary>
        /// <param name="model">缴费编号表单模型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public JsonResult AddNumber(PaymentNoModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
                T_PropertyExpenseNo expenseNo = new T_PropertyExpenseNo()
                {
                    ExpenseNumber = model.ExpenseNumber,
                    ExpenseTypeId = model.ExpenseTypeId,
                    Memo = model.Memo,
                    BuildCompanyId = model.BuildCompanyId,
                    BuildDoorId = model.DoorId,
                    CreatedDate = DateTime.Now
                };
                //保存
                expenseNoBLL.Save(expenseNo);

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
        /// 编辑缴费编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "编辑缴费编号")]
        public ActionResult EditNumber(int id)
        {
            IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
            var expenseNo = expenseNoBLL.GetEntity(u => u.Id == id);
            if (expenseNo != null)
            {
                PaymentNoModel model = new PaymentNoModel()
                {
                    Id = expenseNo.Id,
                    ExpenseNumber = expenseNo.ExpenseNumber,
                    ExpenseTypeId = expenseNo.ExpenseTypeId,
                    Memo = expenseNo.Memo,
                };
                if (expenseNo.BuildCompanyId != null)
                {
                    model.PlaceType = ConstantParam.PLACE_TYPE_COMPANY;
                    model.BuildCompanyId = expenseNo.BuildCompanyId;
                    model.BuildCompanyList = GetBuildCompanyList(expenseNo.BuildCompany.PropertyPlaceId);
                }
                else if (expenseNo.BuildDoorId != null)
                {
                    model.PlaceType = ConstantParam.PLACE_TYPE_HOUSE;
                    model.BuildId = expenseNo.BuildDoor.BuildUnit.Build.Id;
                    model.BuildList = GetBuildList(expenseNo.BuildDoor.BuildUnit.Build.PropertyPlaceId);
                    model.UnitId = expenseNo.BuildDoor.BuildUnit.Id;
                    model.UnitList = GetUnitList(model.BuildId);
                    model.DoorId = expenseNo.BuildDoorId;
                    model.DoorList = GetDoorList(model.UnitId);
                }
                model.ExpenseTypeList = GetExpenseTypeList();
                return View(model);
            }
            else
            {
                return RedirectToAction("NumberList");
            }
        }

        /// <summary>
        /// 编辑缴费编号提交
        /// </summary>
        /// <param name="model">缴费编号模型</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditNumber(PaymentNoModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
                var expenseNo = expenseNoBLL.GetEntity(u => u.Id == model.Id);
                if (expenseNo != null)
                {
                    expenseNo.ExpenseNumber = model.ExpenseNumber;
                    expenseNo.ExpenseTypeId = model.ExpenseTypeId;
                    expenseNo.Memo = model.Memo;
                    expenseNo.BuildCompanyId = model.BuildCompanyId;
                    expenseNo.BuildDoorId = model.DoorId;
                    //修改保存到数据库
                    if (expenseNoBLL.Update(expenseNo))
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
                    jm.Msg = "该缴费编号不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除缴费编号
        /// </summary>
        /// <param name="id">缴费编号Id</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        public JsonResult DeleteNumber(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要删除的缴费编号
            IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
            var expenseNo = expenseNoBLL.GetEntity(u => u.Id == id);
            if (expenseNo == null)
            {
                jm.Msg = "该缴费编号不存在";
            }
            else
            {
                //删除成功
                if (expenseNoBLL.Delete(expenseNo))
                {
                    //记录操作日志
                    jm.Content = "删除缴费编号：" + expenseNo.ExpenseNumber;
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 远程验证是否已存在指定单元户或办公楼单位的缴费编号
        ///// </summary>
        ///// <param name="doorId">户id</param>
        ///// <param name="Id">缴费编号ID</param>
        //[HttpPost]
        //public ContentResult RemoteCheckExist(int? buildCompanyId, int? doorId, int id)
        //{
        //    IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
        //    // 指定单元户的缴费编号已存在
        //    if (doorId != null && expenseNoBLL.Exist(m => m.BuildDoorId == doorId && m.Id != id))
        //    {
        //        // 校验不通过
        //        return Content("false");
        //    }
        //    else if (buildCompanyId != null && expenseNoBLL.Exist(m => m.BuildCompanyId == buildCompanyId && m.Id != id))
        //    {
        //        // 校验不通过
        //        return Content("false");
        //    }
        //    else
        //    {
        //        return Content("true");
        //    }
        //}

        /// <summary>
        /// 远程验证缴费编号是否存在
        /// </summary>
        /// <param name="Id">缴费编号ID</param>
        /// <param name="Number">缴费编号</param>
        /// <returns></returns>
        public ContentResult RemoteCheckNumberExist(int Id, string Number)
        {
            IPropertyExpenseNoBLL expenseNoBLL = BLLFactory<IPropertyExpenseNoBLL>.GetBLL("PropertyExpenseNoBLL");
            //缴费编号已存在
            if (expenseNoBLL.Exist(m => m.ExpenseNumber == Number && m.Id != Id))
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
        /// 获取当前小区缴费种类下拉列表
        /// </summary>
        private List<SelectListItem> GetExpenseTypeList()
        {
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPropertyExpenseTypeBLL expenseTypeBLL = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = expenseTypeBLL.GetList(u => u.PropertyPlaceId == CurrentPlaceId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        #region 楼盘,单元,户下拉列表

        /// <summary>
        /// 获取楼盘列表
        /// </summary>
        /// <returns>楼盘列表</returns>
        private List<SelectListItem> GetBuildList(int PlaceId)
        {
            //获取楼座列表
            IBuildBLL BuildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = BuildBll.GetList(u => u.PropertyPlaceId == PlaceId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.BuildName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取单元列表
        /// </summary>
        /// <returns>单元列表</returns>
        private List<SelectListItem> GetUnitList(int buildId)
        {
            //获取单元列表
            IBuildUnitBLL UnitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = UnitBll.GetList(u => u.BuildId == buildId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.UnitName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取单元户列表
        /// </summary>
        /// <returns>户列表</returns>
        private List<SelectListItem> GetDoorList(int unitId)
        {
            //获取单元户列表
            IBuildDoorBLL DoorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = DoorBll.GetList(u => u.UnitId == unitId, sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.DoorName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 根据楼盘ID获取单元列表
        /// </summary>
        /// <param name="buildId">楼盘ID</param>
        /// <returns>单元列表</returns>
        [HttpPost]
        public JsonResult GetUnitList(int? buildId)
        {
            List<object> list = new List<object>();
            if (buildId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            IBuildUnitBLL bll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            foreach (var item in bll.GetList(m => m.BuildId == buildId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.UnitName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据单元ID获取单元户列表
        /// </summary>
        /// <param name="buildId">单元ID</param>
        /// <returns>单元户列表</returns>
        [HttpPost]
        public JsonResult GetDoorList(int? unitId)
        {
            List<object> list = new List<object>();
            if (unitId == null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            IBuildDoorBLL bll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            foreach (var item in bll.GetList(m => m.UnitId == unitId.Value).ToList())
            {
                list.Add(new { Value = item.Id.ToString(), Text = item.DoorName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 获取当前小区办公楼单位列表
        /// </summary>
        private List<SelectListItem> GetBuildCompanyList(int PlaceId)
        {
            //获取办公楼单位列表
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            var sortModel = this.SettingSorting("Id", true);
            var list = buildCompanyBll.GetList(u => u.PropertyPlaceId == PlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT,
                sortModel.SortName, sortModel.IsAsc);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }
    }
}
