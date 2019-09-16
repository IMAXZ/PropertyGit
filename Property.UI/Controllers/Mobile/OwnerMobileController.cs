using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;

namespace Property.UI.Controllers.Mobile
{
    /// <summary>
    /// 手机端业主客户端用户管理控制器
    /// </summary>
    public class OwnerMobileController : ApiController
    {
        #region 登录
        /// <summary>
        /// 业主客户端登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel Login(OwnerLoginModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户名查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => (u.Email == model.UserName || u.Phone == model.UserName)
                    && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //1.判断用户名是否正确
                if (owner == null)
                {
                    resultModel.Msg = APIMessage.USERNAME_ERROR;
                    return resultModel;
                }

                //2.判断密码是否正确
                string md5Str = PropertyUtils.GetMD5Str(model.Password);
                if (owner.Password != md5Str)
                {
                    resultModel.Msg = APIMessage.PWD_ERROR;
                    return resultModel;
                }
                //产生随机令牌
                var token = System.Guid.NewGuid().ToString("N");
                //更新用户令牌和最近登录时间及Token失效时间
                owner.Token = token;
                owner.LatelyLoginTime = DateTime.Now;
                owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                ownerBll.Update(owner);

                //返回登录用户的ID和用户名以及令牌
                resultModel.result = new { token = token, userId = owner.Id, userName = owner.UserName };

