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
    /// 单元户控制器
    /// </summary>
    public class BuildDoorController : BaseController
    {
        /// <summary>
        /// 单元户列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "单元户列表")]
        [HttpGet]
        public ActionResult DoorList(BuildDoorSearchModel model)
        {
            // 获取单元户访问对象
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_BuildDoor, bool>> where = w => (model.UnitId == 0 ? true : w.UnitId == model.UnitId) && (string.IsNullOrEmpty(model.DoorName) ? true : w.DoorName.Contains(model.DoorName)) && (string.IsNullOrEmpty(model.UnitName) ? true : w.BuildUnit.UnitName.Contains(model.UnitName)) && (string.IsNullOrEmpty(model.BuildName) ? true : w.BuildUnit.Build.BuildName.Contains(model.BuildName)) && w.BuildUnit.Build.PropertyPlaceId == propertyPlaceId;

            // 排序模型
            var sortModel = this.SettingSorting("Id", false);
            model.List = doorBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_BuildDoor>;
            return View(model);
        }


        /// <summary>
        /// 新增单元户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增单元户")]
        [HttpGet]
        public ActionResult AddDoor(int unitId)
        {
            BuildDoorAddModel model = new BuildDoorAddModel();
            model.BuildList = getBuildList();
            model.UnitList = new List<SelectListItem>();
            return View(model);
        }


        /// <summary>
        /// 批量添加单元户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "批量添加单元户")]
        [HttpGet]
        public ActionResult BatchAddDoor(int unitId)
        {
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            var entity = unitBll.GetEntity(m => m.Id == unitId);
            BuildDoorSearchModel model = new BuildDoorSearchModel();
            model.UnitId = unitId;
            model.UnitName = entity.UnitName;
            model.BuildName = entity.Build.BuildName;
            model.BuildId = entity.Build.Id;
            return View(model);
        }


        /// <summary>
        /// 批量添加单元户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchAddDoor(BuildDoorBatchAddModel model, int BuildId)
        {
            JsonModel jm = new JsonModel();
            var unitId = model.UnitId;
            var result = RemoteCheck(model, BuildId);
            if (!string.IsNullOrEmpty(result))
            {
                // 保存异常日志
                jm.Msg = result + "单元户名已存在";
                return Json(jm, JsonRequestBehavior.AllowGet);
            }

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
                List<T_BuildDoor> list = new List<T_BuildDoor>();
                for (int i = 0; i < model.DoorName.Length; i++)
                {
                    T_BuildDoor newDoor = new T_BuildDoor()
                    {
                        DoorName = model.DoorName[i],
                        UnitId = model.UnitId
                    };
                    list.Add(newDoor);
                }
                // 批量保存
                doorBll.BatchAddDoor(model.UnitId, list);
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
        /// 远程验证指定单元户名称是否存在
        /// </summary>
        /// <param name="doorName">单元户名称</param>
        /// <param name="id">单元户id,新增时恒为0，修改单元户名称时不为0</param>
        public ContentResult RemoteCheckExist(BuildDoorSearchModel model)
        {
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            // 单元户名称已存在
            if (doorBll.Exist(m => m.DoorName == model.DoorName && m.Id != model.DoorId && m.UnitId == model.UnitId))
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
        /// 批量增加单元户的远程验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RemoteCheck(BuildDoorBatchAddModel model, int BuildId)
        {
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            var lamdaList = new List<Expression<Func<T_BuildDoor, bool>>>();
            foreach (var name in model.DoorName)
            {
                lamdaList.Add(r => r.DoorName == name);
            }
            Expression<Func<T_BuildDoor, bool>> lamda = p => false;
            Expression<Func<T_BuildDoor, bool>> lamda1 = r => (r.UnitId == model.UnitId && r.BuildUnit.BuildId == model.BuildId);
            foreach (var expression in lamdaList)
            {
                lamda = PredicateBuilder.Or<T_BuildDoor>(lamda, expression);
            }

            lamda = PredicateBuilder.And(lamda, lamda1);

            var doorlist = doorBll.GetList(lamda);//查询

            if (doorlist.Count() > 0)
            {
                // 校验不通过
                return string.Join(",", doorlist.ToList().Select(q => q.DoorName).Distinct().ToArray());
            }
            else
            {
                // 校验通过
                return "";
            }

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
        /// 获取单元列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUnitList(int buildId)
        {
            IBuildUnitBLL buildUnitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");
            var list = buildUnitBll.GetList(u => u.BuildId == buildId);
            //转换为下拉列表
            List<SelectListItem> buildUnitList = list.Select(c => new SelectListItem()
            {
                Text = c.UnitName,
                Value = c.Id.ToString(),
                Selected = false,
            }).ToList();
            return Json(buildUnitList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增单元户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDoor(BuildDoorSearchModel model)
        {
            JsonModel jm = new JsonModel();
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            if (doorBll.Exist(m => m.DoorName == model.DoorName && m.Id != model.DoorId && m.UnitId == model.UnitId))
            {
                jm.Msg = "该单元户名称已经存在";
            }
            //如果表单模型验证成功
            else if (ModelState.IsValid)
            {
                T_BuildDoor newDoor = new T_BuildDoor()
                {
                    DoorName = model.DoorName,
                    UnitId = model.UnitId
                };
                // 保存到数据库
                doorBll.Save(newDoor);

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
        /// 删除单元户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteDoor(int id)
        {
            JsonModel jm = new JsonModel();
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            // 根据指定id值获取实体对象
            var door = doorBll.GetEntity(index => index.Id == id);
            var count = door.HouseUsers.Where(p=>p.DelFlag==0).Count();
            if (door == null)
            {
                jm.Msg = "该单元户不存在";
            }
            else
            {
                if (doorBll.Delete(door))
                {
                    //操作日志
                    jm.Content = "删除单元户 " + door.DoorName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑单元户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑单元户")]
        [HttpGet]
        public ActionResult EditDoor(int id)
        {
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
            var doorInfo = doorBll.GetEntity(index => index.Id == id);

            if (doorInfo != null)
            {
                BuildDoorSearchModel doorModel = new BuildDoorSearchModel();
                doorModel.BuildName = doorInfo.BuildUnit.Build.BuildName;
                doorModel.UnitName = doorInfo.BuildUnit.UnitName;
                doorModel.UnitId = doorInfo.BuildUnit.Id;
                doorModel.DoorId = id;
                doorModel.DoorName = doorInfo.DoorName;
                return View(doorModel);
            }
            else
            {
                return RedirectToAction("DoorList");
            }
        }

        /// <summary>
        /// 编辑单元户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDoor(BuildDoorSearchModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");
                T_BuildDoor doorInfo = doorBll.GetEntity(m => m.Id == model.DoorId);
                if (doorInfo != null)
                {
                    doorInfo.DoorName = model.DoorName;
                    // 保存到数据库
                    doorBll.Update(doorInfo);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该单元户不存在";
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
