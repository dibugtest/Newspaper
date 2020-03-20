namespace Newspaper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class officer_complement_added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblOfficer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Designation = c.String(),
                        OfficeAddress = c.String(),
                        Email = c.String(),
                        PISNo = c.String(),
                        Phone = c.String(),
                        Status = c.Boolean(nullable: false),
                        OfficerType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tblCustomer", "OfficerId", c => c.Int());
            CreateIndex("dbo.tblCustomer", "OfficerId");
            AddForeignKey("dbo.tblCustomer", "OfficerId", "dbo.tblOfficer", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblCustomer", "OfficerId", "dbo.tblOfficer");
            DropIndex("dbo.tblCustomer", new[] { "OfficerId" });
            DropColumn("dbo.tblCustomer", "OfficerId");
            DropTable("dbo.tblOfficer");
        }
    }
}
