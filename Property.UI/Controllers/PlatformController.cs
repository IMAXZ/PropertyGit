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
    /// 平台管理后台首页
    /// </summary>
    public class PlatformController : BaseController
    {

        /// <summary>
        /// 后台管理首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BreadCrumb(Label = "后台管理首页")]
        public ActionResult Index(int id = 1)
        {
            PlatformIndexModel model = new PlatformIndexModel();

            #region 用户、公司、小区、业主个数统计

            //获取平台用户个数并赋值
            IPlatformUserBLL platformUserBll = BLLFactory<IPlatformUserBLL>.GetBLL("PlatformUserBLL");
            model.PlatformUserCount = platformUserBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取物业公司个数并赋值
            IPropertyCompanyBLL companyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            model.CompanyCount = companyBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取物业小区个数并赋值
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            model.PlaceCount = placeBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取业主个数并赋值
            IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
            model.OwnerCount = ownerBll.Count(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //获取一周内登录个数
            DateTime beforeWeek = DateTime.Now.AddDays(-7);
            model.OwnerWeekLoginCount = ownerBll.Count(u => u.LatelyLoginTime >= beforeWeek);

            //获取一个月内登录个数
            DateTime beforeMonth = DateTime.Now.AddMonths(-1);
            model.OwnerMonthLoginCount = ownerBll.Count(u => u.LatelyLoginTime >= beforeMonth);

            #endregion

            #region 公司小区个数及业主个数统计（柱状图）

            //公司小区、业主个数模型集合
            var barDatas = new List<BarDataModel>();

            //获取物业公司名称及下辖小区个数及业主个数 数据并转换为Json字符串
            var companyList = companyBll.GetList(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList();
            foreach (var c in companyList)
            {
                BarDataModel data = new BarDataModel();
                //物业公司名称
                data.CompanyName = c.Name;
                var placeList = c.PropertyPlaces.Where(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //该物业公司的小区个数
                data.PlaceCount = placeList.Count();

                var userPlaces = new List<R_UserPlace>();
                //遍历物业小区
                foreach (var p in placeList)
                {
                    //遍历物业小区业主关联
                    foreach (var u in p.UserPlaces.ToList())
                    {
                        //如果不包含，则添加
                        if (!userPlaces.Contains(u))
                        {
                            userPlaces.Add(u);
                        }
                    }
                }
                //赋值 业主个数
                data.OwnerCount = userPlaces.Count;
                barDatas.Add(data);
            }
            model.BarJsonData = PropertyUtils.ModelToJsonString(barDatas);
            #endregion

            #region 小区业主列表数据
            //获取所有小区业主数据
            var list = placeBll.GetList(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).ToList().Select(p => new OwnerDataModel()
            {
                PlaceName = p.Name,
                CompanyName = p.Company.Name,
                OwnerCount = p.UserPlaces.Count
            });
            //降序排序分页并赋值
            model.OwnerDatas = list.OrderByDescending(m => m.OwnerCount).ToPagedList(id, ConstantParam.PAGE_SIZE);
            #endregion

            return View(model);
        }
    }
}
