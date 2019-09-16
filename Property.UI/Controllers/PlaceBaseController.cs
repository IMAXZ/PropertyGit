using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 物业小区控制器基类
    /// </summary>
    public class PlaceBaseController : BaseController
    {
        /// <summary>
        /// 获取小区列表
        /// </summary>
        /// <param name="model">物业小区查询模型</param>
        /// <returns></returns>
        public PropertyPlaceSearchModel GetPlaceList(PropertyPlaceSearchModel model)
        {
            model.PlaceTypeList = GetPlaceTypeList(null);
            //查询条件
            Expression<Func<T_PropertyPlace, bool>> where = u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
            //如果查询的物业公司ID不为空
            if (model.CompanyId != null)
            {
                where = PredicateBuilder.And(where, u => u.CompanyId == model.CompanyId);
            }
            if (model.PlaceType != null)
            {
                where = PredicateBuilder.And(where, u => u.PlaceType == model.PlaceType.Value);
            }
            if (!string.IsNullOrEmpty(model.PlaceName))
            {
                where = PredicateBuilder.And(where, u => u.Name.Contains(model.PlaceName));
            }

            //排序
            var sortModel = this.SettingSorting("Id", false);
            //将查询到的数据赋值传到页面
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            model.DataList = placeBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_PropertyPlace>;
            return model;
        }

        /// <summary>
        /// 添加小区
        /// </summary>
        public JsonModel AddPlace(PropertyPlaceModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyCompanyBLL companyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
                //如果对应的公司不存在（被删除）
                if (!companyBll.Exist(c => c.Id == model.CompanyId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    jm.Msg = "物业总公司不存在";
                }
                else
                {
                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    T_PropertyPlace place = new T_PropertyPlace()
                    {
                        Name = model.PlaceName,
                        CompanyId = model.CompanyId,
                        ProvinceId = model.ProvinceId,
                        CityId = model.CityId,
                        CountyId = model.CountyId,
                        Address = model.Address,
                        Longitude = model.Longitude,
                        Latitude = model.Latitude,
                        Tel = model.Tel,
                        Content = model.Content,
                        PlaceType = model.PlaceType,
                        IsValidate = model.IsValidate ? 0 : 1
                    };
                    //添加小区并指定系统角色
                    placeBll.AddPlace(place);
                    //日志记录
                    jm.Content = PropertyUtils.ModelToJsonString(model);
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return jm;
        }

        /// <summary>
        /// 初始化小区编辑模型
        /// </summary>
        public PropertyPlaceModel InitPlaceEditModel(T_PropertyPlace place)
        {
            //创建物业小区模型并赋值
            PropertyPlaceModel model = new PropertyPlaceModel()
            {
                PlaceId = place.Id,
                PlaceName = place.Name,
                CompanyId = place.CompanyId,
                ProvinceId = place.ProvinceId,
                ProvinceList = GetProvinceList(),
                CityId = place.CityId,
                CityList = base.GetCityList(place.ProvinceId),
                CountyId = place.CountyId,
                CountyList = base.GetCountyList(place.CityId),
                Address = place.Address,
                Longitude = place.Longitude,
                Latitude = place.Latitude,
                Tel = place.Tel,
                Content = place.Content,
                PlaceType = place.PlaceType,
                PlaceTypeList = GetPlaceTypeList(place.PlaceType),
                IsValidate = place.IsValidate == 0
            };
            return model;
        }

        /// <summary>
        /// 编辑小区信息
        /// </summary>
        public JsonModel EditPlace(PropertyPlaceModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyCompanyBLL companyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
                //如果对应的公司不存在（被删除）
                if (!companyBll.Exist(c => c.Id == model.CompanyId && c.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    jm.Msg = "当前物业总公司不存在";
                }
                else
                {
                    //获取指定ID且未删除的小区
                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    T_PropertyPlace place = placeBll.GetEntity(m => m.Id == model.PlaceId && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                    if (place != null)
                    {
                        //重新给数据实体赋值
                        place.Name = model.PlaceName;
                        place.CompanyId = model.CompanyId;
                        place.ProvinceId = model.ProvinceId;
                        place.CityId = model.CityId;
                        place.CountyId = model.CountyId;
                        place.Address = model.Address;
                        place.Longitude = model.Longitude;
                        place.Latitude = model.Latitude;
                        place.Tel = model.Tel;
                        place.Content = model.Content;
                        place.PlaceType = model.PlaceType;
                        place.IsValidate = model.IsValidate ? 0 : 1;
                        //编辑
                        if (placeBll.Update(place))
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
                        jm.Msg = "该物业小区不存在";
                    }
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return jm;
        }

        /// <summary>
        /// 删除小区
        /// </summary>
        /// <param name="id">小区ID</param>
        /// <returns></returns>
        public JsonModel DelPlace(int id)
        {
            JsonModel jm = new JsonModel();

            //获取指定ID且未删除的小区
            IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = placeBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //如果该小区存在
            if (place == null)
            {
                jm.Msg = "该物业小区不存在";
            }
            else
            {
                place.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                if (placeBll.DeletePlace(place))
                {
                    //操作日志
                    jm.Content = "删除物业小区 " + place.Name;
                }
                else
                {
                    jm.Msg = "删除失败";
                }
            }
            return jm;
        }

        /// <summary>
        /// 设置管理员提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonModel SetAdmin(PropertyUserModel model)
        {
            JsonModel jm = new JsonModel();

            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser propertyUser = new T_PropertyUser()
                {
                    PropertyPlaceId = model.PlaceId,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = PropertyUtils.GetMD5Str(model.Password),
                    IsMgr = ConstantParam.USER_ROLE_MGR,
                    DelFlag = ConstantParam.DEL_FLAG_DEFAULT,
                };

                //为管理员添加角色
                IPropertyRoleBLL roleBll = BLLFactory<IPropertyRoleBLL>.GetBLL("PropertyRoleBLL");
                var role = roleBll.GetEntity(r => r.IsSystem == ConstantParam.USER_ROLE_MGR && r.PropertyPlaceId == model.PlaceId);
                if (role != null)
                {
                    propertyUser.PropertyUserRoles.Add(new R_PropertyUserRole()
                    {
                        RoleId = role.Id,
                    });
                }
                //创建管理员
                propertyUserBll.Save(propertyUser);

                //日志记录
                jm.Content = PropertyUtils.ModelToJsonString(model);
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return jm;
        }

        /// <summary>
        /// 上传小区图标
        /// </summary>
        /// <param name="data">图片数据</param>
        /// <param name="id">小区ID</param>
        /// <returns></returns>
        public JsonModel UploadPlaceImg(string data, int id)
        {
            JsonModel jm = new JsonModel();

            string directory = Server.MapPath(ConstantParam.PROPERTY_PLACE_DIR);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
            var path = Path.Combine(directory, fileName);

            using (FileStream fs = new FileStream(path, FileMode.Create))//使用指定的路径初始化实例
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] datas = Convert.FromBase64String(data);
                    bw.Write(datas);
                    bw.Close();
                }
            }

            if (!Directory.Exists(Server.MapPath(ConstantParam.PROPERTY_PLACE_ThumIMG_DIR)))
            {
                Directory.CreateDirectory(Server.MapPath(ConstantParam.PROPERTY_PLACE_ThumIMG_DIR));
            }

            //生成缩略图
            string thumpFile = DateTime.Now.ToFileTime() + ".jpg";
            var thumpPath = Path.Combine(Server.MapPath(ConstantParam.PROPERTY_PLACE_ThumIMG_DIR), thumpFile);
            PropertyUtils.getThumImage(path, 18, 3, thumpPath);

            IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
            T_PropertyPlace place = propertyPlaceBll.GetEntity(m => m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && m.Id == id);

            //物业小区存在
            if (place != null)
            {
                string oldFile = place.Img;
                string oldThumFile = place.ImgThumbnail;

                place.Img = ConstantParam.PROPERTY_PLACE_DIR + fileName;
                place.ImgThumbnail = ConstantParam.PROPERTY_PLACE_ThumIMG_DIR + thumpFile;
                if (propertyPlaceBll.Update(place))
                {
                    //删除旧图标
                    if (!string.IsNullOrEmpty(oldFile))
                    {
                        //FileInfo f = new FileInfo(Server.MapPath(oldFile));
                        //if (f.Exists)
                        //    f.Delete();
                    }
                    //删除旧缩略图标
                    if (!string.IsNullOrEmpty(oldThumFile))
                    {
                        FileInfo m = new FileInfo(Server.MapPath(oldThumFile));
                        if (m.Exists)
                            m.Delete();
                    }
                }
            }
            //物业小区不存在
            else
            {
                jm.Msg = "物业小区不存在";
            }
            return jm;
        }


        /// <summary>
        /// 获取物业公司列表
        /// </summary>
        /// <returns>物业公司下拉列表</returns>
        public List<SelectListItem> GetCompanyList()
        {
            //获取所有的物业公司列表
            IPropertyCompanyBLL companyBll = BLLFactory<IPropertyCompanyBLL>.GetBLL("PropertyCompanyBLL");
            var list = companyBll.GetList(u => u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

            //转换为下拉列表
            List<SelectListItem> companyList = list.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = false,
            }).ToList();

            return companyList;
        }

        /// <summary>
        /// 获取小区类型下拉列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPlaceTypeList(int? PlaceType = 0)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "住宅小区",
                Value = ConstantParam.PLACE_TYPE_HOUSE.ToString(),
                Selected = PlaceType == ConstantParam.PLACE_TYPE_HOUSE
            });
            list.Add(new SelectListItem()
            {
                Text = "办公楼小区",
                Value = ConstantParam.PLACE_TYPE_COMPANY.ToString(),
                Selected = PlaceType == ConstantParam.PLACE_TYPE_COMPANY
            });
            return list;
        }
    }
}
