namespace Newspaper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createdDate_to_officer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblOfficer", "CreatedBy", c => c.String());
            AddColumn("dbo.tblOfficer", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.tblOfficer", "UpdatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblOfficer", "UpdatedDate");
            DropColumn("dbo.tblOfficer", "CreatedDate");
            DropColumn("dbo.tblOfficer", "CreatedBy");
        }
    }
}
