using Property.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public class PropertyDbContext : DbContext
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PropertyDbContext()
            : base("PropertyConn")
        {
        }

        /// <summary>
        /// 模型创建预处理方法
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //为避免循环或多重级关系，下面这两种对应关系的表关闭级联删除
            modelBuilder.Entity<M_City>().HasRequired(a => a.Province).WithMany(u => u.Citys).WillCascadeOnDelete(false);
            modelBuilder.Entity<M_County>().HasRequired(a => a.City).WithMany(u => u.Counties).WillCascadeOnDelete(false);
            modelBuilder.Entity<M_Action>().HasRequired(a => a.Menu).WithMany(u => u.Actions).WillCascadeOnDelete(false);
            modelBuilder.Entity<M_ActionItem>().HasRequired(a => a.Action).WithMany(u => u.ActionItems).WillCascadeOnDelete(false);

            modelBuilder.Entity<T_InspectionCategory>().HasRequired(a => a.PropertyPlace).WithMany(u => u.InspectionCategorys).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionPlan>().HasRequired(a => a.PropertyPlace).WithMany(u => u.InspectionPlans).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionTimePlan>().HasRequired(a => a.InspectionPlan).WithMany(u => u.TimePlans).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_InspectionPlan>().HasRequired(a => a.PropertyPlace).WithMany(u => u.InspectionPlans).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionPlan>().HasRequired(a => a.PropertyPlace).WithMany(u => u.InspectionPlans).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionPoint>().HasRequired(a => a.InspectionCategory).WithMany(u => u.InspectionPoints).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionResult>().HasRequired(a => a.InspectionTimePlan).WithMany(u => u.InspectionResults).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionResult>().HasRequired(a => a.UploadUser).WithMany(u => u.InspectionResults).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Post>().HasRequired(a => a.PropertyPlace).WithMany(u => u.Posts).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Post>().HasRequired(a => a.SubmitUser).WithMany(u => u.Posts).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyPlace>().HasRequired(a => a.Province).WithMany(u => u.PropertyPlaces).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyPlace>().HasRequired(a => a.City).WithMany(u => u.PropertyPlaces).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyPlace>().HasRequired(a => a.Company).WithMany(u => u.PropertyPlaces).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyRole>().HasRequired(a => a.PropertyPlace).WithMany(u => u.PropertyRoles).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Question>().HasRequired(a => a.PropertyPlace).WithMany(u => u.Questions).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Question>().HasRequired(a => a.UploadUser).WithMany(u => u.Questions).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_QuestionDispose>().HasRequired(a => a.Question).WithMany(u => u.QuestionDisposes).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_QuestionDispose>().HasRequired(a => a.DisposeUser).WithMany(u => u.QuestionDisposes).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_InspectionExceptionDispose>().HasRequired(a => a.Result).WithMany(u => u.ExceptionDisposes).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_InspectionExceptionDispose>().HasRequired(a => a.DisposeUser).WithMany(u => u.ExceptionDisposes).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PlatformOpreateLog>().HasRequired(a => a.Opreater).WithMany(u => u.PlatformOpreateLogs).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyOpreateLog>().HasRequired(a => a.Opreater).WithMany(u => u.PropertyOpreateLogs).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_CompanyOpreateLog>().HasRequired(a => a.Opreater).WithMany(u => u.CompanyOpreateLogs).WillCascadeOnDelete(false);

            modelBuilder.Entity<T_PropertyUser>().HasRequired(a => a.PropertyPlace).WithMany(u => u.PropertyUsers).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_BuildUnit>().HasRequired(a => a.Build).WithMany(u => u.BuildUnits).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_BuildDoor>().HasRequired(a => a.BuildUnit).WithMany(u => u.BuildDoors).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_HouseUser>().HasRequired(a => a.BuildDoor).WithMany(u => u.HouseUsers).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_HouseUser>().HasRequired(a => a.PropertyPlace).WithMany(u => u.HouseUsers).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_BuildCompany>().HasRequired(a => a.PropertyPlace).WithMany(u => u.BuildCompanys).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Shop>().HasRequired(a => a.ShopUser).WithMany(u => u.Shops).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_GoodsCategory>().HasRequired(a => a.Shop).WithMany(u => u.GoodsCategorys).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_ShopSale>().HasRequired(a => a.GoodsCategory).WithMany(u => u.ShopSales).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_CompanyPost>().HasRequired(a => a.Company).WithMany(u => u.CompanyPosts).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_CompanyPost>().HasRequired(a => a.SubmitUser).WithMany(u => u.CompanyPosts).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_CompanyUser>().HasRequired(a => a.Company).WithMany(u => u.CompanyUsers).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_AppUserShippingAddress>().HasRequired(a => a.User).WithMany(u => u.AppUserShippingAddresss).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_AppUserShippingAddress>().HasRequired(a => a.County).WithMany(u => u.UserShippingAddressList).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyExpenseType>().HasRequired(a => a.PropertyPlace).WithMany(u => u.PropertyExpenseTypes).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_HouseUserExpenseTemplate>().HasRequired(a => a.PropertyExpenseType).WithMany(u => u.HouseUserExpenseTemplates).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_HouseUserExpenseDetails>().HasRequired(a => a.PropertyExpenseType).WithMany(u => u.HouseUserExpenseDetails).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Order>().HasRequired(a => a.Shop).WithMany(u => u.Orders).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Order>().HasRequired(a => a.User).WithMany(u => u.Orders).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_Order>().HasRequired(a => a.ShippingAddress).WithMany(u => u.Orders).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_OrderDetails>().HasRequired(a => a.Order).WithMany(u => u.OrderDetails).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_OrderDetails>().HasRequired(a => a.ShopSale).WithMany(u => u.OrderDetails).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PostBarTopic>().HasRequired(a => a.PostBarTopicType).WithMany(u => u.PostBarTopics).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PostBarTopic>().HasRequired(a => a.PostUser).WithMany(u => u.PostBarTopics).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PostBarTopicDiscuss>().HasRequired(a => a.PostBarTopic).WithMany(u => u.PostBarTopicDiscusss).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_PostBarTopicDiscuss>().HasRequired(a => a.PostUser).WithMany(u => u.PostBarTopicDiscussList).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PostBarTopicDiscuss>().HasRequired(a => a.ReplyUser).WithMany(u => u.ReplyPostBarTopicDiscussList).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyAccount>().HasRequired(a => a.PropertyPlace).WithMany(u => u.PropertyAccounts).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_PropertyExpenseNo>().HasRequired(a => a.PropertyExpenseType).WithMany(u => u.PropertyExpenseNos).WillCascadeOnDelete(false);
            
            modelBuilder.Entity<R_PlatformUserRole>().HasRequired(a => a.PlatformRole).WithMany(u => u.PlatformUserRoles).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_PlatformRoleAction>().HasRequired(a => a.Action).WithMany(u => u.PlatformRoleActions).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_PropertyUserRole>().HasRequired(a => a.PropertyRole).WithMany(u => u.PropertyUserRoles).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_PropertyRoleAction>().HasRequired(a => a.Action).WithMany(u => u.PropertyRoleActions).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_CompanyUserRole>().HasRequired(a => a.CompanyRole).WithMany(u => u.CompanyUserRoles).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_CompanyRoleAction>().HasRequired(a => a.Action).WithMany(u => u.CompanyRoleActions).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_UserPlace>().HasRequired(a => a.PropertyPlace).WithMany(u => u.UserPlaces).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserPlace>().HasRequired(a => a.User).WithMany(u => u.UserPlaces).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_ShopPlace>().HasRequired(a => a.Shop).WithMany(u => u.ShopPlaces).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_ShopPlace>().HasRequired(a => a.PropertyPlace).WithMany(u => u.ShopPlaces).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_PlanPoint>().HasRequired(a => a.InspectionPoint).WithMany(u => u.PlanPoints).WillCascadeOnDelete(false);

            modelBuilder.Entity<R_PropertyIdentityVerification>().HasRequired(a => a.User).WithMany(u => u.PropertyIdentityVerification).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserPostBarTopic>().HasRequired(a => a.AppUser).WithMany(u => u.UserPostBarTopics).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserPostBarTopic>().HasRequired(a => a.PostBarTopic).WithMany(u => u.UserPostBarTopics).WillCascadeOnDelete(true);

            modelBuilder.Entity<T_SocialCircle>().HasRequired(a => a.Creater).WithMany(u => u.SocialCircles).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_SocialCircle>().HasRequired(a => a.PropertyPlace).WithMany(u => u.SocialCircles).WillCascadeOnDelete(false);
            modelBuilder.Entity<R_UserSocialCircle>().HasRequired(a => a.ApplyUser).WithMany(u => u.UserSocialCircles).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserSocialCircle>().HasRequired(a => a.SocialCircle).WithMany(u => u.UserSocialCircles).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_SocialCircleChat>().HasRequired(a => a.ChatUser).WithMany(u => u.SocialCircleChats).WillCascadeOnDelete(true);
            modelBuilder.Entity<T_SocialCircleChat>().HasRequired(a => a.SocialCircle).WithMany(u => u.SocialCircleChats).WillCascadeOnDelete(false);
            modelBuilder.Entity<T_SocialCircleMassTexting>().HasRequired(a => a.SocialCircle).WithMany(u => u.SocialCircleMassTextings).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserSocialCircleMassTexting>().HasRequired(a => a.SocialCircleMassTexting).WithMany(u => u.UserSocialCircleMassTextings).WillCascadeOnDelete(true);
            modelBuilder.Entity<R_UserSocialCircleMassTexting>().HasRequired(a => a.User).WithMany(u => u.UserSocialCircleMassTextings).WillCascadeOnDelete(false);
            
            base.OnModelCreating(modelBuilder);
        }


        #region 数据库表实体对象
        public virtual DbSet<M_Action> M_Action { get; set; }
        public virtual DbSet<M_ActionItem> M_ActionItem { get; set; }
        public virtual DbSet<M_City> M_City { get; set; }
        public virtual DbSet<M_County> M_County { get; set; }
        public virtual DbSet<M_Menu> M_Menu { get; set; }
        public virtual DbSet<T_PlatformOpreateLog> T_PlatformOpreateLog { get; set; }
        public virtual DbSet<T_PropertyOpreateLog> T_PropertyOpreateLog { get; set; }
        public virtual DbSet<M_Province> M_Province { get; set; }
        public virtual DbSet<T_CompanyRole> T_CompanyRole { get; set; }
        public virtual DbSet<T_CompanyUser> T_CompanyUser { get; set; }
        public virtual DbSet<T_CompanyPost> T_CompanyPost { get; set; }
        public virtual DbSet<T_Company> T_Company { get; set; }
        public virtual DbSet<T_InspectionCategory> T_InspectionCategory { get; set; }
        public virtual DbSet<T_InspectionPlan> T_InspectionPlan { get; set; }
        public virtual DbSet<T_InspectionTimePlan> T_InspectionTimePlan { get; set; }
        public virtual DbSet<T_InspectionPoint> T_InspectionPoint { get; set; }
        public virtual DbSet<T_InspectionResult> T_InspectionResult { get; set; }
        public virtual DbSet<T_PlatformRole> T_PlatformRole { get; set; }
        public virtual DbSet<T_PlatformUser> T_PlatformUser { get; set; }
        public virtual DbSet<T_Post> T_Post { get; set; }
        public virtual DbSet<T_PropertyPlace> T_PropertyPlace { get; set; }
        public virtual DbSet<T_PropertyRole> T_PropertyRole { get; set; }
        public virtual DbSet<T_PropertyUser> T_PropertyUser { get; set; }
        public virtual DbSet<T_Question> T_Question { get; set; }
        public virtual DbSet<T_User> T_User { get; set; }
        public virtual DbSet<T_MobileVersion> T_MobileVersion { get; set; }
        public virtual DbSet<T_QuestionDispose> T_QuestionDispose { get; set; }
        public virtual DbSet<T_UserPush> T_UserPush { get; set; }
        public virtual DbSet<T_PropertyUserPush> T_PropertyUserPush { get; set; }
        public virtual DbSet<T_InspectionExceptionDispose> T_InspectionExceptionDispose { get; set; }
        public virtual DbSet<T_Build> T_Build { get; set; }
        public virtual DbSet<T_BuildUnit> T_BuildUnit { get; set; }
        public virtual DbSet<T_BuildDoor> T_BuildDoor { get; set; }
        public virtual DbSet<T_BuildCompany> T_BuildCompany { get; set; }
        public virtual DbSet<T_HouseUser> T_HouseUser { get; set; }
        public virtual DbSet<T_HouseUserExpenseDetails> T_HouseUserExpenseDetails { get; set; }
        public virtual DbSet<T_HouseUserExpenseTemplate> T_HouseUserExpenseTemplate { get; set; }
        public virtual DbSet<T_ShopUser> T_ShopUser { get; set; }
        public virtual DbSet<T_ShopUserPush> T_ShopUserPush { get; set; }
        public virtual DbSet<T_Shop> T_Shop { get; set; }
        public virtual DbSet<T_ShopSale> T_ShopSale { get; set; }
        public virtual DbSet<T_Order> T_Order { get; set; }
        public virtual DbSet<T_OrderDetails> T_OrderDetails { get; set; }
        public virtual DbSet<T_PostBarTopicType> T_PostBarTopicType { get; set; }
        public virtual DbSet<T_PostBarTopic> T_PostBarTopic { get; set; }
        public virtual DbSet<T_PostBarTopicDiscuss> T_PostBarTopicDiscuss { get; set; }
        public virtual DbSet<T_PropertyAccount> T_PropertyAccount { get; set; }
        public virtual DbSet<T_PropertyExpenseNo> T_PropertyExpenseNo { get; set; }
        public virtual DbSet<T_PropertyExpenseType> T_PropertyExpenseType { get; set; }
        public virtual DbSet<T_ShopAccounts> T_ShopAccounts { get; set; }
        public virtual DbSet<T_ShopPaymentManagement> T_ShopPaymentManagement { get; set; }
        public virtual DbSet<T_ShopShippingCost> T_ShopShippingCost { get; set; }
        public virtual DbSet<T_AppUserShippingAddress> T_AppUserShippingAddress { get; set; }
        public virtual DbSet<T_GoodsCategory> T_GoodsCategory { get; set; }
        public virtual DbSet<T_PhoneValidate> T_PhoneValidate { get; set; }
        public virtual DbSet<T_SystemMessage> T_SystemMessage { get; set; }
        public virtual DbSet<R_PlanPoint> R_PlanPoint { get; set; }
        public virtual DbSet<R_PlatformRoleAction> R_PlatformRoleAction { get; set; }
        public virtual DbSet<R_PlatformUserRole> R_PlatformUserRole { get; set; }
        public virtual DbSet<R_PropertyRoleAction> R_PropertyRoleAction { get; set; }
        public virtual DbSet<R_PropertyUserRole> R_PropertyUserRole { get; set; }
        public virtual DbSet<R_CompanyRoleAction> R_CompanyRoleAction { get; set; }
        public virtual DbSet<R_CompanyUserRole> R_CompanyUserRole { get; set; }
        public virtual DbSet<R_UserPlace> R_UserPlace { get; set; }
        public virtual DbSet<R_ShopPlace> R_ShopPlace { get; set; }
        public virtual DbSet<R_PropertyIdentityVerification> R_PropertyIdentityVerification { get; set; }
        public virtual DbSet<R_UserPostBarTopic> R_UserPostBarTopic { get; set; }
        public virtual DbSet<T_SocialCircle> T_SocialCircle { get; set; }
        public virtual DbSet<R_UserSocialCircle> R_UserSocialCircle { get; set; }
        public virtual DbSet<T_SocialCircleChat> T_SocialCircleChat { get; set; }
        public virtual DbSet<T_SocialCircleMassTexting> T_SocialCircleMassTexting { get; set; }
        public virtual DbSet<R_UserSocialCircleMassTexting> R_UserSocialCircleMassTexting { get; set; }
        public virtual DbSet<T_ApplyInfo> T_ApplyInfo { get; set; }
        public virtual DbSet<T_Feedback> T_Feedback { get; set; }
        public virtual DbSet<T_ExpressCompany> T_ExpressCompany { get; set; }
        public virtual DbSet<T_LifeBill> T_LifeBill { get; set; }
        public virtual DbSet<T_LifeBillType> T_LifeBillType { get; set; }
        public virtual DbSet<T_LifePayType> T_LifePayType { get; set; }

        #endregion
    }
}
