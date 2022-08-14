using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class MakeCorrectAnswerNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Answers_CorrectAnswerID",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "CorrectAnswerID",
                table: "Question",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Answers_CorrectAnswerID",
                table: "Question",
                column: "CorrectAnswerID",
                principalTable: "Answers",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Answers_CorrectAnswerID",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "CorrectAnswerID",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Answers_CorrectAnswerID",
                table: "Question",
                column: "CorrectAnswerID",
                principalTable: "Answers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
