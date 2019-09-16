using Microsoft.International.Converters.PinYinConverter;
using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Filter;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 小区绑定 身份验证控制器
    /// </summary>
    public class WeixinIdentityBindController : WeixinBaseController
    {

        /// <summary>
        /// 小区身份绑定列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var owner = GetCurrentUser();
            if (owner != null)
            {
                var BindPlaces = owner.UserPlaces.Where(p => p.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(p => new BindPlaceModel
                {
                    PlaceId = p.PropertyPlaceId,
                    PlaceName = p.PropertyPlace.Name,
                    CompanyName = p.PropertyPlace.Company.Name,
                    VerifyStatus = GetVerifyStatus(p.PropertyPlace, p.User)
                });
                return View(BindPlaces);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 小区绑定结果页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PlaceBindResult(int id)
        {
            var owner = GetCurrentUser();

            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = propertyPlaceBll.GetEntity(u => u.Id == id && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            ViewBag.PlaceType = place.PlaceType;
            //如果是住宅小区
            if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                var IdentityVerification = owner.PropertyIdentityVerification.Where(v => v.DoorId != null && v.BuildDoor.BuildUnit.Build.PropertyPlaceId == place.Id).FirstOrDefault();
                return View(IdentityVerification);
            }
            else
            {
                var IdentityVerification = owner.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.BuildCompany.PropertyPlaceId == place.Id).FirstOrDefault();
                return View(IdentityVerification);
            }
        }

        /// <summary>
        /// 添加小区页面
        /// </summary>
        /// <param name="id">所选城市ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddPlace(int? placeId, int cityId = 136)
        {
            PlaceAddSubmitModel model = new PlaceAddSubmitModel();
            model.CityList = GetCityList();
            if (placeId == null)
            {
                ICityBLL cityBll = BLLFactory<ICityBLL>.GetBLL("CityBLL");
                var city = cityBll.GetEntity(c => c.Id == cityId);
                if (city == null)
                {
                    city = cityBll.GetEntity(c => c.Id == 136);
                }
                model.CityId = city.Id;
                model.CityName = city.CityName;
                model.PlaceList = GetPlaceList(city.Id);
            }
            else
            {
                IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBll.GetEntity(c => c.Id == placeId.Value);
                model.CityId = place.CityId;
                model.CityName = place.City.CityName;
                model.PlaceId = placeId.Value;
                model.PlaceList = GetPlaceList(place.CityId);
                model.PlaceName = place.Name;
            }
            return View(model);
        }

        /// <summary>
        /// 添加绑定小区提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddPlace(PlaceAddSubmitModel model)
        {
            JsonModel jm = new JsonModel();

            var owner = GetCurrentUser();
            //如果要添加的小区不存在
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = propertyPlaceBll.GetEntity(u => u.Id == model.PlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (place == null)
            {
                jm.Msg = "小区已不存在";
            }
            //如果该用户已经有要添加的小区
            else if (owner.UserPlaces.Any(up => up.UserId == owner.Id && up.PropertyPlaceId == model.PlaceId))
            {
                jm.Msg = "小区已绑定，请重新选择";
            }
            else
            {
                //添加拥有的小区
                owner.UserPlaces.Add(new R_UserPlace()
                {
                    PropertyPlaceId = model.PlaceId
                });

                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                ownerBll.Update(owner);
                jm.Content = model.PlaceId.ToString();
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据关键字获取物业小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        [HttpGet]
        public JsonResult GetSearchPlaceList(int cityId, string kwords)
        {
            PageResultModel m = new PageResultModel();

            //查询条件
            Expression<Func<T_PropertyPlace, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CityId == cityId;
            //如果关键字不为空
            if (!string.IsNullOrEmpty(kwords))
            {
                where = PredicateBuilder.And(where, s => s.Name.Contains(kwords));
            }

            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            m.Result = placeBll.GetList(where).Select(p => new
            {
                PlaceId = p.Id,
                PlaceName = p.Name
            }).ToList();
            m.Total = placeBll.Count(where);
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 身份验证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IdentityVerification(int placeId)
        {
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = propertyPlaceBll.GetEntity(u => u.Id == placeId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (place == null)
            {
                return RedirectToAction("Index");
            }
            PlaceIdentityVerifyModel verifyModel = new PlaceIdentityVerifyModel();
            verifyModel.PlaceId = placeId;
            verifyModel.PlaceName = place.Name;
            verifyModel.PlaceType = place.PlaceType;
            verifyModel.BuildList = GetBuildList(placeId);
            verifyModel.UnitList = new List<SelectListItem>();
            verifyModel.DoorList = new List<SelectListItem>();

            verifyModel.BuildCompanyList = GetBuildCompanyList(placeId);
            return View(verifyModel);
        }

        /// <summary>
        /// 身份验证提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IdentityVerification(PlaceIdentityVerifyModel model)
        {
            JsonModel jm = new JsonModel();
            var owner = GetCurrentUser();

            //获取要绑定验证的小区
            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var place = propertyPlaceBll.GetEntity(u => u.Id == model.PlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (place == null)
            {
                jm.Msg = "小区已不存在";
                return Json(jm, JsonRequestBehavior.AllowGet);
            }

            IPropertyIdentityVerificationBLL identityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");
            //如果小区类型为住宅小区
            if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                //判断验证码是否正确
                IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.Phone && v.ActionCode == 2);
                //判断验证码不准确
                if (val == null || model.VerityCode != val.ValidateCode)
                {
                    jm.Msg = APIMessage.VALIDATE_ERROR;
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }
                //验证码已失效
                if (val.InvalidTime < DateTime.Now)
                {
                    jm.Msg = APIMessage.VALIDATE_INVALID;
                    return Json(jm, JsonRequestBehavior.AllowGet);
                }

                var identityVerification = identityVerificationBll.GetEntity(i => i.AppUserId == owner.Id && i.BuildDoor.BuildUnit.Build.PropertyPlaceId == place.Id);
                if (identityVerification != null)
                {
                    //如果存在审核中或已通过的验证信息
                    if (identityVerification.IsVerified != 2)
                    {
                        jm.Msg = APIMessage.VerifingOrYES;
                        return Json(jm, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        identityVerification.DoorId = model.DoorId;
                        identityVerification.Name = model.OwnerName;
                        identityVerification.Phone = model.Phone;
                        //如果该小区不需要审批身份
                        if (place.IsValidate == 1)
                        {
                            identityVerification.IsVerified = ConstantParam.IsVerified_YES;
                        }
                        else
                        {
                            identityVerification.IsVerified = ConstantParam.IsVerified_DEFAULT;
                        }
                        //更新验证申请
                        identityVerificationBll.Update(identityVerification);
                    }
                }
                else
                {
                    identityVerification = new R_PropertyIdentityVerification()
                    {
                        AppUserId = owner.Id,
                        Name = model.OwnerName,
                        Phone = model.Phone,
                        DoorId = model.DoorId,
                    };
                    //如果该小区不需要审批身份
                    if (place.IsValidate == 1)
                    {
                        identityVerification.IsVerified = ConstantParam.IsVerified_YES;
                    }
                    else
                    {
                        identityVerification.IsVerified = ConstantParam.IsVerified_DEFAULT;
                    }
                    //保存验证申请
                    identityVerificationBll.Save(identityVerification);
                }

            }
            //如果小区是办公楼小区
            else
            {
                var identityVerification = identityVerificationBll.GetEntity(i => i.AppUserId == owner.Id && i.BuildCompany.PropertyPlaceId == place.Id);
                if (identityVerification != null)
                {
                    //如果存在审核中或已通过的验证信息
                    if (identityVerification.IsVerified != 2)
                    {
                        jm.Msg = APIMessage.VerifingOrYES;
                        return Json(jm, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        identityVerification.BuildCompanyId = model.DoorId;
                        identityVerification.Phone = model.Phone;
                        //如果该小区不需要审批身份
                        if (place.IsValidate == 1)
                        {
                            identityVerification.IsVerified = ConstantParam.IsVerified_YES;
                        }
                        else
                        {
                            identityVerification.IsVerified = ConstantParam.IsVerified_DEFAULT;
                        }
                        //更新验证申请
                        identityVerificationBll.Update(identityVerification);
                    }
                }
                else
                {
                    identityVerification = new R_PropertyIdentityVerification()
                    {
                        AppUserId = owner.Id,
                        Phone = model.Phone,
                        BuildCompanyId = model.DoorId,
                    };
                    //如果该小区不需要审批身份
                    if (place.IsValidate == 1)
                    {
                        identityVerification.IsVerified = ConstantParam.IsVerified_YES;
                    }
                    else
                    {
                        identityVerification.IsVerified = ConstantParam.IsVerified_DEFAULT;
                    }
                    //保存验证申请
                    identityVerificationBll.Save(identityVerification);
                }
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除绑定的小区
        /// </summary>
        /// <param name="id">小区ID</param>
        /// <returns></returns>
        public JsonResult DelPlace(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                var owner = GetCurrentUser();
                IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                var place = placeBll.GetEntity(p => p.Id == id && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //如果该用户要删除的小区不存在
                if (place == null || owner.UserPlaces.Count(up => up.PropertyPlaceId == place.Id) <= 0)
                {
                    jm.Msg = "该用户未绑定该小区";
                }
                else
                {
                    //删除拥有小区的验证
                    IPropertyIdentityVerificationBLL identityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");
                    if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
                    {
                        var identityVerification = identityVerificationBll.GetEntity(v => v.AppUserId == owner.Id && v.DoorId != null && v.BuildDoor.BuildUnit.Build.PropertyPlaceId == place.Id);
                        if (identityVerification != null)
                        {
                            identityVerificationBll.Delete(identityVerification);
                        }
                    }
                    else
                    {
                        var identityVerification = identityVerificationBll.GetEntity(v => v.AppUserId == owner.Id && v.BuildCompanyId != null && v.BuildCompany.PropertyPlaceId == place.Id);
                        if (identityVerification != null)
                        {
                            identityVerificationBll.Delete(identityVerification);
                        }
                    }
                    //删除拥有的小区
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    ownerBll.ExecuteSql("delete from R_UserPlace where UserId=" + owner.Id + " and PropertyPlaceId=" + id);
                }
            }
            catch
            {
                jm.Msg = "删除发生异常";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectCityModel> GetCityList()
        {
            //获取物业小区列表
            ICityBLL cityBll = BLLFactory<ICityBLL>.GetBLL("CityBLL");
            var list = cityBll.GetList(c => c.PropertyPlaces.Count(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT) > 0).ToList();

            //转换为下拉列表并返回
            return list.Select(m => new SelectCityModel()
            {
                CityId = m.Id,
                CityName = m.CityName,
                PlaceCount = m.PropertyPlaces.Count
            }).OrderByDescending(c => c.PlaceCount).ToList();
        }

        /// <summary>
        /// 获取物业小区列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<PlaceListModel> GetPlaceList(int cityId)
        {
            //获取物业小区列表
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            var list = placeBll.GetList(p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && p.CityId == cityId).Select(m => new PlaceListModel()
            {
                PlaceId = m.Id,
                PlaceName = m.Name,
                FirstLetter = GetFirstLetter(m.Name)
            }).ToList();
            return list;
        }


        /// <summary>
        /// 获取小区楼座下拉列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetBuildList(int placeId)
        {
            //获取楼座列表
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var list = buildBll.GetList(b => b.PropertyPlaceId == placeId);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.BuildName,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取文本的首字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetFirstLetter(string str)
        {
            char firstChinese = str.FirstOrDefault();

            //如果是数字，则直接返回 
            if (firstChinese >= '0' && firstChinese <= '9')
            {
                return "#";
            }
            //如果是字母，则直接返回 
            if ((firstChinese >= 'a' && firstChinese <= 'z') || (firstChinese >= 'A' && firstChinese <= 'Z'))
            {
                return firstChinese.ToString().ToUpper();
            }

            ChineseChar chineseChar = new ChineseChar(firstChinese);
            //因为一个汉字可能有多个读音，pinyins是一个集合
            var pinyins = chineseChar.Pinyins;

            String firstPinyin = null;
            //下面的方法只是简单的获得了集合中第一个非空元素
            foreach (var pinyin in pinyins)
            {
                if (pinyin != null)
                {
                    //拼音的最后一个字符是音调
                    firstPinyin = pinyin.Substring(0, 1);
                    break;
                }
            }
            return firstPinyin;
        }

        /// <summary>
        /// 获取小区办公楼单位下拉列表
        /// </summary>
        /// <returns>小区列表</returns>
        private List<SelectListItem> GetBuildCompanyList(int placeId)
        {
            //获取楼座列表
            IBuildCompanyBLL buildCompanyBll = BLLFactory<IBuildCompanyBLL>.GetBLL("BuildCompanyBLL");
            var list = buildCompanyBll.GetList(b => b.PropertyPlaceId == placeId && b.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //转换为下拉列表并返回
            return list.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// 获取验证状态
        /// </summary>
        /// <param name="Place">要验证的小区</param>
        /// <param name="User">注册用户</param>
        /// <returns>-1:未申请 0：审核中 1：已通过 2：已驳回</returns>
        private int GetVerifyStatus(T_PropertyPlace Place, T_User User)
        {
            //如果是住宅小区
            if (Place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.DoorId != null && v.BuildDoor.BuildUnit.Build.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification == null)
                {
                    return -1;
                }
                else
                {
                    return IdentityVerification.IsVerified;
                }
            }
            else
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.BuildCompany.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification == null)
                {
                    return -1;
                }
                else
                {
                    return IdentityVerification.IsVerified;
                }
            }
        }
    }
}