                //推送设备管理
                IUserPushBLL userPushBll = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                var userPush = userPushBll.GetEntity(p => p.UserId == owner.Id);
                var userPush1 = userPushBll.GetEntity(p => p.RegistrationId == model.RegistrationId);
                if (userPush != null)
                {
                    userPush.RegistrationId = model.RegistrationId;
                    userPushBll.Update(userPush);
                }
                else if (userPush1 != null)
                {
                    userPush1.UserId = owner.Id;
                    userPushBll.Update(userPush1);
                }
                else
                {
                    userPush = new T_UserPush()
                    {
                        UserId = owner.Id,
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
        /// 业主客户端微信登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel WeixinLogin(WeixinLoginModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户名查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.WeixinUnionId == model.OpenId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                //1.判断用户名是否正确
                if (owner == null)
                {
                    resultModel.Msg = APIMessage.WEIXIN_NO_REGISTER;
                    return resultModel;
                }
                //产生随机令牌
                var token = System.Guid.NewGuid().ToString("N");
                //更新用户令牌和最近登录时间及Token失效时间
                owner.Token = token;
                owner.LatelyLoginTime = DateTime.Now;
                owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                ownerBll.Update(owner);

                //返回登录用户的ID和用户名以及令牌
                resultModel.result = new
                {
                    token = token,
                    userId = owner.Id,
                    userName = owner.UserName,
                    IsHavePassword = !string.IsNullOrEmpty(owner.Password)
                };

                //推送设备管理
                IUserPushBLL userPushBll = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                var userPush = userPushBll.GetEntity(p => p.UserId == owner.Id);
                var userPush1 = userPushBll.GetEntity(p => p.RegistrationId == model.RegistrationId);
                if (userPush != null)
                {
                    userPush.RegistrationId = model.RegistrationId;
                    userPushBll.Update(userPush);
                }
                else if (userPush1 != null)
                {
                    userPush1.UserId = owner.Id;
                    userPushBll.Update(userPush1);
                }
                else
                {
                    userPush = new T_UserPush()
                    {
                        UserId = owner.Id,
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
        #endregion

        #region 注册、身份验证
        /// <summary>
        /// 业主账户注册(邮箱)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel Register(OwnerRegisterModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                //如果邮箱已存在
                if (ownerBll.Exist(o => o.Email == model.Email && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    resultModel.Msg = APIMessage.EMAIL_EXIST;
                    return resultModel;
                }
                T_User owner = new T_User();

                //随机字符串
                string str = "1234567890abcdefghijklmnopqrstuvwxyz";
                Random r = new Random();
                string RandomStr = "";
                for (int i = 0; i < 16; i++)
                {
                    RandomStr += str[r.Next(str.Length)].ToString();
                }
                owner.UserName = RandomStr;
                owner.Email = model.Email;
                owner.Password = PropertyUtils.GetMD5Str(model.Password);
                owner.UserPlaces.Add(new R_UserPlace()
                {
                    PropertyPlaceId = model.PlaceId
                });
                ownerBll.Save(owner);
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 业主账户注册(手机号)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel PhoneRegister(OwnerPhoneRegisterModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                //如果手机号已存在
                if (ownerBll.Exist(o => o.Phone == model.Phone && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    resultModel.Msg = APIMessage.PHONE_EXIST;
                    return resultModel;
                }
                IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.Phone && v.ActionCode == 0);
                //判断验证码不准确
                if (val == null || model.validateCode != val.ValidateCode)
                {
                    resultModel.Msg = APIMessage.VALIDATE_ERROR;
                    return resultModel;
                }
                //验证码已失效
                if (val.InvalidTime < DateTime.Now)
                {
                    resultModel.Msg = APIMessage.VALIDATE_INVALID;
                    return resultModel;
                }

                T_User owner = new T_User();
                owner.Phone = model.Phone;
                //随机字符串
                string str = "1234567890abcdefghijklmnopqrstuvwxyz";
                Random r = new Random();
                string RandomStr = "";
                for (int i = 0; i < 16; i++)
                {
                    RandomStr += str[r.Next(str.Length)].ToString();
                }
                owner.UserName = RandomStr;
                owner.Password = PropertyUtils.GetMD5Str(model.Password);
                owner.UserPlaces.Add(new R_UserPlace()
                {
                    PropertyPlaceId = model.PlaceId
                });
                ownerBll.Save(owner);
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 微信第三方账户注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel WeixinRegister(OwnerWeixinRegisterModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                //如果该微信用户已存在
                if (ownerBll.Exist(o => o.WeixinUnionId == model.OpenId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                {
                    resultModel.Msg = "该微信用户已注册";
                    return resultModel;
                }
                T_User owner = new T_User();

                //随机字符串
                string str = "1234567890abcdefghijklmnopqrstuvwxyz";
                Random r = new Random();
                string RandomStr = "";
                for (int i = 0; i < 16; i++)
                {
                    RandomStr += str[r.Next(str.Length)].ToString();
                }
                owner.UserName = RandomStr;
                owner.WeixinUnionId = model.OpenId;
                owner.UserPlaces.Add(new R_UserPlace()
                {
                    PropertyPlaceId = model.PlaceId
                });
                ownerBll.Save(owner);
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 业主身份审批验证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel IdentityVerification(OwnerApprovalModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要进行身份验证的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //如果要验证的小区存在
                    IPropertyPlaceBLL propertyPlaceBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    var place = propertyPlaceBll.GetEntity(u => u.Id == model.PlaceId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                    if (place != null)
                    {
                        IPropertyIdentityVerificationBLL identityVerificationBll = BLLFactory<IPropertyIdentityVerificationBLL>.GetBLL("PropertyIdentityVerificationBLL");

                        //如果小区类型为住宅小区
                        if (place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
                        {
                            //判断验证码是否正确
                            IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                            var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.PhoneNum && v.ActionCode == 2);
                            //判断验证码不准确
                            if (val == null || model.CodeNum != val.ValidateCode)
                            {
                                resultModel.Msg = APIMessage.VALIDATE_ERROR;
                                return resultModel;
                            }
                            //验证码已失效
                            if (val.InvalidTime < DateTime.Now)
                            {
                                resultModel.Msg = APIMessage.VALIDATE_INVALID;
                                return resultModel;
                            }

                            var identityVerification = identityVerificationBll.GetEntity(i => i.AppUserId == model.UserId && i.BuildDoor.BuildUnit.Build.PropertyPlaceId == place.Id);
                            if (identityVerification != null)
                            {
                                //如果存在审核中或已通过的验证信息
                                if (identityVerification.IsVerified != 2)
                                {
                                    resultModel.Msg = APIMessage.VerifingOrYES;
                                    return resultModel;
                                }
                                else
                                {
                                    identityVerification.DoorId = model.DoorId;
                                    identityVerification.Name = model.UserName;
                                    identityVerification.Phone = model.PhoneNum;
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
                                    AppUserId = model.UserId,
                                    Name = model.UserName,
                                    Phone = model.PhoneNum,
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
                            var identityVerification = identityVerificationBll.GetEntity(i => i.AppUserId == model.UserId && i.BuildCompany.PropertyPlaceId == place.Id);
                            if (identityVerification != null)
                            {
                                //如果存在审核中或已通过的验证信息
                                if (identityVerification.IsVerified != 2)
                                {
                                    resultModel.Msg = APIMessage.VerifingOrYES;
                                    return resultModel;
                                }
                                else
                                {
                                    identityVerification.BuildCompanyId = model.DoorId;
                                    identityVerification.Phone = model.PhoneNum;
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
                                    AppUserId = model.UserId,
                                    Phone = model.PhoneNum,
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

                    }
                    else
                    {
                        resultModel.Msg = APIMessage.PLACE_NO_EXIST;
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
        #endregion

        #region 用户基本信息
        /// <summary>
        /// 获取业主信息
        /// </summary>
        /// <param name="model">用户Token模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetOwnerInfo([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //初始化业主基本信息并返回
                    OwnerModel ownerM = new OwnerModel()
                    {
                        UserName = owner.UserName,
                        HeadPath = owner.HeadPath,
                        Gender = owner.Gender == null ? -1 : owner.Gender.Value,
                        Email = owner.Email,
                        Phone = owner.Phone
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
        /// 设置业主基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetOwnerInfo(OwnerInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要设置基本信息的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //如果邮箱已被使用
                    if (ownerBll.Exist(o => o.Email == model.Email && o.Id != owner.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        resultModel.Msg = APIMessage.EMAIL_EXIST;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));

                    //设定头像路径及文件名
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.OWNER_HEAD_DIR);
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
                        if (!string.IsNullOrEmpty(owner.HeadPath))
                        {
                            FileInfo f = new FileInfo(HttpContext.Current.Server.MapPath(owner.HeadPath));
                            if (f.Exists)
                                f.Delete();
                        }

                        //头像路径保存
                        owner.HeadPath = ConstantParam.OWNER_HEAD_DIR + "/" + fileName;
                    }
                    owner.Gender = model.Gender;
                    owner.Email = model.Email;
                    owner.Phone = model.Phone;
                    ownerBll.Update(owner);
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
        /// 二期 设置业主基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetOwnerInfo2(OwnerInfoModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要设置基本信息的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);
                    //用户名为空
                    if (string.IsNullOrEmpty(model.UserName))
                    {
                        resultModel.Msg = APIMessage.NAME_NO_NULL;
                        return resultModel;
                    }
                    //如果邮箱已被使用
                    if (!string.IsNullOrEmpty(model.Email) && ownerBll.Exist(o => o.Email == model.Email && o.Id != owner.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        resultModel.Msg = APIMessage.EMAIL_EXIST;
                        return resultModel;
                    }
                    //如果手机号已被使用
                    if (!string.IsNullOrEmpty(model.Phone) && ownerBll.Exist(o => o.Phone == model.Phone && o.Id != owner.Id && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        resultModel.Msg = APIMessage.PHONE_EXIST;
                        return resultModel;
                    }

                    //手机号不为空，则需要验证码验证
                    if (!string.IsNullOrEmpty(model.Phone) && model.Phone != model.Phone)
                    {
                        IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                        var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.Phone && v.ActionCode == 0);
                        //判断验证码不准确
                        if (val == null || model.ValidateCode != val.ValidateCode)
                        {
                            resultModel.Msg = APIMessage.VALIDATE_ERROR;
                            return resultModel;
                        }
                        //验证码已失效
                        if (val.InvalidTime < DateTime.Now)
                        {
                            resultModel.Msg = APIMessage.VALIDATE_INVALID;
                            return resultModel;
                        }
                    }
                    //设定头像路径及文件名
                    string dir = HttpContext.Current.Server.MapPath(ConstantParam.OWNER_HEAD_DIR);
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
                        if (!string.IsNullOrEmpty(owner.HeadPath))
                        {
                            FileInfo f = new FileInfo(HttpContext.Current.Server.MapPath(owner.HeadPath));
                            if (f.Exists)
                                f.Delete();
                        }

                        //头像路径保存
                        owner.HeadPath = ConstantParam.OWNER_HEAD_DIR + "/" + fileName;
                    }
                    owner.UserName = model.UserName;
                    owner.Gender = model.Gender;
                    owner.Email = model.Email;
                    owner.Phone = model.Phone;
                    ownerBll.Update(owner);
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
        #endregion

        #region 密码

        /// <summary>
        /// 密码修改接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel ChangePassword(OwnerChangePasswordModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要修改密码的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //如果输入的旧密码与数据库中不一致
                    if (!string.IsNullOrEmpty(owner.Password))
                    {
                        if (string.IsNullOrEmpty(model.OldPwd))
                        {
                            resultModel.Msg = APIMessage.OLD_PWD_ERROR;
                        }
                        else
                        {
                            string OldMd5Pwd = PropertyUtils.GetMD5Str(model.OldPwd);
                            if (OldMd5Pwd != owner.Password)
                            {
                                resultModel.Msg = APIMessage.OLD_PWD_ERROR;
                            }
                            else
                            {
                                //修改密码并保存
                                owner.Password = PropertyUtils.GetMD5Str(model.NewPwd);
                                ownerBll.Update(owner);
                            }
                        }

                    }
                    else
                    {
                        //修改密码并保存
                        owner.Password = PropertyUtils.GetMD5Str(model.NewPwd);
                        ownerBll.Update(owner);
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
        /// 邮箱重置找回密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel ResetPassword(ResetPasswordModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要找回密码的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Email == model.Email && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //验证邮箱
                    string PassResetUrl = ConfigurationManager.AppSettings["PassResetUrl"].ToString();
                    PassResetUrl = PassResetUrl + "?Body={0}&activecode={1}";

                    //用户Id加密
                    string IdEncrypt = PropertyUtils.EncodeBase64(owner.Id.ToString());
                    //产生随机激活码
                    var activecode = PropertyUtils.EncodeBase64(System.Guid.NewGuid().ToString("N"));
                    PassResetUrl = string.Format(PassResetUrl, new string[] { IdEncrypt, activecode });

                    //保存或修改激活码和激活码失效时间
                    owner.Activecode = activecode;
                    //获取找回密码重置链接激活码有效时长（分钟数）
                    int ActiveCodeInvalid = Convert.ToInt32(PropertyUtils.GetConfigParamValue("ActiveCodeInvalid"));
                    owner.ActivecodeInvalidTime = DateTime.Now.AddMinutes(ActiveCodeInvalid);
                    ownerBll.Update(owner);

                    string EmailBody = "<P>您好：</p><br/>请点击：<a href=" + PassResetUrl + ">" + PassResetUrl + "</a>，进行重置密码";
                    //发生重置密码邮件
                    bool flag = PropertyUtils.SendEmail(model.Email, owner.UserName,
                        ConstantParam.PASS_RESET_EMAIL_TITLE, EmailBody);
                    if (!flag)
                    {
                        //邮件发送失败
                        resultModel.Msg = APIMessage.EMAIL_SEND_ERROR;
                        return resultModel;
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
        /// 手机号找回密码验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel PhoneRetrievePwdValidate(ValidateCodeModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要找回的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Phone == model.PhoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                    var val = phoneValidateBll.GetEntity(v => v.PhoneNum == model.PhoneNum && v.ActionCode == 1);
                    //判断验证码不准确
                    if (val == null || model.ValidateCode != val.ValidateCode)
                    {
                        resultModel.Msg = APIMessage.VALIDATE_ERROR;
                        return resultModel;
                    }
                    //验证码已失效
                    if (val.InvalidTime < DateTime.Now)
                    {
                        resultModel.Msg = APIMessage.VALIDATE_INVALID;
                        return resultModel;
                    }
                }
                else
                {
                    resultModel.Msg = APIMessage.PHONE_NO_EXIST;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 通过验证后重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel PhoneResetPassword(PhoneResetPasswordModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要修改密码的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Phone == model.PhoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //修改密码并保存
                    owner.Password = PropertyUtils.GetMD5Str(model.NewPwd);
                    ownerBll.Update(owner);
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
        #endregion

        #region 拥有小区管理
        /// <summary>
        /// 获取业主拥有小区
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetOwnPlace([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取拥有房屋的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    resultModel.result = owner.UserPlaces.Where(p => p.PropertyPlace.DelFlag == ConstantParam.DEL_FLAG_DEFAULT).Select(p => new
                    {
                        PlaceId = p.PropertyPlaceId,
                        PlaceName = p.PropertyPlace.Name,
                        PlaceImg = string.IsNullOrEmpty(p.PropertyPlace.ImgThumbnail) ? "/Images/news_item_default.png" : p.PropertyPlace.ImgThumbnail,
                        CompanyName = p.PropertyPlace.Company.Name,
                        PlaceType = p.PropertyPlace.PlaceType,
                        PlaceTel = p.PropertyPlace.Tel,
                        VerifyStatus = GetVerifyStatus(p.PropertyPlace, p.User),
                        VerifyInfo = GetVerifyInfo(p.PropertyPlace, p.User)
                    });
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

        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <param name="Place">要验证的小区</param>
        /// <param name="User">注册用户</param>
        private object GetVerifyInfo(T_PropertyPlace Place, T_User User)
        {
            //如果是住宅小区
            if (Place.PlaceType == ConstantParam.PLACE_TYPE_HOUSE)
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.DoorId != null && v.BuildDoor.BuildUnit.Build.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification != null)
                {
                    return new
                    {
                        DoorInfo = IdentityVerification.BuildDoor.BuildUnit.Build.BuildName + IdentityVerification.BuildDoor.BuildUnit.UnitName + IdentityVerification.BuildDoor.DoorName,
                        Name = IdentityVerification.Name,
                        Tel = IdentityVerification.Phone
                    };
                }
            }
            else
            {
                var IdentityVerification = User.PropertyIdentityVerification.Where(v => v.BuildCompanyId != null && v.BuildCompany.PropertyPlaceId == Place.Id).FirstOrDefault();
                if (IdentityVerification != null)
                {
                    return new
                    {
                        Name = IdentityVerification.BuildCompany.Name,
                        Tel = IdentityVerification.Phone
                    };
                }
            }
            return null;
        }


        /// <summary>
        /// 业主添加小区
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddOwnPlace(PlaceModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取拥有房屋的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    //如果该用户已经有要添加的小区
                    if (owner.UserPlaces.Any(up => up.UserId == model.UserId && up.PropertyPlaceId == model.PlaceId))
                    {
                        resultModel.Msg = APIMessage.PLACE_EXIST;
                    }
                    else
                    {
                        //添加拥有的小区
                        owner.UserPlaces.Add(new R_UserPlace()
                        {
                            PropertyPlaceId = model.PlaceId
                        });
                    }
                    ownerBll.Update(owner);
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
        /// 业主删除小区
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel DeleteOwnPlace([FromUri]PlaceModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取拥有房屋的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    var place = placeBll.GetEntity(p => p.Id == model.PlaceId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);

                    //如果该用户要删除的小区不存在
                    if (place == null || owner.UserPlaces.Count(up => up.PropertyPlaceId == place.Id) <= 0)
                    {
                        resultModel.Msg = "该用户未拥有该小区";
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
                        ownerBll.ExecuteSql("delete from R_UserPlace where UserId=" + model.UserId + " and PropertyPlaceId=" + model.PlaceId);
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

        #endregion

        #region 收货地址管理
        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetShippingAddress([FromUri]TokenModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取收货地址的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    resultModel.result = owner.AppUserShippingAddresss.Where(a => a.DelFlag == ConstantParam.DEL_FLAG_DEFAULT)
                        .OrderByDescending(a => a.Id).Select(a => new
                    {
                        AddressId = a.Id,
                        CountyId = a.CountyId,
                        AddressDetail = a.County.City.Province.ProvinceName + a.County.City.CityName + a.County.CountyName + "  " + a.AddressDetails,
                        ShipperName = a.ShipperName,
                        Gender = a.Gender,
                        Tel = a.Telephone,
                        IsDefault = a.IsDefault == 1
                    });
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
        /// 业主添加收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel AddShippingAddress(ShippingAddressModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取拥有房屋的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //添加收货地址
                    ICountyBLL countyBll = BLLFactory<ICountyBLL>.GetBLL("CountyBLL");
                    //如果存在
                    if (countyBll.Exist(c => c.Id == model.AreaId))
                    {
                        T_AppUserShippingAddress address = new T_AppUserShippingAddress()
                        {
                            ShipperName = model.ShipperName,
                            Gender = model.Gender,
                            Telephone = model.Tel,
                            AppUserId = owner.Id,
                            CountyId = model.AreaId,
                            AddressDetails = model.AddressDetails,
                        };
                        IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
                        addressBll.Save(address);
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.AREA_ERROR;
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
        /// 业主修改收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel EditShippingAddress(ShippingAddressModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要获取拥有房屋的小区的业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //编辑收货地址
                    ICountyBLL countyBll = BLLFactory<ICountyBLL>.GetBLL("CountyBLL");
                    if (countyBll.Exist(c => c.Id == model.AreaId))
                    {
                        IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
                        var address = addressBll.GetEntity(a => a.Id == model.AddressId && a.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                        if (address != null)
                        {
                            address.ShipperName = model.ShipperName;
                            address.Gender = model.Gender;
                            address.Telephone = model.Tel;
                            address.CountyId = model.AreaId;
                            address.AddressDetails = model.AddressDetails;
                            addressBll.Update(address);
                        }
                        else
                        {
                            resultModel.Msg = APIMessage.ADDRESS_NOEXIST;
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.AREA_ERROR;
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
        /// 业主删除收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel DeleteShippingAddress(ShippingAddressIDModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要删除收货地址的APP用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //删除收货地址
                    IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
                    var address = addressBll.GetEntity(a => a.Id == model.AddressId);
                    if (address != null)
                    {
                        address.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                        addressBll.Update(address);
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ADDRESS_NOEXIST;
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
        /// 设置为默认收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel SetShippingAddressToDefault(ShippingAddressIDModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //获取要设置默认收货地址的APP用户
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //设置默认收货地址
                    IAppUserShippingAddressBLL addressBll = BLLFactory<IAppUserShippingAddressBLL>.GetBLL("AppUserShippingAddressBLL");
                    var address = addressBll.GetEntity(a => a.Id == model.AddressId);
                    if (address != null)
                    {
                        //如果没设置成功
                        if (!addressBll.SetDefault(address))
                        {
                            resultModel.Msg = "默认地址设置失败";
                        }
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.ADDRESS_NOEXIST;
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
        #endregion

        #region 获取物业（商家）账户信息
        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="model">支付宝账户查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetAccountInfo([FromUri]AlipayAccountSearchModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);
                    if (model.Type == 0)
                    {
                        //获取物业支付宝账户信息
                        IPropertyAccountBLL propertyAccountBll = BLLFactory<IPropertyAccountBLL>.GetBLL("PropertyAccountBLL");
                        var alipayAccount = propertyAccountBll.GetEntity(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && a.PropertyPlaceId == model.Id
                            && a.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                        if (alipayAccount != null)
                        {
                            resultModel.result = new
                            {
                                AccountNumber = alipayAccount.Number,
                                MerchantNo = alipayAccount.MerchantNo,
                                Key = alipayAccount.AccountKey
                            };
                        }
                        else
                        {
                            resultModel.Msg = "物业未设置账户信息，不能在线缴费";
                        }
                    }
                    else if (model.Type == 1)
                    {
                        //获取商家支付宝账户信息
                        IShopAccountBLL shopAccountBll = BLLFactory<IShopAccountBLL>.GetBLL("ShopAccountBLL");
                        var alipayAccount = shopAccountBll.GetEntity(a => a.AccountType == ConstantParam.PROPERTY_ACCOUNT_Alipay && a.ShopId == model.Id
                            && a.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                        if (alipayAccount != null)
                        {
                            resultModel.result = new
                            {
                                AccountNumber = alipayAccount.Number,
                                MerchantNo = alipayAccount.MerchantNo,
                                Key = alipayAccount.AccountKey
                            };
                        }
                        else
                        {
                            resultModel.Msg = "所属商家未设置账户信息,不能在线支付";
                        }
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
        #endregion

        #region 版本信息管理
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
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    //调用版本信息BLL层获取最新的版本信息
                    IMobileVersionBLL versionBll = BLLFactory<IMobileVersionBLL>.GetBLL("MobileVersionBLL");
                    var Versions = versionBll.GetList(v => v.Type == ConstantParam.MOBILE_TYPE_OWNER, "VersionCode", false);
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

        #endregion

        #region 共通接口（获取省市区、获取小区列表、获取验证码、获取楼座单元户列表、办公楼单位列表、获取系统消息列表）

        /// <summary>
        /// 获取省市区数据
        /// </summary>
        /// <param name="model">用户Token模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetArea()
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //SQL 语句 获取出拥有小区的所有省市区组合
                //
                //

                IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                //获取出拥有小区的所有省市区组合
                var placeList = placeBll.GetList().ToList().Select(p => new
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.Province.ProvinceName,
                    CityId = p.CityId,
                    CityName = p.City.CityName,
                    CountyId = p.CountyId,
                    CountyName = p.County == null ? null : p.County.CountyName
                }).Distinct().ToList();

                //将省市区数据组装成树状模型
                var list = placeList.Select(p => new ProvinceModel
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.ProvinceName
                }).Distinct(new ProvinceComparer()).ToList();

                foreach (var Province in list)
                {
                    var CitysList = placeList.Where(pc => pc.ProvinceId == Province.ProvinceId).Select(pc => new CityModel()
                    {
                        CityId = pc.CityId,
                        CityName = pc.CityName
                    }).Distinct(new CityComparer()).ToList();

                    foreach (var City in CitysList)
                    {
                        var CountyList = placeList.Where(pco => pco.CityId == City.CityId && pco.CountyId != null).Select(pco => new CountyModel()
                        {
                            CountyId = pco.CountyId.Value,
                            CountyName = pco.CountyName
                        }).Distinct(new CountyComparer()).ToList();
                        City.Countys = CountyList;
                    }
                    Province.Citys = CitysList;
                }
                resultModel.result = list;
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取小区列表
        /// </summary>
        /// <param name="model">地区模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetPlace([FromUri]AreaModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                //初始化查询条件
                Expression<Func<T_PropertyPlace, bool>> where = p => p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT;
                if (model.ProvinceId != null)
                {
                    where = PredicateBuilder.And(where, p => p.ProvinceId == model.ProvinceId.Value);
                }
                if (model.CityId != null)
                {
                    where = PredicateBuilder.And(where, p => p.CityId == model.CityId.Value);
                }
                if (model.CountyId != null)
                {
                    where = PredicateBuilder.And(where, p => p.CountyId == model.CountyId.Value);
                }
                //获取小区列表并赋值
                IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                resultModel.result = placeBll.GetList(where).ToList().Select(p => new
                {
                    PlaceId = p.Id,
                    PlaceName = p.Name,
                    CompanyName = p.Company.Name
                });
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResultModel GetValidateCode(ValidateCodeGetModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                string code = PropertyUtils.CreateValidateCode(6);
                //如果是注册操作获取验证码
                if (model.ActionCode == 0)
                {
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    //如果手机号已存在
                    if (ownerBll.Exist(o => o.Phone == model.PhoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        resultModel.Msg = APIMessage.PHONE_EXIST;
                        return resultModel;
                    }
                }
                else if (model.ActionCode == 1)
                {
                    IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                    //如果手机号对应用户不存在
                    if (!ownerBll.Exist(o => o.Phone == model.PhoneNum && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT))
                    {
                        resultModel.Msg = APIMessage.PHONE_NO_EXIST;
                        return resultModel;
                    }
                }
                string msg = "【Ai我家】感谢使用Ai我家APP,验证码为:" + code + ",请在页面输入完成验证,如非本人操作请忽略";
                //如果短信发送成功
                if (PropertyUtils.SendSMS(model.PhoneNum, msg, null))
                {
                    IPhoneValidateBLL phoneValidateBll = BLLFactory<IPhoneValidateBLL>.GetBLL("PhoneValidateBLL");
                    var phoneValidate = phoneValidateBll.GetEntity(v => v.PhoneNum == model.PhoneNum && v.ActionCode == model.ActionCode);
                    //如果该手机号在相同操作中不存在
                    if (phoneValidate == null)
                    {
                        T_PhoneValidate v = new T_PhoneValidate()
                        {
                            PhoneNum = model.PhoneNum,
                            ValidateCode = code,
                            InvalidTime = DateTime.Now.AddMinutes(Convert.ToInt32(PropertyUtils.GetConfigParamValue("ValidateCodeInvalid"))),
                            ActionCode = model.ActionCode
                        };
                        phoneValidateBll.Save(v);
                    }
                    else
                    {
                        phoneValidate.ValidateCode = code;
                        phoneValidate.InvalidTime = DateTime.Now.AddMinutes(Convert.ToInt32(PropertyUtils.GetConfigParamValue("ValidateCodeInvalid")));
                        phoneValidateBll.Update(phoneValidate);
                    }
                    resultModel.result = new { Code = code };
                }
                else
                {
                    resultModel.Msg = APIMessage.VALDATE_GET_FAIL;
                }
            }
            catch
            {
                resultModel.Msg = APIMessage.REQUEST_EXCEPTION;
            }
            return resultModel;
        }

        /// <summary>
        /// 获取指定小区楼座单元户数据
        /// </summary>
        /// <param name="model">根据小区查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetBuildDoorList([FromUri]PlaceModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    var place = placeBll.GetEntity(p => p.Id == model.PlaceId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                    if (place != null)
                    {
                        //将楼座单元户数据组装成树状模型
                        var list = place.Builds.Select(b => new BuildModel
                        {
                            BuildId = b.Id,
                            BuildName = b.BuildName,
                            Units = b.BuildUnits.Select(bu => new UnitModel()
                            {
                                UnitId = bu.Id,
                                UnitName = bu.UnitName,
                                Doors = bu.BuildDoors.Select(d => new DoorModel()
                                {
                                    DoorId = d.Id,
                                    DoorName = d.DoorName
                                }).ToList()
                            }).ToList()
                        }).ToList();
                        resultModel.result = list;
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.PLACE_NO_EXIST;
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
        /// 获取指定办公楼小区单位数据
        /// </summary>
        /// <param name="model">根据小区查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiResultModel GetBuildCompanyList([FromUri]PlaceModel model)
        {
            ApiResultModel resultModel = new ApiResultModel();
            try
            {
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(o => o.Id == model.UserId && o.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    IPropertyPlaceBLL placeBll = BLLFactory<IPropertyPlaceBLL>.GetBLL("PropertyPlaceBLL");
                    var place = placeBll.GetEntity(p => p.Id == model.PlaceId && p.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                    if (place != null)
                    {
                        //将楼座单元户数据组装成树状模型
                        var list = place.BuildCompanys.Select(b => new BuildCompanyModel
                        {
                            CompanyId = b.Id,
                            CompanyName = b.Name,
                        }).ToList();
                        resultModel.result = list;
                    }
                    else
                    {
                        resultModel.Msg = APIMessage.PLACE_NO_EXIST;
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
        /// 获取系统消息列表
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public ApiPageResultModel GetSystemMessageList([FromUri]PagedSearchModel model)
        {
            ApiPageResultModel resultModel = new ApiPageResultModel();
            try
            {
                //根据用户ID查找业主
                IUserBLL ownerBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                T_User owner = ownerBll.GetEntity(u => u.Id == model.UserId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                //如果业主存在
                if (owner != null)
                {
                    //如果验证Token不通过或已过期
                    if (DateTime.Now > owner.TokenInvalidTime || model.Token != owner.Token)
                    {
                        resultModel.Msg = APIMessage.TOKEN_INVALID;
                        return resultModel;
                    }
                    //更新最近登录时间和Token失效时间
                    owner.LatelyLoginTime = DateTime.Now;
                    owner.TokenInvalidTime = DateTime.Now.AddDays(Convert.ToInt32(PropertyUtils.GetConfigParamValue("TokenInvalid")));
                    ownerBll.Update(owner);

                    // 获取消息列表数据
                    ISystemMessageBLL messageBLL = BLLFactory<ISystemMessageBLL>.GetBLL("SystemMessageBLL");
                    var list = messageBLL.GetPageList(m => true, "CreateTime", false, model.PageIndex).Select(m => new
                    {
                        MessageId = m.Id,
                        Title = m.Title,
                        Content = m.Content,
                        CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
                    resultModel.result = list;
                    resultModel.Total = messageBLL.Count(m => true);
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

        #endregion
    }

    #region 取消重复比对类
    /// <summary>
    /// 省份比对
    /// </summary>
    class ProvinceComparer : IEqualityComparer<ProvinceModel>
    {
        public bool Equals(ProvinceModel x, ProvinceModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.ProvinceId == y.ProvinceId;
        }

        public int GetHashCode(ProvinceModel model)
        {
            if (Object.ReferenceEquals(model, null)) return 0;

            int ProvinceId = model.ProvinceId.GetHashCode();
            int ProvinceName = model.ProvinceName.GetHashCode();
            return ProvinceId ^ ProvinceName;
        }
    }

    /// <summary>
    /// 城市比对
    /// </summary>
    class CityComparer : IEqualityComparer<CityModel>
    {
        public bool Equals(CityModel x, CityModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.CityId == y.CityId;
        }

        public int GetHashCode(CityModel model)
        {
            if (Object.ReferenceEquals(model, null)) return 0;

            int Id = model.CityId.GetHashCode();
            int Name = model.CityName.GetHashCode();
            return Id ^ Name;
        }
    }

    /// <summary>
    /// 县区比对
    /// </summary>
    class CountyComparer : IEqualityComparer<CountyModel>
    {
        public bool Equals(CountyModel x, CountyModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.CountyId == y.CountyId;
        }

        public int GetHashCode(CountyModel model)
        {
            if (Object.ReferenceEquals(model, null)) return 0;

            int Id = model.CountyId.GetHashCode();
            int Name = model.CountyName.GetHashCode();
            return Id ^ Name;
        }
    }
    #endregion
}
