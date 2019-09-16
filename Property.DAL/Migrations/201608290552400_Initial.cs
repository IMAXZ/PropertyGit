namespace Property.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.T_LifeBilling", "BillingTypeId", "dbo.T_LifeBillingType");
            DropForeignKey("dbo.T_LifeBilling", "UserId", "dbo.T_User");
            DropIndex("dbo.T_LifeBilling", new[] { "UserId" });
            DropIndex("dbo.T_LifeBilling", new[] { "BillingTypeId" });
            CreateTable(
                "dbo.T_Feedback",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 500),
                        Img = c.String(maxLength: 200),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.T_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.T_LifeBill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Money = c.Double(nullable: false),
                        ConsumptionDate = c.DateTime(nullable: false),
                        Demo = c.String(maxLength: 500),
                        DelFlag = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        BillTypeId = c.Int(nullable: false),
                        PayTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.T_LifeBillType", t => t.BillTypeId, cascadeDelete: true)
                .ForeignKey("dbo.T_LifePayType", t => t.PayTypeId, cascadeDelete: true)
                .ForeignKey("dbo.T_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BillTypeId)
                .Index(t => t.PayTypeId);
            
            CreateTable(
                "dbo.T_LifeBillType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Img = c.String(nullable: false, maxLength: 200),
                        DelFlag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.T_LifePayType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Img = c.String(nullable: false, maxLength: 200),
                        DelFlag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.T_LifeBilling");
            DropTable("dbo.T_LifeBillingType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.T_LifeBillingType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Img = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.T_LifeBilling",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Money = c.Double(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Content = c.String(maxLength: 200),
                        Demo = c.String(maxLength: 500),
                        UserId = c.Int(nullable: false),
                        BillingTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.T_LifeBill", "UserId", "dbo.T_User");
            DropForeignKey("dbo.T_LifeBill", "PayTypeId", "dbo.T_LifePayType");
            DropForeignKey("dbo.T_LifeBill", "BillTypeId", "dbo.T_LifeBillType");
            DropForeignKey("dbo.T_Feedback", "UserId", "dbo.T_User");
            DropIndex("dbo.T_LifeBill", new[] { "PayTypeId" });
            DropIndex("dbo.T_LifeBill", new[] { "BillTypeId" });
            DropIndex("dbo.T_LifeBill", new[] { "UserId" });
            DropIndex("dbo.T_Feedback", new[] { "UserId" });
            DropTable("dbo.T_LifePayType");
            DropTable("dbo.T_LifeBillType");
            DropTable("dbo.T_LifeBill");
            DropTable("dbo.T_Feedback");
            CreateIndex("dbo.T_LifeBilling", "BillingTypeId");
            CreateIndex("dbo.T_LifeBilling", "UserId");
            AddForeignKey("dbo.T_LifeBilling", "UserId", "dbo.T_User", "Id", cascadeDelete: true);
            AddForeignKey("dbo.T_LifeBilling", "BillingTypeId", "dbo.T_LifeBillingType", "Id", cascadeDelete: true);
        }
    }
}
