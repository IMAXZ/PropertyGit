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
    public class PropertyPaymentController : BaseController
    {
        /// <summary>
        /// 缴费类别列表
        /// </summary>
        /// <returns></returns>
       [BreadCrumb(Label="缴费类别列表")]
       [HttpGet]
        public ActionResult PaymentTypeList(ExpenseTypeSearchModel model)
        {
           IPropertyExpenseTypeBLL expenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
           int PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
           Expression<Func<T_PropertyExpenseType, bool>> where = u => u.PropertyPlaceId == PropertyPlaceId;
            //查询条件:是否固定
           if (model.IsFixed != null)
           {
               where = PredicateBuilder.And(where,u => u.IsFixed == model.IsFixed.Value);
           }
           //根据名称查询
           if (!string.IsNullOrEmpty(model.Kword)) 
           {
               where = PredicateBuilder.And(where, u => u.Name.Contains(model.Kword));
           }
            //排序
            var sortModel = this.SettingSorting("Id", false);
          
            //将查询到的数据赋值传到页面
            model.DataList = expenseTypeBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PropertyExpenseType>;
            model.IsFixedList = getTypeList(null);
            return View(model);
        }

        /// <summary>
        /// 缴费类别新增
        /// </summary>
        /// <returns></returns>
       [BreadCrumb(Label = "新增缴费类别")]
       [HttpGet]
        public ActionResult AddPaymentType()
        {
            PropertyExpenseTypeModel model = new PropertyExpenseTypeModel();
            model.TypeList = getTypeList();
            return View(model);
        }
        /// <summary>
        /// 缴费类别新增提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult AddPaymentType(PropertyExpenseTypeModel model)
       {
           JsonModel jm = new JsonModel();
           //如果表单模型验证成功
           if (ModelState.IsValid)
           {
               IPropertyExpenseTypeBLL propertyExpenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
               T_PropertyExpenseType propertyExpenseType = new T_PropertyExpenseType()
               {
                   IsFixed=model.IsFixed.Value,
                   Name = model.Name,
                   Memo=model.Memo,
                   PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value
               };
               // 保存
               propertyExpenseTypeBll.Save(propertyExpenseType);

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
        /// 缴费类别编辑
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑缴费类别")]
        [HttpGet]
        public ActionResult EditPaymentType(int id) 
        {
            IPropertyExpenseTypeBLL propertyExpenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            //获取要编辑的类别 
            T_PropertyExpenseType propertyExpenseType = propertyExpenseTypeBll.GetEntity(m => m.Id == id);
            if (propertyExpenseType != null)
            {
                //初始化返回页面的模型
                PropertyExpenseTypeModel model = new PropertyExpenseTypeModel()
                {
                    ExpenseTypeId=propertyExpenseType.Id,
                    IsFixed=propertyExpenseType.IsFixed,
                    Name=propertyExpenseType.Name,
                    Memo=propertyExpenseType.Memo,
                    TypeList = getTypeList(propertyExpenseType.IsFixed)
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("PaymentTypeList");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditPaymentType(PropertyExpenseTypeModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyExpenseTypeBLL propertyExpenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
                T_PropertyExpenseType propertyExpenseType = propertyExpenseTypeBll.GetEntity(m => m.Id == model.ExpenseTypeId);
                if (propertyExpenseType != null)
                {
                    propertyExpenseType.IsFixed = model.IsFixed.Value;
                    propertyExpenseType.Name=model.Name;
                    propertyExpenseType.Memo = model.Memo;
                    //编辑
                    if (propertyExpenseTypeBll.Update(propertyExpenseType))
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
                    jm.Msg = "该类别不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        [BreadCrumb(Label="删除缴费类别")]
        [HttpPost]
        public ActionResult DeletePaymentType(int id)
        {
            JsonModel jm = new JsonModel();
            IPropertyExpenseTypeBLL propertyExpenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            //获取要删除的类别
            T_PropertyExpenseType propertyExpense = propertyExpenseTypeBll.GetEntity(m => m.Id == id);
            if (propertyExpense == null)
            {
                jm.Msg = "该类别不存在";
            }
            else if (propertyExpense.PropertyExpenseNos.Count > 0 || propertyExpense.HouseUserExpenseTemplates.Count > 0 
                || propertyExpense.HouseUserExpenseDetails.Count > 0)
            {
                jm.Msg = "已有该类别的缴费相关记录，无法删除";
            }
            else
            {
                if (propertyExpenseTypeBll.Delete(propertyExpense))
                {
                    //操作日志
                    jm.Content = "删除缴费类别 " + propertyExpense.Name;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证指定类别名称是否存在
        /// </summary>
        /// <param name="UserName">类别名称</param>
        /// <param name="UserId">版本id,新增时恒为0</param>
        [HttpPost]
        public ContentResult RemoteCheckExist(string Name,int Id = 0)
        {
            int CurrentPlaceId = GetSessionModel().PropertyPlaceId.Value;
            IPropertyExpenseTypeBLL propertyExpenseTypeBll = BLLFactory<IPropertyExpenseTypeBLL>.GetBLL("PropertyExpenseTypeBLL");
            // 版本名称已存在
            if (propertyExpenseTypeBll.Exist(m => m.Name == Name && m.PropertyPlaceId == CurrentPlaceId && m.Id != Id))
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
        /// 获取是否固定缴费类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> getTypeList(int? Fixed = 0)
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = "非固定",
                Value = ConstantParam.NO_FIXED.ToString(),
                Selected = Fixed == ConstantParam.NO_FIXED
            });
            typeList.Add(new SelectListItem()
            {
                Text = "固定",
                Value = ConstantParam.FIXED.ToString(),
                Selected = Fixed == ConstantParam.FIXED
            });
            return typeList;
        }
    }
}
