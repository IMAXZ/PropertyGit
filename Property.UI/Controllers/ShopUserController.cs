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
    /// 门店用户管理控制器
    /// </summary>
    public class ShopUserController : BaseController
    {
        /// <summary>
        /// 门店用户列表
        /// </summary>
        /// <param name="Model">门店用户查询模型</param>
        /// <returns>结果返回</returns>
        [BreadCrumb(Label = "门店用户列表")]
        [HttpGet]
        public ActionResult ShopUserList(SearchModel model)
        {
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            Expression<Func<T_ShopUser, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, u => u.UserName.Contains(model.Kword) || u.TrueName.Contains(model.Kword) || u.Email.Contains(model.Kword));
            }
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = shopUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex);
            return View(list);
        }

        /// <summary>
        /// 新增门店用户
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增门店用户")]
        [HttpGet]
        public ActionResult AddShopUser()
        {
            ShopUserModel Model = new ShopUserModel();
            Model.GenderList = GetGenderList();
            return View(Model);
        }

        /// <summary>
        /// 新增门店用户
        /// </summary>
        /// <param name="Model">门店用户模型</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddShopUser(ShopUserModel Model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser shopUser = new T_ShopUser()
                {
                    UserName = Model.UserName,
                    TrueName = Model.TrueName,
                    Phone = Model.Phone,
                    Gender = Model.Gender,
                    Email = Model.Email,
                    Password = PropertyUtils.GetMD5Str(Model.Password),
                    Memo = Model.Memo
                };
                //保存到数据库
                shopUserBll.Save(shopUser);
                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(Model);
            }
            else
            {
                //保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑门店用户
        /// </summary>
        /// <param name="id">门店用户id</param>
        /// <returns></returns>
        [BreadCrumb(Label = "编辑门店用户")]
        [HttpGet]
        public ActionResult EditShopUser(int id)
        {
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            //获取要编辑的门店用户
            T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (shopUser != null)
            {
                //初始化返回页面的模型
                ShopUserModel model = new ShopUserModel()
                {
                    Id = shopUser.Id,
                    UserName = shopUser.UserName,
                    TrueName = shopUser.TrueName,
                    Phone = shopUser.Phone,
                    Gender = shopUser.Gender,
                    GenderList = GetGenderList(),
                    Email = shopUser.Email,
                    Memo = shopUser.Memo
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("ShopUserList");
            }
        }

        /// <summary>
        /// 编辑门店用户
        /// </summary>
        /// <param name="model">门店用户模型</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditShopUser(ShopUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
                T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (shopUser != null)
                {
                    shopUser.UserName = model.UserName;
                    shopUser.TrueName = model.TrueName;
                    shopUser.Email = model.Email;
                    shopUser.Gender = model.Gender;
                    shopUser.Phone = model.Phone;
                    shopUser.Memo = model.Memo;
                    //保存到数据库
                    if (shopUserBll.Update(shopUser))
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
                    jm.Msg = "该门店用户不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除门店用户
        /// </summary>
        /// <param name="id">门店用户id</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        public JsonResult DeleteShopUser(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要删除的门店用户
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            //如果该门店用户存在
            if (shopUser == null)
            {
                jm.Msg = "该门店用户不存在";
            }
            else
            {
                //修改门店用户中的已删除标识
                shopUser.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                shopUserBll.DeleteShopUser(shopUser);
                //操作日志
                jm.Content = "删除门店用户" + shopUser.UserName;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetPassword(int id)
        {
            JsonModel jm = new JsonModel();
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            // 根据指定id值获取实体对象
            var shopUser = shopUserBll.GetEntity(index => index.Id == id && index.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (shopUser != null)
            {
                Random r = new Random();
                int radVal = r.Next(100, 1000);
                shopUser.Password = PropertyUtils.GetMD5Str(shopUser.UserName + radVal);
                //重置密码
                shopUserBll.Update(shopUser);

                //给门店用户发送邮件
                PropertyUtils.SendEmail(shopUser.Email, shopUser.UserName, "物业生活管理系统 用户密码重置", "您的用户密码已重置为" + shopUser.UserName + radVal + ", 请及时修改密码！");
                //操作日志
                jm.Content = "门店用户" + shopUser.TrueName + "密码一键重置成功";
            }
            else
            {
                jm.Msg = "该门店用户不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建门店
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "创建门店")]
        [HttpGet]
        public ActionResult AddShop(int Id)
        {
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            T_ShopUser shopUser = shopUserBll.GetEntity(m => m.Id == Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (shopUser != null)
            {
                ShopModel model = new ShopModel();
                model.TypeList = GetTypeList();
                model.ProvinceList = GetProvinceList();
                model.CityList = new List<SelectListItem>();
                model.CountyList = new List<SelectListItem>();
                model.TimeList = GetBusinessTimeList();
                model.ShopUserId = Id;
                model.ShopUserName = shopUser.UserName;
                return View(model);
            }
            else
            {
                return RedirectToAction("ShopUserList");
            }
        }

        /// <summary>
        /// 创建门店提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddShop(ShopModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IShopBLL ShopBll = BLLFactory<IShopBLL>.GetBLL("ShopBLL");
                //初始化门店数据
                T_Shop newShop = new T_Shop()
                {
                    ShopName = model.ShopName,
                    Phone = model.Tel,
                    ProvinceId = model.ProvinceId,
                    CityId = model.CityId,
                    CountyId = model.CountyId,
                    Address = model.Address,
                    MainSale = model.MainSale,
                    ShopUserId = model.ShopUserId,
                    Content = model.Content,
                    StartBusinessTime = model.StartBusinessTime,
                    EndBusinessTime = model.EndBusinessTime,
                    UpdateTime = DateTime.Now,
                    Type = model.Types
                };

                //如果服务小区不为空
                if (!string.IsNullOrEmpty(model.PlaceIds))
                {
                    foreach (var placeId in model.PlaceIds.Split(','))
                    {
                        newShop.ShopPlaces.Add(new R_ShopPlace()
                        {
                            PropertyPlaceId = Convert.ToInt32(placeId)
                        });
                    }
                }
                //默认支付方式：货到现金支付
                newShop.ShopPaymentManagements.Add(new T_ShopPaymentManagement()
                {
                    PayTypeId = 3
                });
                // 保存到数据库
                ShopBll.Save(newShop);
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
        /// 获取营业时间列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetBusinessTimeList()
        {
            // 开始列表
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i <= 24; i++)
            {
                list.Add(new SelectListItem()
                {
                    Text = i + " : 00",
                    Value = i.ToString(),
                    Selected = false
                });
            }
            return list;
        }

        /// <summary>
        /// 远程验证门店用户名称是否存在
        /// </summary>
        /// <param name="UserName">门店用户名称</param>
        /// <param name="UserId">用户id,新增时恒为0，修改用户信息时不为0</param>
        public ContentResult RemoteCheckExist(int Id, string userName)
        {
            IShopUserBLL shopUserBll = BLLFactory<IShopUserBLL>.GetBLL("ShopUserBLL");
            // 用户名已存在
            if (shopUserBll.Exist(m => m.UserName == userName && m.Id != Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
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
        /// 性别列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetGenderList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Text = "女",
                Value = ConstantParam.GENDER_ZERO.ToString(),
                Selected = false
            });
            list.Add(new SelectListItem()
            {
                Text = "男",
                Value = ConstantParam.GENDER_ONE.ToString(),
                Selected = false
            });
            return list;
        }


        /// <summary>
        /// 获取门店类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetTypeList()
        {
            // 类型列表
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_0,
                Value = ConstantParam.SHOP_TYPE_0.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_1,
                Value = ConstantParam.SHOP_TYPE_1.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_2,
                Value = ConstantParam.SHOP_TYPE_2.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = ConstantParam.SHOP_TYPE_String_3,
                Value = ConstantParam.SHOP_TYPE_3.ToString(),
                Selected = false
            });
            return typeList;
        }
    }
}
