using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommProject_1147.DataAccess.Migrations
{
    public partial class bbb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverType
                @id int
                AS
	            Select*from CoverTypes where Id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateCoverType
	            @id int,
	            @name varchar(50)
                AS
	            Update CoverTypes set name=@name where Id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCoverType
                @id int
                AS
	            delete from CoverTypes where Id=@id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
