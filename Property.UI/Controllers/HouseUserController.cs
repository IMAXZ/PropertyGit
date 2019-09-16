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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Property.UI.Controllers
{
    /// <summary>
    /// 住宅业主管理控制器
    /// </summary>
    public class HouseUserController : BaseController
    {
        /// <summary>
        /// 住宅业主管理列表
        /// </summary>
        /// <param name="Model">住宅业主查询模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "住宅业主列表")]
        [HttpGet]
        public ActionResult HouseUserList(HouseUserSearchModel Model)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            int propertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
            Expression<Func<T_HouseUser, bool>> where = u => (string.IsNullOrEmpty(Model.Name) ? true : u.Name.Contains(Model.Name)) && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT && u.PropertyPlaceId == propertyPlaceId;
            //排序
            var sortModel = this.SettingSorting("Id", false);
            var list = houseUserBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, Model.PageIndex);
            return View(list);
        }

        /// <summary>
        /// 新增住宅业主
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增住宅业主")]
        [HttpGet]
        public ActionResult AddHouseUser()
        {
            HouseUserModel model = new HouseUserModel();
            model.GenderList = GetGenderList();
            model.BuildList = GetBuildList();
            model.UnitList = new List<SelectListItem>();
            model.DoorList = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// 新增住宅业主
        /// </summary>
        /// <param name="model">住宅业主数据模型</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public JsonResult AddHouseUser(HouseUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
                T_HouseUser houseuser = new T_HouseUser()
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Gender = model.Gender,
                    DoorId = model.DoorId,
                    Desc = model.Desc,
                    PayDesc = model.PayDesc,
                    ServiceDesc = model.ServiceDesc,
                    PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value,
                };
                //保存
                houseUserBll.Save(houseuser);

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
        /// 住宅业主详细
        /// </summary>
        /// <param name="id">住宅业主Id</param>
        /// <returns>结果返回</returns>
        [BreadCrumb(Label = "住宅业主详细")]
        [HttpGet]
        public ActionResult DetailHouseUser(int id)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            //获取要查看的住宅业主
            T_HouseUser houseUser = houseUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (houseUser != null)
            {
                return View(houseUser);
            }
            else
            {
                return RedirectToAction("HouseUserList");
            }
        }

        /// <summary>
        /// 编辑住宅业主
        /// </summary>
        /// <param name="id">住宅业主Id</param>
        /// <returns>结果返回</returns>
        [BreadCrumb(Label = "编辑住宅业主")]
        [HttpGet]
        public ActionResult EditHouseUser(int id)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            //获取要编辑的住宅业主
            T_HouseUser houseUser = houseUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (houseUser != null)
            {
                //初始化返回页面模型
                HouseUserModel model = new HouseUserModel()
                {
                    Name = houseUser.Name,
                    Phone = houseUser.Phone,
                    BuildId = houseUser.BuildDoor.BuildUnit.BuildId,
                    BuildList = GetBuildList(),
                    UnitId = houseUser.BuildDoor.UnitId,
                    UnitList = GetUnitList(houseUser.BuildDoor.BuildUnit.BuildId),
                    DoorId = houseUser.DoorId,
                    DoorList = GetDoorList(houseUser.BuildDoor.UnitId),
                    Gender = houseUser.Gender,
                    GenderList = GetGenderList(),
                    Desc = houseUser.Desc,
                    PayDesc = houseUser.PayDesc,
                    ServiceDesc = houseUser.ServiceDesc
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("HouseUserList");
            }
        }

        /// <summary>
        /// 编辑住宅业主
        /// </summary>
        /// <param name="model">住宅业主模型</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditHouseUser(HouseUserModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单验证成功
            if (ModelState.IsValid)
            {
                IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
                T_HouseUser houseUser = houseUserBll.GetEntity(m => m.Id == model.Id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (houseUser != null)
                {
                    houseUser.Name = model.Name;
                    houseUser.Phone = model.Phone;
                    houseUser.PropertyPlaceId = GetSessionModel().PropertyPlaceId.Value;
                    houseUser.BuildDoor.BuildUnit.BuildId = model.BuildId;
                    houseUser.BuildDoor.UnitId = model.UnitId;
                    houseUser.DoorId = model.DoorId;
                    houseUser.Gender = model.Gender;
                    houseUser.Desc = model.Desc;
                    houseUser.PayDesc = model.PayDesc;
                    houseUser.ServiceDesc = model.ServiceDesc;
                    //保存到数据库
                    if (houseUserBll.Update(houseUser))
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
                    jm.Msg = "该住宅业主不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除业主住宅
        /// </summary>
        /// <param name="id">业主住宅Id</param>
        /// <returns>结果返回</returns>
        [HttpPost]
        public JsonResult DeleteHouseUser(int id)
        {
            JsonModel jm = new JsonModel();
            //获取要删除的住宅业主
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            T_HouseUser houseUser = houseUserBll.GetEntity(m => m.Id == id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
            if (houseUser == null)
            {
                jm.Msg = "该住宅业主不存在";
            }
            else
            {
                //修改住宅业主的已删除标识
                houseUser.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                //更新数据库
                houseUserBll.Update(houseUser);
                //操作日志
                jm.Content = "删除住宅业主" + houseUser.Name;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证指定业主住宅单元户是否存在
        /// </summary>
        /// <param name="doorId">户id</param>
        /// <param name="Id">用户id,新增时恒为0，修改用户信息时不为0</param>
        [HttpPost]
        public ContentResult RemoteCheckExist(int doorId, int id)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            // 业主住宅单元户已存在
            if (houseUserBll.Exist(m => m.DoorId == doorId && m.Id != id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                // 校验不通过
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }

        #region 楼盘,单元,户 下拉列表

        /// <summary>
        /// 获取楼盘列表
        /// </summary>
        /// <returns>楼盘列表</returns>
        private List<SelectListItem> GetBuildList()
        {
            //获取楼座列表
            IBuildBLL BuildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");
            var sortModel = this.SettingSorting("Id", true);
            int placeId = GetSessionModel().PropertyPlaceId.Value;
            var list = BuildBll.GetList(u => u.PropertyPlaceId == placeId, sortModel.SortName, sortModel.IsAsc);

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

        #region 性别下拉列表
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
        #endregion

        #region 业主信息导入
        /// <summary>
        /// 业务信息导入主页
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "业主信息导入")]
        [HttpGet]
        public ActionResult ImportHouseUsers()
        {
            var ImportUsersModel = new ImportUsersModel();

            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "ImportData/ImportTemplate/住宅业主信息导入模板.xlsx";
            FileInfo file = new FileInfo(filePath);
            ImportUsersModel.HasImportTemplate = file.Exists;

            return View(ImportUsersModel);
        }
        /// <summary>
        /// 执行住宅业主信息导入
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ImportHouseUsers(ImportUsersModel Model)
        {
            JsonModel jm = new JsonModel();

            HttpPostedFileBase file = Model.file;
            //Model.file
            //存入的文件名
            string FileName;
            //存入的文件路径
            string savePath;

            string filename = Path.GetFileName(file.FileName);
            string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
            //string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
            string FileType = ".xls,.xlsx";//定义上传文件的类型字符串

            if (!FileType.Contains(fileEx))
            {
                jm.Msg = "文件类型只能是xlsx格式的文件";
                jm.Code = "error";
                return Json(jm, JsonRequestBehavior.AllowGet);
            }

            FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileEx;

            //存一份及时要导入的数据文件
            string path = AppDomain.CurrentDomain.BaseDirectory + "ImportData/ImportHouseUserData/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            savePath = Path.Combine(path, FileName);

            file.SaveAs(savePath);

            //住宅业主接口
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            //当前物业小区Id
            var placePropertyId = GetSessionModel().PropertyPlaceId.Value;
            //读取存在的要导入的EXCEL数据文件
            DataTable dtHouseUser = ImportExcelFile(savePath);
            //如果文件中没有内容
            if (dtHouseUser.Rows.Count == 0)
            {
                jm.Msg = "文件内无业主信息";
                return Json(jm, JsonRequestBehavior.AllowGet);
            }

            //有效业主信息列表
            var validateHouseUserlist = new List<T_HouseUser>();
            //无效业主信息列表
            var invalidateHouseUserList = new List<ImportHouseUserModel>();

            for (int i = 0; i < dtHouseUser.Rows.Count; i++)
            {
                //是否有效标记
                bool isValidated = true;

                var houseUserModel = new ImportHouseUserModel();

                //为导入的ImportHouseUserModel赋值，以便用于无效业主列表
                if (dtHouseUser.Rows[i][0] != DBNull.Value)
                {
                    houseUserModel.Name = dtHouseUser.Rows[i][0].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][1] != DBNull.Value)
                {
                    houseUserModel.Gender = dtHouseUser.Rows[i][1].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][2] != DBNull.Value)
                {
                    houseUserModel.Phone = dtHouseUser.Rows[i][2].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][3] != DBNull.Value)
                {
                    houseUserModel.BuildName = dtHouseUser.Rows[i][3].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][4] != DBNull.Value)
                {
                    houseUserModel.UnitName = dtHouseUser.Rows[i][4].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][5] != DBNull.Value)
                {
                    houseUserModel.DoorName = dtHouseUser.Rows[i][5].ToString().Trim();
                }

                if (dtHouseUser.Rows[i][6] != DBNull.Value)
                {
                    houseUserModel.Desc = dtHouseUser.Rows[i][6].ToString().Trim();
                }

                //如果业主姓名没有就是无效
                if (string.IsNullOrEmpty(houseUserModel.Name))
                {
                    isValidated = false;
                }

                //如果电话没有或者填写不正确就是无效
                if (string.IsNullOrEmpty(houseUserModel.Phone) || !CheckPhoneValidate(houseUserModel.Phone))
                {
                    isValidated = false;
                }

                //如果楼座名称没有就是无效
                if (string.IsNullOrEmpty(houseUserModel.BuildName))
                {
                    isValidated = false;
                }

                //如果单元名称没有就是无效
                if (string.IsNullOrEmpty(houseUserModel.UnitName))
                {
                    isValidated = false;
                }

                //如果单元户名字没有就是无效
                if (string.IsNullOrEmpty(houseUserModel.DoorName))
                {
                    isValidated = false;
                }

                //获取楼座ID，如果没有将新建
                int buildId = GetBuildId(houseUserModel.BuildName, placePropertyId);
                //获取单元ID，如果没有将新建
                int unitId = GetBuildUnitId(houseUserModel.UnitName, buildId);
                //获取单元户ID，如果没有将新建
                int doorId = GetBuildDoorId(houseUserModel.DoorName, unitId);

                //楼座，单元，单元户有一个为0，就是无效数据
                if (buildId == 0 || unitId == 0 || doorId == 0)
                {
                    isValidated = false;
                }

                //如果单元户已经存在也是无效
                if (CheckUserExist(doorId, 0))
                {
                    isValidated = false;
                }

                if (isValidated)
                {
                    var houseuser = new T_HouseUser();
                    //当前物业小区Id
                    houseuser.PropertyPlaceId = placePropertyId;
                    //业主姓名
                    houseuser.Name = houseUserModel.Name;
                    //业主电话
                    houseuser.Phone = houseUserModel.Phone;
                    //业主单元户
                    houseuser.DoorId = doorId;
                    //业主备注
                    houseuser.Desc = houseUserModel.Desc;

                    //业主性别（男，女）
                    if (houseUserModel.Gender == "男")
                    {
                        houseuser.Gender = 1;
                    }

                    if (houseUserModel.Gender == "女")
                    {
                        houseuser.Gender = 0;
                    }

                    //将有效的业主加入到有效的列表里面
                    validateHouseUserlist.Add(houseuser);

                }
                else
                {
                    //将无效的业主加入到无效的列表里面
                    invalidateHouseUserList.Add(houseUserModel);
                }
            }


            bool check = true;
            //前符合要求的导入数据库
            if (validateHouseUserlist.Count > 0)
            {
                check = houseUserBll.ImportHouseUsers(validateHouseUserlist);
            }

            //如果有业主信息导入成功
            if (validateHouseUserlist.Count > 0 && check)
            {
                //部分导入失败
                if (invalidateHouseUserList.Count > 0)
                {
                    jm.Msg = string.Format("业主信息导入成功{0}条，失败{1}条", validateHouseUserlist.Count, invalidateHouseUserList.Count);
                    jm.Content = GenerateInvalidateDataExcel(invalidateHouseUserList);
                }
                else 
                {
                    jm.Content = string.Format("{0}条业主信息全部导入成功", validateHouseUserlist.Count);
                }   
            }
            else
            {
                jm.Msg = "业主信息全部导入失败";
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取楼座Id, 如果没有将新建一个楼座
        /// </summary>
        /// <param name="buildName"></param>
        /// <param name="propertPlaceId"></param>
        /// <returns></returns>
        private static int GetBuildId(string buildName, int propertPlaceId)
        {
            int buildId = 0;
            //楼座接口
            IBuildBLL buildBll = BLLFactory<IBuildBLL>.GetBLL("BuildBLL");

            var build = buildBll.GetEntity(b => b.BuildName == buildName && b.PropertyPlaceId == propertPlaceId);

            if (build != null)
            {
                buildId = build.Id;
            }
            else
            {
                build = new T_Build()
                {
                    BuildName = buildName,
                    PropertyPlaceId = propertPlaceId,
                };
                try
                {
                    buildBll.Save(build);
                    buildId = build.Id;
                }
                catch
                {
                    buildId = 0;
                }
            }

            return buildId;
        }
        /// <summary>
        /// 获取单元Id, 如果没有将新建一个单元
        /// </summary>
        /// <param name="unitName"></param>
        /// <param name="buildId"></param>
        /// <returns></returns>
        private static int GetBuildUnitId(string unitName, int buildId)
        {
            int unitId = 0;
            //楼座接口
            IBuildUnitBLL unitBll = BLLFactory<IBuildUnitBLL>.GetBLL("BuildUnitBLL");

            var unit = unitBll.GetEntity(b => b.UnitName == unitName && b.BuildId == buildId);

            if (unit != null)
            {
                unitId = unit.Id;
            }
            else
            {
                unit = new T_BuildUnit()
                {
                    UnitName = unitName,
                    BuildId = buildId
                };
                try
                {
                    unitBll.Save(unit);
                    unitId = unit.Id;
                }
                catch
                {

                    unitId = 0;
                }
            }

            return unitId;
        }
        /// <summary>
        /// 获取单元户Id, 如果没有将新建一个单元户
        /// </summary>
        /// <param name="doorName"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        private static int GetBuildDoorId(string doorName, int unitId)
        {
            int doorId = 0;
            //楼座接口
            IBuildDoorBLL doorBll = BLLFactory<IBuildDoorBLL>.GetBLL("BuildDoorBLL");

            var door = doorBll.GetEntity(b => b.DoorName == doorName && b.UnitId == unitId);

            if (door != null)
            {
                doorId = door.Id;
            }
            else
            {
                door = new T_BuildDoor()
                {
                    DoorName = doorName,
                    UnitId = unitId
                };

                try
                {
                    doorBll.Save(door);
                    doorId = door.Id;
                }
                catch (Exception)
                {
                    doorId = 0;
                }
            }

            return doorId;
        }
        /// <summary>
        /// 将无效的业主信息重新生成到一个Excel文件里面，并返回保存的全路径加文件名以便于下载。
        /// </summary>
        /// <param name="invalidDataList"></param>
        /// <returns></returns>
        private static string GenerateInvalidateDataExcel(List<ImportHouseUserModel> invalidDataList)
        {
            //创建Excel文件的对象
            XSSFWorkbook book = new XSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = book.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("业主姓名");
            row1.CreateCell(1).SetCellValue("性别");
            row1.CreateCell(2).SetCellValue("手机");
            row1.CreateCell(3).SetCellValue("楼座");
            row1.CreateCell(4).SetCellValue("单元");
            row1.CreateCell(5).SetCellValue("单元户");
            row1.CreateCell(6).SetCellValue("业主备注");

            int i = 0;
            foreach (var invalidData in invalidDataList)
            {
                IRow rowtemp = sheet1.CreateRow(i + 1);

                rowtemp.CreateCell(0).SetCellValue(invalidData.Name);
                rowtemp.CreateCell(1).SetCellValue(invalidData.Gender);
                rowtemp.CreateCell(2).SetCellValue(invalidData.Phone);
                rowtemp.CreateCell(3).SetCellValue(invalidData.BuildName);
                rowtemp.CreateCell(4).SetCellValue(invalidData.UnitName);
                rowtemp.CreateCell(5).SetCellValue(invalidData.DoorName);
                rowtemp.CreateCell(6).SetCellValue(invalidData.Desc);

                i += 1;
            }

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string resposeFilePath = "/ImportData/InvalidateHouseUserData/" + fileName;
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "ImportData/InvalidateHouseUserData/";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = filePath + fileName;

            FileStream newfile = new FileStream(filePath, FileMode.Create);
            book.Write(newfile);
            newfile.Close();

            return resposeFilePath;
        }
        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable ImportExcelFile(string filePath)
        {
            XSSFWorkbook xssfworkbook;

            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch
            {
                return null;
            }
            #endregion

            ISheet sheet = xssfworkbook.GetSheetAt(0);

            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(0);//第一行为标题行
            int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
            int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1

            //handling header.
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = GetCellValue(row.GetCell(j));
                    }
                }

                table.Rows.Add(dataRow);
            }

            return table;
        }
        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        XSSFFormulaEvaluator e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        public bool CheckUserExist(int doorId, int id)
        {
            IHouseUserBLL houseUserBll = BLLFactory<IHouseUserBLL>.GetBLL("HouseUserBLL");
            // 业主住宅单元户已存在
            if (houseUserBll.Exist(m => m.DoorId == doorId && m.Id != id && m.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckPhoneValidate(string val)
        {
            bool isMatch = Regex.IsMatch(val, @"^1[34578]\d{9}$");
            return isMatch && val.Length <= 15;
        }
        /// <summary>
        /// 导入模板下载
        /// </summary>
        /// <returns></returns>
        public FilePathResult ImportTemplateDownload()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "ImportData/ImportTemplate/住宅业主信息导入模板.xlsx";
            FileInfo file = new FileInfo(filePath);

            // 如果文件不存在时
            if (!file.Exists)
            {
                return null;
            }

            return File(filePath, "application/octet-stream", "住宅业主信息导入模板.xlsx");
        }
        /// <summary>
        /// 导入不成功数据下载
        /// </summary>
        /// <returns></returns>
        public FilePathResult ExportInvalidHouseUser()
        {
            string filePath = string.Empty;

            if (Session["InvalidatePath"] != null)
            {
                filePath = Session["InvalidatePath"].ToString();

                FileInfo file = new FileInfo(filePath);

                // 如果文件不存在时
                if (!file.Exists)
                {
                    return null;
                }
            }

            return File(filePath, "application/octet-stream", "导入不成功数据.xlsx");
        }
        #endregion
    }
}
