using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.UI.Models;
using Property.UI.Models.Weixin;
using System;
﻿using Property.BLL;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Webdiyer.WebControls.Mvc;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Property.UI.Controllers.Weixin
{
    /// <summary>
    /// 业主圈子模块控制器
    /// </summary>
    public class WeixinSocialCircleController : WeixinBaseController
    {
        public readonly static string TemplateId = "HZqYLh-rgn8zLx1uFg9tS8DYgPwEgOS3iLKHe0M7X2A";

        #region 业主圈子
        /// <summary>
        /// 业主圈子首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var placeIds = GetVerifiedPlaceIds();
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");

            //获取验证通过的小区圈子总数
            var PlaceIds = GetVerifiedPlaceIds();
            ViewBag.Count = socialCircleBLL.Count(s => PlaceIds.Contains(s.PlaceId));
            return View();
        }

        /// <summary>
        /// 业主圈子首页2
        /// </summary>
        /// <returns></returns>
        public ActionResult Index2()
        {
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            //获取验证通过的小区圈子总数
            var PlaceIds = GetVerifiedPlaceIds();
            ViewBag.Count = socialCircleBLL.Count(s => PlaceIds.Contains(s.PlaceId));
            return View();
        }

        /// <summary>
        /// 获取业主圈子分页数据
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSocialCircleJsonList(SearchModel model)
        {
            PageResultModel m = new PageResultModel();
            var ownerId = GetCurrentUser().Id;
            var PlaceIds = GetVerifiedPlaceIds();

            //查询条件
            Expression<Func<T_SocialCircle, bool>> where = s => PlaceIds.Contains(s.PlaceId) && s.CreaterId != ownerId
                && s.UserSocialCircles.Count(us => us.UserId == ownerId && us.ApplyStatus == ConstantParam.IsVerified_YES) == 0;
            if (!string.IsNullOrEmpty(model.Kword))
            {
                where = PredicateBuilder.And(where, s => s.Name.Contains(model.Kword));
            }
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            m.Total = socialCircleBLL.Count(where);
            m.Result = socialCircleBLL.GetPageList(where, "Id", false, model.PageIndex).Select(s => new SocialCircleItemModel()
            {
                Id = s.Id,
                Name = s.Name.Length > 10 ? s.Name.Substring(0, 10) + ".." : s.Name,
                PlaceName = s.PropertyPlace.Name.Length > 10 ? s.PropertyPlace.Name.Substring(0, 10) + ".." : s.PropertyPlace.Name,
                HeadPath = s.HeadImgPath,
                IsApplyed = s.UserSocialCircles.Count(us => us.UserId == ownerId && us.ApplyStatus == 0) > 0
            });
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据关键字搜索业主圈子
        /// </summary>
        /// <param name="kwords"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSearchMySocialCircle(string kwords)
        {
            var placeIds = GetVerifiedPlaceIds();
            MySocialCircleListModel model = new MySocialCircleListModel();
            var ownerId = GetCurrentUser().Id;
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");

            //我创建的圈子查询条件
            Expression<Func<T_SocialCircle, bool>> where1 = s => s.CreaterId == ownerId && placeIds.Contains(s.PlaceId);
            if (!string.IsNullOrEmpty(kwords))
            {
                where1 = PredicateBuilder.And(where1, s => s.Name.Contains(kwords));
            }
            //获取我创建的圈子列表
            model.CreateList = socialCircleBLL.GetList(where1).ToList().Select(s => new SocialCircleItemModel()
            {
                Id = s.Id,
                Name = s.Name.Length > 10 ? s.Name.Substring(0, 10) + ".." : s.Name,
                PlaceName = s.PropertyPlace.Name.Length > 10 ? s.PropertyPlace.Name.Substring(0, 10) + ".." : s.PropertyPlace.Name,
                HeadPath = s.HeadImgPath
            }).ToList();
            model.CreateCount = socialCircleBLL.Count(where1);

            //我加入的圈子查询条件
            Func<R_UserSocialCircle, bool> where2 = us => us.ApplyStatus == ConstantParam.IsVerified_YES && placeIds.Contains(us.SocialCircle.PlaceId)
                && (string.IsNullOrEmpty(kwords) ? true : us.SocialCircle.Name.Contains(kwords));

            //获取我加入的圈子列表
            model.JoinList = GetCurrentUser().UserSocialCircles.Where(where2).ToList().Select(us => new SocialCircleItemModel()
            {
                Id = us.SocialCircleId,
                Name = us.SocialCircle.Name.Length > 10 ? us.SocialCircle.Name.Substring(0, 10) + ".." : us.SocialCircle.Name,
                PlaceName = us.SocialCircle.PropertyPlace.Name.Length > 10 ? us.SocialCircle.PropertyPlace.Name.Substring(0, 10) + ".." : us.SocialCircle.PropertyPlace.Name,
                HeadPath = us.SocialCircle.HeadImgPath,
                NewestChatTime = ""
            }).Distinct(new SocialCircleComparer()).ToList();

            model.JoinCount = model.JoinList.Count;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建业主圈子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            WeixinApiInit();

            SocialCircleModel model = new SocialCircleModel();
            model.PlaceList = GetVerifiedPlaceList();
            return View(model);
        }


        /// <summary>
        /// 创建圈子提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create(SocialCircleModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                var returnImg = GetMultimedia(ConstantParam.SOCIAL_CIRCLE_HEAD_DIR, model.HeadImg);

                ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
                T_SocialCircle socialCircle = new T_SocialCircle()
                {
                    Name = model.Name,
                    Content = model.Content,
                    CreaterId = GetCurrentUser().Id,
                    CreateTime = DateTime.Now,
                    PlaceId = model.PlaceId,
                    HeadImgPath = returnImg
                };
                socialCircleBLL.Save(socialCircle);
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 申请加入圈子
        /// </summary>
        /// <param name="Id">圈子ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApplyJoin(int Id)
        {
            JsonModel jm = new JsonModel();
            var userId = GetCurrentUser().Id;
            try
            {
                IUserSocialCircleBLL userSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                //如果还没通过过申请
                if (!userSocialCircleBll.Exist(us => us.UserId == userId && us.SocialCircleId == Id && us.ApplyStatus == ConstantParam.IsVerified_YES))
                {
                    var userSocialCircle = userSocialCircleBll.GetEntity(us => us.UserId == userId && us.SocialCircleId == Id 
                        && us.ApplyStatus == ConstantParam.IsVerified_DEFAULT);
                    //如果已申请，正在等待审核
                    if (userSocialCircle != null)
                    {
                        userSocialCircle.ApplyTime = DateTime.Now;
                        userSocialCircleBll.Update(userSocialCircle);
                    }
                    else 
                    {
                        R_UserSocialCircle us = new R_UserSocialCircle()
                        {
                            UserId = GetCurrentUser().Id,
                            SocialCircleId = Id,
                            ApplyStatus = ConstantParam.IsVerified_DEFAULT,
                            ApplyTime = DateTime.Now
                        };
                        userSocialCircleBll.Save(us);
                    }
                }
                else
                {
                    jm.Msg = "您已加入该圈子啦";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 个人中心我的圈子、圈子详细

        /// <summary>
        /// 个人中心我的圈子页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyCircle()
        {
            MySocialCircleListModel model = new MySocialCircleListModel();

            var Owner = GetCurrentUser();
            var PlaceIds = GetVerifiedPlaceIds();
            //用户创建的圈子ID集合
            var sIds = Owner.SocialCircles.Select(s => s.Id).ToList();
            IUserSocialCircleBLL userSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
            var userSocialCircles = userSocialCircleBll.GetList(us => sIds.Contains(us.SocialCircleId) && PlaceIds.Contains(us.SocialCircle.PlaceId) && us.ApplyStatus == ConstantParam.IsVerified_DEFAULT);
            //获取未处理个数
            model.NoDealCount = userSocialCircles.Count();
            if (model.NoDealCount > 0)
            {
                var usc = userSocialCircles.OrderByDescending(us => us.ApplyTime).First();
                model.ApplyInfo = new ApplyInfoModel()
                {
                    ApplyUserName = usc.ApplyUser.UserName,
                    CircleName = usc.SocialCircle.Name,
                    ApplyTime = TimeFormat(usc.ApplyTime)
                };
            }

            //获取我接收到的最新群发消息
            IUserSocialCircleMassTextingBLL usMassTextingBll = BLLFactory<IUserSocialCircleMassTextingBLL>.GetBLL("UserSocialCircleMassTextingBLL");
            var usMassTextings = usMassTextingBll.GetList(m => m.UserId == Owner.Id && PlaceIds.Contains(m.SocialCircleMassTexting.SocialCircle.PlaceId));

            //如果有接收到的群发消息
            if (usMassTextings.Count() > 0)
            {
                var firstMass = usMassTextings.OrderByDescending(m => m.SocialCircleMassTexting.ChatTime).First();
                model.NewsetMass = new MassTextingModel()
                {
                    CircleName = firstMass.SocialCircleMassTexting.SocialCircle.Name,
                    MassSendTime = TimeFormat(firstMass.SocialCircleMassTexting.ChatTime)
                };
            }
            model.NoReadCount = usMassTextings.Count(um => um.IsNoRead);

            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            //获取我创建的圈子列表
            model.CreateList = socialCircleBLL.GetList(s => s.CreaterId == Owner.Id && PlaceIds.Contains(s.PlaceId)).ToList().Select(s => new SocialCircleItemModel()
            {
                Id = s.Id,
                Name = s.Name,
                PlaceName = s.PropertyPlace.Name,
                HeadPath = s.HeadImgPath,
                NewestChatTime = s.SocialCircleChats.Count > 0 ? TimeFormat(s.SocialCircleChats.OrderByDescending(c => c.ChatTime).First().ChatTime) : ""
            }).ToList();
            //获取我加入的圈子列表
            model.JoinList = GetCurrentUser().UserSocialCircles.Where(us => us.ApplyStatus == ConstantParam.IsVerified_YES && PlaceIds.Contains(us.SocialCircle.PlaceId)).ToList().Select(us => new SocialCircleItemModel()
            {
                Id = us.SocialCircleId,
                Name = us.SocialCircle.Name,
                PlaceName = us.SocialCircle.PropertyPlace.Name,
                HeadPath = us.SocialCircle.HeadImgPath,
                NewestChatTime = us.SocialCircle.SocialCircleChats.Count > 0 ? TimeFormat(us.SocialCircle.SocialCircleChats.OrderByDescending(c => c.ChatTime).First().ChatTime) : ""
            }).Distinct(new SocialCircleComparer()).ToList();
            return View(model);
        }

        /// <summary>
        /// 圈子详细页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Deatil(int id)
        {
            WeixinApiInit();

            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);

            ViewBag.CurrentUserId = GetCurrentUser().Id;
            return View(sc);
        }
        #endregion

        #region 圈子退出 解散 编辑

        /// <summary>
        /// 退出圈子
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Exit(int id)
        {
            int userId = GetCurrentUser().Id;
            JsonModel jm = new JsonModel();
            try
            {
                IUserSocialCircleBLL userSocialCircleBLL = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                if (userSocialCircleBLL.Exist(u => u.SocialCircleId == id && u.UserId == userId && u.ApplyStatus == ConstantParam.IsVerified_YES))
                {
                    int n = userSocialCircleBLL.ExecuteSql(string.Format("update R_UserSocialCircle set ApplyStatus = 3 where UserId={0} and SocialCircleId={1} and ApplyStatus=1", userId, id));
                    if (n < 0)
                    {
                        jm.Msg = "退出失败";
                    }
                }
                else
                {
                    jm.Msg = "您还未加入该圈子";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 解散圈子
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Dissolve(int id)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
                var sc = socialCircleBLL.GetEntity(s => s.Id == id);
                if (sc != null)
                {
                    if (sc.CreaterId == GetCurrentUser().Id)
                    {
                        if (!socialCircleBLL.Dissolve(sc))
                        {
                            jm.Msg = "解散失败";
                        }
                    }
                    else
                    {
                        jm.Msg = "只有创建人自己才能解散圈子";
                    }
                }
                else
                {
                    jm.Msg = "圈子已不存在";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改图片
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateImg(int id, string Img)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
                var sc = socialCircleBLL.GetEntity(s => s.Id == id);
                if (sc != null)
                {
                    if (!string.IsNullOrEmpty(Img))
                    {
                        string oldImgPath = sc.HeadImgPath;

                        //头像路径保存
                        sc.HeadImgPath = GetMultimedia(ConstantParam.SOCIAL_CIRCLE_HEAD_DIR, Img);
                        socialCircleBLL.Update(sc);

                        //删除旧头像
                        if (!string.IsNullOrEmpty(oldImgPath))
                        {
                            FileInfo f = new FileInfo(Server.MapPath(oldImgPath));
                            if (f.Exists)
                                f.Delete();
                        }
                    }
                }
                else
                {
                    jm.Msg = "圈子已不存在";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改圈子名称
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateName(int id)
        {
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);
            EditNameModel model = new EditNameModel()
            {
                Id = sc.Id,
                Name = sc.Name
            };
            return View(model);
        }


        /// <summary>
        /// 修改圈子名称提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateName(EditNameModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
                var sc = socialCircleBLL.GetEntity(s => s.Id == model.Id);
                if (sc != null)
                {
                    //修改名称
                    sc.Name = model.Name;
                    socialCircleBLL.Update(sc);
                }
                else
                {
                    jm.Msg = "圈子已不存在";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改圈子内容
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateContent(int id)
        {
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);
            EditContentModel model = new EditContentModel()
            {
                Id = sc.Id,
                Content = sc.Content
            };
            return View(model);
        }

        /// <summary>
        /// 修改圈子内容提交
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateContent(EditContentModel model)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
                var sc = socialCircleBLL.GetEntity(s => s.Id == model.Id);
                if (sc != null)
                {
                    //修改名称
                    sc.Content = model.Content;
                    socialCircleBLL.Update(sc);
                }
                else
                {
                    jm.Msg = "圈子已不存在";
                }
            }
            catch
            {
                jm.Msg = "请求发生异常";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 圈子聊天
        /// <summary>
        /// 圈子聊天页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Chat(int id)
        {
            WeixinApiInit();

            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);
            int MemberCount = sc.UserSocialCircles.Where(us => us.ApplyStatus == Property.Common.ConstantParam.IsVerified_YES).Select(us => us.UserId).Distinct().Count();
            SocialCircleChatModel model = new SocialCircleChatModel()
            {
                Id = sc.Id,
                Name = sc.Name,
                MemberCount = MemberCount + 1
            };
            ViewBag.TimeInterval = Convert.ToInt32(PropertyUtils.GetConfigParamValue("ChatTimeInterval"));
            return View(model);
        }


        /// <summary>
        /// 获取圈子聊天记录分页数据
        /// </summary>
        /// <param name="model">分页查询模型</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSocialCircleChatJsonList(int id, int pageIndex)
        {
            PageResultModel m = new PageResultModel();

            ISocialCircleChatBLL socialCircleChatBLL = BLLFactory<ISocialCircleChatBLL>.GetBLL("SocialCircleChatBLL");
            var list = socialCircleChatBLL.GetPageList(c => c.SocialCircleId == id, "ChatTime", false, pageIndex, 5).Select(c => new SocialCircleChatRecordModel
            {
                ChatUser = c.ChatUser.UserName,
                ChatUserHeadImg = string.IsNullOrEmpty(c.ChatUser.HeadPath) ? "/Images/Weixin/header_default.png" : c.ChatUser.HeadPath,
                ChatTime = c.ChatTime.ToString("yyyy/MM/dd HH:mm:ss"),
                ChatContent = c.Content,
                ChatImg = c.Img,
                IsMySelf = c.ChatUserId == GetCurrentUser().Id
            }).ToList();

            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i].LastChatTime = list[i + 1].ChatTime;
            }
            m.Result = list;
            m.Total = socialCircleChatBLL.Count(c => c.SocialCircleId == id);

            return Json(m, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送聊天信息
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <param name="content">圈子内容</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendChatMessage(int id, string content)
        {
            JsonModel jm = new JsonModel();
            try
            {
                ISocialCircleChatBLL socialCircleChatBLL = BLLFactory<ISocialCircleChatBLL>.GetBLL("SocialCircleChatBLL");
                T_SocialCircleChat chat = new T_SocialCircleChat()
                {
                    ChatUserId = GetCurrentUser().Id,
                    Content = content,
                    ChatTime = DateTime.Now,
                    SocialCircleId = id
                };
                socialCircleChatBLL.Save(chat);
            }
            catch
            {
                jm.Msg = "发送失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 发送聊天信息（图片）
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <param name="imgs">图片内容</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendChatImgMessage(int id, string imgs)
        {
            JsonModel jm = new JsonModel();
            try
            {
                var returnImgs = GetMultimedia(ConstantParam.SOCIAL_CIRCLE_CHAT_DIR, imgs);
                foreach (var img in returnImgs.Split(';'))
                {
                    ISocialCircleChatBLL socialCircleChatBLL = BLLFactory<ISocialCircleChatBLL>.GetBLL("SocialCircleChatBLL");
                    T_SocialCircleChat chat = new T_SocialCircleChat()
                    {
                        ChatUserId = GetCurrentUser().Id,
                        Img = img,
                        ChatTime = DateTime.Now,
                        SocialCircleId = id
                    };
                    socialCircleChatBLL.Save(chat);
                }
            }
            catch
            {
                jm.Msg = "发送失败";

            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 圈子成员管理

        ///<summary>
        ///我的圈子 成员管理
        ///</summary>
        ///<returns></returns>
        [HttpGet]
        public ActionResult CircleUserManage(int Id)
        {
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var src = socialCircleBLL.GetEntity(s => s.Id == Id);
            int coun = src.UserSocialCircles.Where(s => s.ApplyStatus == ConstantParam.IsVerified_YES).Count();
            circleuserModel model = new circleuserModel()
            {
                Count = coun + 1,
                Id = src.Id,
                UserId = GetCurrentUser().Id,
                CreateId = src.Creater.Id,
                CreateName = src.Creater.UserName,
                HeadImg = string.IsNullOrEmpty(src.Creater.HeadPath) ? "/Images/WeiXin/header_default.png" : src.Creater.HeadPath
            };
            return View(model);
        }

        /// <summary>
        /// 成员管理列表Json方式获取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CircleUserList(int id, int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            IUserSocialCircleBLL userSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
            model.Result = userSocialCircleBll.GetPageList(c => c.SocialCircleId == id && c.ApplyStatus == 1, pageIndex, ConstantParam.PAGE_SIZE).Select(c => new CircleUserModel
                {
                    Id = c.Id,
                    UserId = c.ApplyUser.Id,
                    userid = GetCurrentUser().Id,
                    CreateId = c.SocialCircle.CreaterId,
                    UserName = c.ApplyUser.UserName,
                    UserImg = string.IsNullOrEmpty(c.ApplyUser.HeadPath) ? "/Images/WeiXin/header_default.png" : c.ApplyUser.HeadPath,
                }).Distinct(new CircleUserComparer()).ToList();
            model.Total = userSocialCircleBll.Count(c => c.SocialCircleId == id && c.ApplyStatus == 1);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 成员管理 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleUserManage(int id, string userId)
        {
            JsonModel jm = new JsonModel();

            try
            {
                IUserSocialCircleBLL userSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                if (userId != null)
                {
                    string[] a = userId.Split(',');

                    int[] intTemp = new int[a.Length - 1];
                    for (int i = 0; i < a.Length - 1; i++)
                    {
                        intTemp[i] = int.Parse(a[i]);
                    }

                    for (var c = 0; c < intTemp.Length; c++)
                    {
                        int d = intTemp[c];
                        if (userSocialCircleBll.Exist(u => u.UserId == d && u.SocialCircleId == id && u.ApplyStatus == ConstantParam.IsVerified_YES))
                        {
                            userSocialCircleBll.ExecuteSql(string.Format("update R_UserSocialCircle set ApplyStatus=3 where UserId={0} and SocialCircleId={1} and ApplyStatus=1", d, id));
                        }
                    }
                }
            }
            catch
            {
                jm.Msg = "删除失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 群发

        /// <summary>
        /// 群发的消息界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendMsg(int id, string ids)
        {
            WeixinApiInit();

            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);

            string[] i = ids.Split(',');
            int[] d = new int[i.Length - 1];
            for (var s = 0; s < i.Length - 1; s++)
            {
                d[s] = int.Parse(i[s]);
            }
            string names = "";
            for (int c = 0; c < d.Length; c++)
            {
                int o = d[c];
                IUserSocialCircleBLL userSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                R_UserSocialCircle user = userSocialCircleBll.GetEntity(u => u.UserId == o);
                names += user.ApplyUser.UserName + "、";
            }
            names = names.Substring(0, names.Length - 1);
            UserListSocialCircleMassTextingModel model = new UserListSocialCircleMassTextingModel()
            {
                Id = sc.Id,
                CreaterName = sc.Creater.UserName,
                CreaterHeadPath = string.IsNullOrEmpty(sc.Creater.HeadPath) ? "/Images/Weixin/header_default.png" : sc.Creater.HeadPath,
                MemberIds = ids,
                NameList = names,
                Count = d.Length
            };
            return View(model);
        }


        /// <summary>
        /// 群发文本消息
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <param name="content">群发内容</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendMessage(int id, string memberIds, string content)
        {
            JsonResultModel jm = new JsonResultModel();
            try
            {
                ISocialCircleMassTextingBLL socialCircleMassTextingBll = BLLFactory<ISocialCircleMassTextingBLL>.GetBLL("SocialCircleMassTextingBLL");
                T_SocialCircleMassTexting socialCircleMassTexting = new T_SocialCircleMassTexting()
                {
                    SocialCircleId = id,//所属圈子id
                    ChatTime = DateTime.Now,
                    Content = content
                };

                foreach (var memberId in memberIds.Split(','))
                {
                    if (!string.IsNullOrEmpty(memberId))
                    {
                        socialCircleMassTexting.UserSocialCircleMassTextings.Add(new R_UserSocialCircleMassTexting()
                        {
                            UserId = Convert.ToInt32(memberId),
                            IsNoRead = true
                        });
                    }
                }
                socialCircleMassTextingBll.Save(socialCircleMassTexting);
                try
                {
                    if (!AccessTokenContainer.CheckRegistered(ConstantParam.AppId))
                    {
                        AccessTokenContainer.Register(ConstantParam.AppId, ConstantParam.AppSecret);
                    }
                    var accessToken = AccessTokenContainer.GetAccessToken(ConstantParam.AppId, true);
                    //发送模板消息
                    foreach (var memberId in memberIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(memberId))
                        {
                            int userId = Convert.ToInt32(memberId);
                            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                            var user = userBll.GetEntity(u => u.Id == userId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                            if (user != null && !string.IsNullOrEmpty(user.WeixinOpenId))
                            {
                                SendTemplateMessage(id, user.WeixinOpenId, accessToken);
                            }
                        }
                    }
                }
                catch
                {
                    jm.Msg = "消息提醒发生异常";
                }
                jm.Result = new
                {
                    Content = socialCircleMassTexting.Content,
                    ChatTime = socialCircleMassTexting.ChatTime.ToString("MM/dd HH:mm"),
                };
            }
            catch
            {
                jm.Msg = "发送失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///群发图片
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <param name="memberIds">群发成员ID集合</param>
        /// <param name="imgs">群发图片</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendImg(int id, string memberIds, string imgs)
        {
            JsonResultModel jm = new JsonResultModel();
            try
            {
                var returnImgs = GetMultimedia(ConstantParam.SOCIAL_CIRCLE_CHAT_DIR, imgs);
                foreach (var img in returnImgs.Split(';'))
                {
                    ISocialCircleMassTextingBLL socialCircleMassTextingBll = BLLFactory<ISocialCircleMassTextingBLL>.GetBLL("SocialCircleMassTextingBLL");
                    T_SocialCircleMassTexting socialCircleMassTexting = new T_SocialCircleMassTexting()
                    {
                        SocialCircleId = id,//所属圈子id
                        ChatTime = DateTime.Now,
                        Img = img
                    };
                    foreach (var memberId in memberIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(memberId))
                        {
                            socialCircleMassTexting.UserSocialCircleMassTextings.Add(new R_UserSocialCircleMassTexting()
                            {
                                UserId = Convert.ToInt32(memberId),
                                IsNoRead = true
                            });
                        }
                    }
                    socialCircleMassTextingBll.Save(socialCircleMassTexting);
                }

                try
                {
                    if (!AccessTokenContainer.CheckRegistered(ConstantParam.AppId))
                    {
                        AccessTokenContainer.Register(ConstantParam.AppId, ConstantParam.AppSecret);
                    }
                    var accessToken = AccessTokenContainer.GetAccessToken(ConstantParam.AppId, true);

                    //发送模板消息
                    foreach (var memberId in memberIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(memberId))
                        {
                            int userId = Convert.ToInt32(memberId);
                            IUserBLL userBll = BLLFactory<IUserBLL>.GetBLL("UserBLL");
                            var user = userBll.GetEntity(u => u.Id == userId && u.DelFlag == ConstantParam.DEL_FLAG_DEFAULT);
                            if (user != null && !string.IsNullOrEmpty(user.WeixinOpenId))
                            {
                                SendTemplateMessage(id, user.WeixinOpenId, accessToken);
                            }
                        }
                    }
                }
                catch
                {
                    jm.Msg = "消息提醒发生异常";
                }
                jm.Result = new
                {
                    MassImgPaths = returnImgs.Split(';'),
                    ChatTime = DateTime.Now.ToString("MM/dd HH:mm"),
                };
            }
            catch
            {
                jm.Msg = "发送失败";
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 群发消息列表
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MassTextingList(int id)
        {
            //获取指定圈子信息
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBLL.GetEntity(s => s.Id == id);
            SocialCircleChatModel model = new SocialCircleChatModel()
            {
                Id = sc.Id,
                Name = sc.Name
            };
            ViewBag.TimeInterval = Convert.ToInt32(PropertyUtils.GetConfigParamValue("ChatTimeInterval"));
            return View(model);
        }

        /// <summary>
        /// 获取圈子群发记录分页数据
        /// </summary>
        /// <param name="id">圈子ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSocialCircleMassTextingJsonList(int id, int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            //获取指定圈子的群发消息
            ISocialCircleMassTextingBLL massTextingBLL = BLLFactory<ISocialCircleMassTextingBLL>.GetBLL("SocialCircleMassTextingBLL");
            var list = massTextingBLL.GetPageList(m => m.SocialCircleId == id, "ChatTime", false, pageIndex, 5).ToList().Select(m => new SocialCircleMassTextingModel()
            {
                Id = m.Id,
                Content = m.Content,
                Img = m.Img,
                ChatTime = m.ChatTime.ToString("yyyy/MM/dd HH:mm:ss"),
                MemberIds = MemberIdsToStr(m.UserSocialCircleMassTextings.Select(um => um.UserId).ToArray()),
                MemberNames = MemberNamesToStr(m.UserSocialCircleMassTextings.Select(um => um.User.UserName).ToArray()),
                MemberCount = m.UserSocialCircleMassTextings.Count
            }).ToList();

            for (int i = 1; i < list.Count; i++)
            {
                list[i].LastTime = list[i - 1].ChatTime;
            }
            model.Result = list;
            model.Total = massTextingBLL.Count(m => m.SocialCircleId == id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 将群发成员ID集合转换为用，分割的字符串
        /// </summary>
        /// <param name="memberIds">群发成员数组</param>
        /// <returns></returns>
        private string MemberIdsToStr(int[] memberIds)
        {
            string memberIdsStr = "";
            foreach (var memberId in memberIds)
            {
                memberIdsStr += memberId + ",";
            }
            return memberIdsStr;
        }

        /// <summary>
        /// 将群发成员名称集合转换为用、分割的字符串
        /// </summary>
        /// <param name="memberNames">群发成员名称数组</param>
        /// <returns></returns>
        private string MemberNamesToStr(string[] memberNames)
        {
            string memberNamesStr = "";
            foreach (var memberName in memberNames)
            {
                memberNamesStr += memberName + "、";
            }
            return memberNamesStr.Substring(0, memberNamesStr.Length - 1);
        }

        /// <summary>
        /// 发送模板消息
        /// <param name="openId">关注公众号的用户openId</param>
        /// </summary>
        public void SendTemplateMessage(int circleId, string openId, string accessToken)
        {
            //获取新消息的所属圈子
            ISocialCircleBLL socialCircleBLL = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var socialCircle = socialCircleBLL.GetEntity(s => s.Id == circleId);

            //获取圈子群发消息未读条数
            int noReadCount = socialCircle.SocialCircleMassTextings.Count(m => m.UserSocialCircleMassTextings.Count(usm => usm.User != null && !string.IsNullOrEmpty(usm.User.WeixinOpenId) 
                && usm.User.WeixinOpenId == openId && usm.IsNoRead) > 0);
            var data = new
            {
                first = new TemplateDataItem(string.Format("您好，{0}发来一条新消息，请尽快查看", socialCircle.Name)),
                keyword1 = new TemplateDataItem(noReadCount.ToString()),
                keyword2 = new TemplateDataItem("【Ai我家】"),
                keyword3 = new TemplateDataItem(DateTime.Now.ToString()),
                remark = new TemplateDataItem("请点击查看消息详细内容！")
            };
            TemplateApi.SendTemplateMessage(accessToken, openId, TemplateId, "#000", "http://v.homekeeper.com.cn/WeixinSocialCircle/ReceiveMassTexting", data);
        }
        #endregion

        #region 收到的群发消息

        /// <summary>
        /// 接收到的群发消息页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReceiveMassTexting()
        {
            var Owner = GetCurrentUser();
            var PlaceIds = GetVerifiedPlaceIds();

            //获取有当前用户群发消息的圈子
            ISocialCircleBLL socialCircleBll = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var list = socialCircleBll.GetList(s => PlaceIds.Contains(s.PlaceId) && s.SocialCircleMassTextings.Count(m => m.UserSocialCircleMassTextings.Count(
                usm => usm.UserId == Owner.Id) > 0) > 0).ToList().Select(s => new ReceiveMassTextSocialCircleItemModel()
            {
                Id = s.Id,
                Name = s.Name.Length > 10 ? s.Name.Substring(0, 10) + ".." : s.Name,
                PlaceName = s.PropertyPlace.Name.Length > 10 ? s.PropertyPlace.Name.Substring(0, 10) + ".." : s.PropertyPlace.Name,
                HeadPath = s.HeadImgPath,
                TotalMassCount = s.SocialCircleMassTextings.Count(m => m.UserSocialCircleMassTextings.Count(usm => usm.UserId == Owner.Id) > 0),
                NoReadMassCount = s.SocialCircleMassTextings.Count(m => m.UserSocialCircleMassTextings.Count(usm => usm.UserId == Owner.Id && usm.IsNoRead) > 0),
                NewestChatTime = TimeFormat(s.SocialCircleMassTextings.Last(m => m.UserSocialCircleMassTextings.Count(usm => usm.UserId == Owner.Id) > 0).ChatTime)
            }).ToList();

            return View(list);
        }

        /// <summary>
        /// 接收到的群发消息详细（某个圈子）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReceiveMassTextingDetail(int id)
        {
            WeixinApiInit();
            var Owner = GetCurrentUser();

            //将该圈子发给当前用户的群发消息改为已读
            IUserSocialCircleMassTextingBLL usMassTextingBll = BLLFactory<IUserSocialCircleMassTextingBLL>.GetBLL("UserSocialCircleMassTextingBLL");
            var usMassTextings = usMassTextingBll.GetList(um => um.UserId == Owner.Id && um.SocialCircleMassTexting.SocialCircleId == id && um.IsNoRead).ToList();
            foreach (var texting in usMassTextings)
            {
                texting.IsNoRead = false;
                usMassTextingBll.Update(texting);
            }
            //获取指定圈子的信息
            ISocialCircleBLL socialCircleBll = BLLFactory<ISocialCircleBLL>.GetBLL("SocialCircleBLL");
            var sc = socialCircleBll.GetEntity(s => s.Id == id);

            SocialCircleChatModel model = new SocialCircleChatModel()
            {
                Id = sc.Id,
                Name = sc.Name
            };
            ViewBag.TimeInterval = Convert.ToInt32(PropertyUtils.GetConfigParamValue("ChatTimeInterval"));
            return View(model);
        }

        /// <summary>
        /// 获取群发记录分页数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetReceiveMassTextingJsonList(int id, int pageIndex)
        {
            PageResultModel model = new PageResultModel();

            var Owner = GetCurrentUser();
            var PlaceIds = GetVerifiedPlaceIds();

            Expression<Func<T_SocialCircleMassTexting, bool>> where = m => m.SocialCircleId == id && PlaceIds.Contains(m.SocialCircle.PlaceId)
                && m.UserSocialCircleMassTextings.Count(um => um.UserId == Owner.Id) > 0;

            //获取指定圈子的发送给当前用户的所有群发消息
            ISocialCircleMassTextingBLL socialCircleMassTextingBll = BLLFactory<ISocialCircleMassTextingBLL>.GetBLL("SocialCircleMassTextingBLL");
            var list = socialCircleMassTextingBll.GetPageList(where, "ChatTime", false, pageIndex, 5).Select(c => new SocialCircleMassTextingModel
            {
                Name = c.SocialCircle.Creater.UserName + ":",
                HeadPath = string.IsNullOrEmpty(c.SocialCircle.Creater.HeadPath) ? "/Images/Weixin/header_default.png" : c.SocialCircle.Creater.HeadPath,
                Content = c.Content,
                Img = c.Img,
                ChatTime = c.ChatTime.ToString("yyyy/MM/dd HH:mm:ss"),
            }).ToList();

            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i].LastTime = list[i + 1].ChatTime;
            }
            model.Result = list;
            model.Total = socialCircleMassTextingBll.Count(where);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 圈子验证
        /// <summary>
        /// 我的圈子验证消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValidateMessage()
        {
            var Owner = GetCurrentUser();
            var PlaceIds = GetVerifiedPlaceIds();
            //用户创建的圈子ID集合
            var sIds = Owner.SocialCircles.Select(s => s.Id).ToList();

            IUserSocialCircleBLL UserSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
            MyListSocialCircleModel model = new MyListSocialCircleModel();
            model.MyListSocialCircle = UserSocialCircleBll.GetList(s => sIds.Contains(s.SocialCircleId) && PlaceIds.Contains(s.SocialCircle.PlaceId)).ToList().Select(s => new MySocialCircleModel()
            {
                Id = s.Id,
                Name = s.SocialCircle.Name,
                UserName = s.ApplyUser.UserName,
                ApplyStatus = s.ApplyStatus,
                ApplyTime = s.ApplyTime,
                HeadPath = string.IsNullOrEmpty(s.ApplyUser.HeadPath) ? "/Images/news_item_default.png" : s.ApplyUser.HeadPath
            }).OrderByDescending(m => m.ApplyTime).ToList();
            return View(model);
        }

        /// <summary>
        /// 通过申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PassMgr(int id)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                IUserSocialCircleBLL UserSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                var UserSocialCircle = UserSocialCircleBll.GetEntity(m => m.Id == id);//获取要申请的圈子id
                if (UserSocialCircle != null)
                {
                    UserSocialCircle.ApplyStatus = 1;
                    UserSocialCircleBll.Update(UserSocialCircle);
                }
                else
                {
                    jm.Msg = "用户不存在";
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectMgr(int id)
        {
            JsonModel jm = new JsonModel();
            if (ModelState.IsValid)
            {
                IUserSocialCircleBLL UserSocialCircleBll = BLLFactory<IUserSocialCircleBLL>.GetBLL("UserSocialCircleBLL");
                var UserSocialCircle = UserSocialCircleBll.GetEntity(m => m.Id == id);
                if (UserSocialCircle != null)
                {
                    UserSocialCircle.ApplyStatus = 2;
                    UserSocialCircleBll.Update(UserSocialCircle);
                }
                else
                {
                    jm.Msg = "该用户不存在";
                }
            }
            else
            {
                jm.Msg = ConstantParam.JSON_RESULT_MODEL_CHECK_ERROR;
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

    /// <summary>
    /// 圈子比对
    /// </summary>
    class SocialCircleComparer : IEqualityComparer<SocialCircleItemModel>
    {
        public bool Equals(SocialCircleItemModel x, SocialCircleItemModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(SocialCircleItemModel model)
        {
            if (Object.ReferenceEquals(model, null)) return 0;
            return model.Id.GetHashCode() ^ model.Name.GetHashCode() ^ model.PlaceName.GetHashCode() ^ model.HeadPath.GetHashCode() ^ model.NewestChatTime.GetHashCode() ^ model.IsApplyed.GetHashCode();
        }
    }

    /// <summary>
    /// 圈子成员比对
    /// </summary>
    class CircleUserComparer : IEqualityComparer<CircleUserModel>
    {
        public bool Equals(CircleUserModel x, CircleUserModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.UserId == y.UserId;
        }

        public int GetHashCode(CircleUserModel model)
        {
            if (Object.ReferenceEquals(model, null)) return 0;
            return model.UserId.GetHashCode() ^ model.UserName.GetHashCode() ^ model.UserImg.GetHashCode() ^ model.CreateId.GetHashCode();
        }
    }
}
