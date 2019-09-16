using Microsoft.Ajax.Utilities;
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
    /// 审批业主信息 控制器
    /// </summary>
    public class ApprovalOwnerController : BaseController
    {
        /// <summary>
        /// 审批业主信息列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "审批业主信息列表")]
        public ActionResult ApprovalOwnerList(ApprovalOwnerModel model)
        {

            //获取当前小区
            var propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;

            //状态下来列表
            model.IsVerifiedList = GetIsVerifiedList();

            Expression<Func<R_PropertyIdentityVerification, bool>> where = u => u.BuildCompany.PropertyPlaceId == propertyPlaceId || u.BuildDoor.BuildUnit.Build.PropertyPlaceId == propertyPlaceId;

            //根据状态名称查询
            if (model.IsVerified != null)
            {
                where = PredicateBuilder.And(where, u => u.IsVerified == model.IsVerified);
            }

            //根据姓名进行查询
            if (!string.IsNullOrEmpty(model.Name))
            {
                where = Property.Common.PredicateBuilder.And(where, u => u.Name.Contains(model.Name));
            }

            //楼座 单元 单元户信息查询
            if (!string.IsNullOrEmpty(model.Kword))
            {
                IPropertyPlaceBLL placeBLL = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBLL.GetEntity(p => p.Id == propertyPlaceId);
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

            //获取要审批业主信息的分页数据
            IPropertyIdentityVerificationBLL propertyIdentityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");
            var sortName = this.SettingSorting("Id", false);
            model.DataList = propertyIdentityVerificationBll.GetPageList(where, sortName.SortName, sortName.IsAsc, model.PageIndex) as PagedList<R_PropertyIdentityVerification>;
            return View(model);
        }


        /// <summary>
        /// 通过申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PassOwnerMgr(int id)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                IPropertyIdentityVerificationBLL propertyIdentityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");

                //获取要审批的业主信息
                var propertyIdentityVerification = propertyIdentityVerificationBll.GetEntity(m => m.Id == id);

                if (propertyIdentityVerification != null)
                {
                    //修改状态
                    propertyIdentityVerification.IsVerified = ConstantParam.IsVerified_YES;

                    //保存到数据库
                    propertyIdentityVerificationBll.Update(propertyIdentityVerification);

                    IUserPushBLL userPushBll = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    var userPush = userPushBll.GetEntity(u => u.UserId == propertyIdentityVerification.AppUserId);

                    if (userPush != null)
                    {
                        //设备Id
                        var registrationId = userPush.RegistrationId;

                        //通知消息
                        string alert = "尊敬的住户,您的身份已通过验证,开始体验新功能吧";
                        bool flag = PropertyUtils.SendPush("审批业主信息", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);

                        if (!flag)
                        {
                            jm.Msg = "推送发生异常";
                        }
                    }

                    //记录日志
                    jm.Content = PropertyUtils.ModelToJsonString(userPush);
                }
                else
                {
                    jm.Msg = "该业主信息不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 驳回申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectOwnerMgr(int id)
        {
            JsonModel jm = new JsonModel();

            if (ModelState.IsValid)
            {
                IPropertyIdentityVerificationBLL propertyIdentityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");

                //获取要审批的业主信息
                var propertyIdentityVerification = propertyIdentityVerificationBll.GetEntity(m => m.Id == id);

                if (propertyIdentityVerification != null)
                {
                    //修改状态
                    propertyIdentityVerification.IsVerified = ConstantParam.IsVerified_NO;

                    //保存到数据库
                    propertyIdentityVerificationBll.Update(propertyIdentityVerification);

                    IUserPushBLL userPushBll = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    var userPush = userPushBll.GetEntity(u => u.UserId == propertyIdentityVerification.AppUserId);

                    if (userPush != null)
                    {
                        //设备Id
                        var registrationId = userPush.RegistrationId;

                        //通知消息
                        string alert = "尊敬的住户,您提交的身份信息不正确，请核实后提交申请";
                        bool flag = PropertyUtils.SendPush("审批业主信息", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);

                        if (!flag)
                        {
                            jm.Msg = "推送发生异常";
                        }
                    }

                    //记录日志
                    jm.Content = PropertyUtils.ModelToJsonString(userPush);
                }
                else
                {
                    jm.Msg = "该业主信息不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 验证状态列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetIsVerifiedList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "已驳回",
                Value = ConstantParam.IsVerified_NO.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "审核中",
                Value = ConstantParam.IsVerified_DEFAULT.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "已通过",
                Value = ConstantParam.IsVerified_YES.ToString(),
                Selected = false
            });
            
            return list;
        }
    }
}
