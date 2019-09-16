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

namespace Property.UI.Controllers
{
    /// <summary>
    /// 楼盘管理控制器
    /// </summary>
    public class BuildController : BaseController
    {
        /// <summary>
        /// 楼座列表
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "楼座列表")]
        [HttpGet]
        public ActionResult BuildList(BuildSearchModel model)
        {
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_Build, bool>> where = w => (string.IsNullOrEmpty(model.BuildName) ? true : w.BuildName.Contains(model.BuildName)) && (string.IsNullOrEmpty(model.Desc) ? true : w.BuildName.Contains(model.Desc)) && w.PropertyPlaceId == propertyPlaceId;

            // 排序模型
            var sortModel = this.SettingSorting("Id", false);
            var list = buildBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }


        /// <summary>
        /// 新增楼座
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增楼座")]
        [HttpGet]
        public ActionResult AddBuild()
        {
            BuildSearchModel model = new BuildSearchModel();
            model.PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            return View(model);
        }


        /// <summary>
        /// 新增楼座
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBuild(BuildSearchModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
                T_Build newBuild = new T_Build()
                {
                    BuildName = model.BuildName,
                    PropertyPlaceId = model.PropertyPlaceId,
                    Desc = model.Desc
                };
                // 保存到数据库
                buildBll.Save(newBuild);

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
        /// 远程验证指定楼座名称是否存在
        /// </summary>
        /// <param name="buildName">楼座名称</param>
        /// <param name="id">楼座id,新增时恒为0，修改楼座名称时不为0</param>
        public ContentResult RemoteCheckExist(int id, string buildName)
        {
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var placeId = GetSessionModel().PropertyPlaceId.Value;
            // 楼座名称已存在
            if (buildBll.Exist(m => m.BuildName == buildName && m.Id != id && m.PropertyPlaceId == placeId))
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
        /// 删除楼座
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBuild(int id)
        {
            JsonModel jm = new JsonModel();
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            // 根据指定id值获取实体对象
            var build = buildBll.GetEntity(index => index.Id == id);
            if (build == null)
            {
                jm.Msg = "该楼座不存在";
            }
            else
            {
                if (buildBll.Delete(build))
                {
                    //操作日志
                    jm.Content = "删除楼座 " + build.BuildName;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑楼座
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑楼座")]
        [HttpGet]
        public ActionResult EditBuild(int id)
        {
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var buildInfo = buildBll.GetEntity(index => index.Id == id);

            if (buildInfo != null)
            {
                BuildSearchModel buildModel = new BuildSearchModel();
                buildModel.BuildName = buildInfo.BuildName;
                buildModel.Desc = buildInfo.Desc;
                return View(buildModel);
            }
            else
            {
                return RedirectToAction("BuildList");
            }
        }

        /// <summary>
        /// 编辑楼座
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBuild(BuildSearchModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
                T_Build buildInfo = buildBll.GetEntity(m => m.Id == model.Id);
                if (buildInfo != null)
                {
                    buildInfo.BuildName = model.BuildName;
                    buildInfo.Desc = model.Desc;
                    // 保存到数据库
                    buildBll.Update(buildInfo);

                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
                else
                {
                    jm.Msg = "该楼座不存在";
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
