using MvcBreadCrumbs;
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
    /// 客户端版本管理Controller
    /// </summary>
    public class MobileVersionController : BaseController
    {
        /// <summary>
        /// 历史版本列表
        /// </summary>
        /// <param name="model">版本查询模型</param>
        /// <returns></returns>
        [BreadCrumb(Label = "APP版本列表")]
        public ActionResult VersionList(MobileVersionSearchModel model)
        {
            IMobileVersionBLL mobileVersionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            Expression<Func<T_MobileVersion, bool>> where = u => (model.Type == null ? true : u.Type == model.Type) && (string.IsNullOrEmpty(model.Desc) ? true : u.Desc.Contains(model.Desc));
            //排序
            var sortModel = this.SettingSorting("Id", false);
            model.List = mobileVersionBll.GetPageList(where, sortModel.SortName, sortModel.IsAsc, model.PageIndex) as PagedList<T_MobileVersion>;
            model.TypeList = getTypeList();
            return View(model);
        }

        /// <summary>
        /// 返回版本类型列表
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> getTypeList()
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem()
            {
                Text = "业主端",
                Value = ConstantParam.MOBILE_TYPE_OWNER.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = "物业端",
                Value = ConstantParam.MOBILE_TYPE_PROPERTY.ToString(),
                Selected = false
            });
            typeList.Add(new SelectListItem()
            {
                Text = "商户端",
                Value = ConstantParam.MOBILE_TYPE_SHOP.ToString(),
                Selected = false
            });
            return typeList;
        }


        /// <summary>
        /// 添加版本记录
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Label = "新增APP版本")]
        public ActionResult AddVersion()
        {
            return View();
        }

        /// <summary>
        /// 添加客户端历史版本提交
        /// </summary>
        /// <param name="model">版本记录模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddVersion(MobileVersionModel model)
        {
            JsonModel jm = new JsonModel();
            //如果表单模型验证成功
            if (ModelState.IsValid)
            {
                string apkFilePath = "";
                // 判断apk文件夹是否存在
                if (Directory.Exists(Server.MapPath(ConstantParam.APK_FILE_DIR)) == false)
                {
                    Directory.CreateDirectory(Server.MapPath(ConstantParam.APK_FILE_DIR));
                }

                // 上传单个文件
                if (System.Web.HttpContext.Current.Request.Files.Count > 0)
                {
                    HttpPostedFile PostedFile = System.Web.HttpContext.Current.Request.Files[0];
                    if (PostedFile.ContentLength > 0)
                    {
                        string FileName = PostedFile.FileName;
                        string strExPrentFile = FileName.Substring(FileName.LastIndexOf(".") + 1);
                        var formatName = (model.Type == 0 ? "Owner_" : "Property_") + model.VersionCode;
                        apkFilePath = ConstantParam.APK_FILE_DIR + formatName + "." + strExPrentFile;
                        var path = Server.MapPath(apkFilePath);
                        PostedFile.SaveAs(path);

                        // 保存表单内容
                        IMobileVersionBLL mobileVersionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
                        T_MobileVersion newVersion = new T_MobileVersion()
                        {
                            VersionCode = model.VersionCode,
                            VersionName = model.VersionName,
                            Type = model.Type,
                            Desc = model.Desc,
                            ApkFilePath = apkFilePath
                        };
                        // 保存到数据库
                        mobileVersionBll.Save(newVersion);
                    }
                    else
                    {
                        jm.Msg = "不能上传空文件";
                    }
                    //日志记录
                    jm.Content = "新增移动端版本" + model.VersionName;
                }

            }
            else
            {
                // 保存异常日志
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }

            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除指定版本
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteVersion(int id)
        {
            JsonModel jm = new JsonModel();

            IMobileVersionBLL mobileVersionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            // 根据指定id值获取实体对象
            var versionInfo = mobileVersionBll.GetEntity(index => index.Id == id);
            if (versionInfo != null)
            {
                //修改平台用户对应的角色集合
                if (mobileVersionBll.DeleteVersion(id))
                {
                    //删除apk文件
                    string apkPath = versionInfo.ApkFilePath;
                    string fullDirectory = Server.MapPath(apkPath);
                    if (!string.IsNullOrEmpty(apkPath))
                    {
                        FileInfo f = new FileInfo(fullDirectory);
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                    }
                    jm.Content = "删除指定版本" + versionInfo.VersionName + "成功";
                }
            }
            else
            {
                jm.Msg = "该版本不存在";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 远程验证指定版本名称是否存在
        /// </summary>
        /// <param name="UserName">版本名称</param>
        /// <param name="UserId">版本id,新增时恒为0</param>
        public ContentResult RemoteCheckExist(int Id, int type, string versionName)
        {
            IMobileVersionBLL mobileVersionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            // 版本名称已存在
            if (mobileVersionBll.Exist(m => m.VersionName == versionName && m.Type == type && m.Id != Id))
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
        /// 下载Apk文件
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DownApkFile(string Path, int Id)
        {
            IMobileVersionBLL mobileVersionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
            T_MobileVersion mobileVersion = mobileVersionBll.GetEntity(u => u.Id == Id);

            //获取文件的相对路径
            var path = Server.MapPath(Path);

            //如果客户端信息版本不存在
            if (mobileVersion != null)
            {
                //获取文件名
                var item = mobileVersion.ApkFilePath.Substring(12);

                //如果不包含改路径
                if (!System.IO.File.Exists(path))
                {
                    return RedirectToAction("MobileVersion", "Error404");
                }

                return File(path, "application/octet-stream", item);
            }

            return RedirectToAction("MobileVersion", "VersionList");
        }
    }
}