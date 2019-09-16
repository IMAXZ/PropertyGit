using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 手机端物业客户端用户相关控制器
    /// </summary>
    public class PropertyMobileController : ApiController
    {
        /// <summary>
        /// 物业客户端登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel Login(OwnerLoginModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户名查找用户
                IPropertyUserBLL propertyUserBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = propertyUserBll.GetEntity(u => u.UserName == model.UserName
                    && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //1.判断用户名是否正确
                if (user == null)
                {
                    resultModel.Msg = APIMessage.NAME_ERROR;
                    return resultModel;
                }

                //2.判断密码是否正确
                string md5Str = PropertyUtils.GetMD5Str(model.Password);
                if (user.Password != md5Str)
                {
                    resultModel.Msg = APIMessage.PWD_ERROR;
                    return resultModel;
                }

                //产生随机令牌
                var token = System.Guid.NewGuid().ToString("N");
                //更新用户令牌和最近登录时间及Token失效时间
                user.Token = token;
                user.LatelyLoginTime = DateTime.Now;
                user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                propertyUserBll.Update(user);

                //返回登录用户的ID和用户名以及令牌
                resultModel.result = new { token = token, userId = user.Id, userName = user.UserName, isMgr = user.IsMgr };

                //推送设备管理
                IPropertyUserPushBLL userPushBll = BLLFactory<IPropertyUserPushBLL>.GetBLL("PropertyUserPushBLL");
                var userPush = userPushBll.GetEntity(p => p.UserId == user.Id);
                var userPush1 = userPushBll.GetEntity(p => p.RegistrationId == model.RegistrationId);
                if (userPush != null)
                {
                    userPush.RegistrationId = model.RegistrationId;
                    userPushBll.Update(userPush);
                }
                else if (userPush1 != null)
                {
                    userPush1.UserId = user.Id;
                    userPushBll.Update(userPush1);
                }
                else
                {
                    userPush = new T_PropertyUserPush()
                    {
                        UserId = user.Id,
                        RegistrationId = model.RegistrationId
                    };
                    userPushBll.Save(userPush);
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取物业用户信息
        /// </summary>
        /// <param name="model">Token模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetUserInfo([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    //初始化物业用户基本信息并返回
                    OwnerModel ownerM = new OwnerModel()
                    {
                        UserName = string.IsNullOrEmpty(user.TrueName) ? user.UserName : user.TrueName,
                        HeadPath = user.HeadPath,
                        Gender = user.Gender == null ? -1 : user.Gender.Value
                    };
                    resultModel.result = ownerM;
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }


        /// <summary>
        /// 设置物业用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetUserInfo(OwnerInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要设置基本信息的物业用户
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    //设定头像路径及文件名
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.PROPERTY_USER_HEAD_DIR);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //设置头像
                    if (!string.IsNullOrEmpty(model.HeadPic))
                    {
                        var fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
                        string filepath = Path.Combine(dir, fileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] datas = Convert.FromBase64String(model.HeadPic);
                                bw.Write(datas);
                                bw.Close();
                            }
                        }

                        //删除旧头像
                        if (!string.IsNullOrEmpty(user.HeadPath))
                        {
                            FileInfo f = new FileInfo(HttpContext.Current.Server.MapPath(user.HeadPath));
                            if (f.Exists)
                                f.Delete();
                        }

                        //头像路径保存
                        user.HeadPath = ConstantParam.PROPERTY_USER_HEAD_DIR + "/" + fileName;
                    }
                    user.Gender = model.Gender;
                    userBll.Update(user);
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model">密码修改模型</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel ChangePassword(OwnerChangePasswordModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要修改密码的物业用户
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    string OldMd5Pwd = PropertyUtils.GetMD5Str(model.OldPwd);
                    //如果输入的旧密码与数据库中不一致
                    if (OldMd5Pwd != user.Password)
                    {
                        resultModel.Msg = APIMessage.OLD_PWD_ERROR;
                    }
                    else
                    {
                        //修改密码并保存
                        user.Password = PropertyUtils.GetMD5Str(model.NewPwd);
                        userBll.Update(user);
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取最新版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetVersionInfo([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取当前用户
                IPropertyUserBLL userBll = BLLFactory<IPropertyUserBLL>.GetBLL("PropertyUserBLL");
                T_PropertyUser user = userBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (user != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > user.TokenInvalidTime || model.Token != user.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    user.LatelyLoginTime = DateTime.Now;
                    user.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    userBll.Update(user);

                    //调用版本信息BLL层获取最新的版本信息
                    IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
                    var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_PROPERTY, "VersionCode", false);
                    //如果版本信息不为空
                    if (Versions != null && Versions.Count() > 0)
                    {
                        var highestVersion = Versions.First();
                        if (highestVersion != null)
                        {
                            resultModel.result = new
                            {
                                VersionCode = highestVersion.VersionCode,
                                VersionName = highestVersion.VersionName,
                                Desc = highestVersion.Desc,
                                ApkFilePath = highestVersion.ApkFilePath
                            };
                        }
                    }
                    else 
                    {
                        resultModel.Msg = APIMessage.NO_APP;
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.NO_USER;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }
    }
}
