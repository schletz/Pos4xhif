using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAdministrator.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Period",
                columns: table => new
                {
                    P_Nr = table.Column<long>(nullable: false),
                    P_From = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    P_To = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Period", x => x.P_Nr);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    T_Lastname = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    T_Firstname = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    T_Email = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    T_Account = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.T_ID);
                });

            migrationBuilder.CreateTable(
                name: "Schoolclass",
                columns: table => new
                {
                    C_ID = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    C_Department = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    C_ClassTeacher = table.Column<string>(type: "VARCHAR(8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schoolclass", x => x.C_ID);
                    table.ForeignKey(
                        name: "FK_Schoolclass_Teacher_C_ClassTeacher",
                        column: x => x.C_ClassTeacher,
                        principalTable: "Teacher",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                columns: table => new
                {
                    L_ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    L_Untis_ID = table.Column<long>(nullable: true, defaultValueSql: "0")
                        .Annotation("Sqlite:Autoincrement", true),
                    L_Class = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    L_Teacher = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    L_Subject = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    L_Room = table.Column<string>(type: "VARCHAR(8)", nullable: true),
                    L_Day = table.Column<long>(nullable: true, defaultValueSql: "0")
                        .Annotation("Sqlite:Autoincrement", true),
                    L_Hour = table.Column<long>(nullable: true, defaultValueSql: "0")
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lesson", x => x.L_ID);
                    table.ForeignKey(
                        name: "FK_Lesson_Schoolclass_L_Class",
                        column: x => x.L_Class,
                        principalTable: "Schoolclass",
                        principalColumn: "C_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lesson_Period_L_Hour",
                        column: x => x.L_Hour,
                        principalTable: "Period",
                        principalColumn: "P_Nr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lesson_Teacher_L_Teacher",
                        column: x => x.L_Teacher,
                        principalTable: "Teacher",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pupil",
                columns: table => new
                {
                    P_ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    P_Account = table.Column<string>(type: "VARCHAR(16)", nullable: false),
                    P_Lastname = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    P_Firstname = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    P_Class = table.Column<string>(type: "VARCHAR(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pupil", x => x.P_ID);
                    table.ForeignKey(
                        name: "FK_Pupil_Schoolclass_P_Class",
                        column: x => x.P_Class,
                        principalTable: "Schoolclass",
                        principalColumn: "C_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    TE_ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TE_Class = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    TE_Teacher = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    TE_Subject = table.Column<string>(type: "VARCHAR(8)", nullable: false),
                    TE_Date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    TE_Lesson = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.TE_ID);
                    table.ForeignKey(
                        name: "FK_Test_Schoolclass_TE_Class",
                        column: x => x.TE_Class,
                        principalTable: "Schoolclass",
                        principalColumn: "C_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Test_Period_TE_Lesson",
                        column: x => x.TE_Lesson,
                        principalTable: "Period",
                        principalColumn: "P_Nr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Test_Teacher_TE_Teacher",
                        column: x => x.TE_Teacher,
                        principalTable: "Teacher",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "SchoolclassLesson",
                table: "Lesson",
                column: "L_Class");

            migrationBuilder.CreateIndex(
                name: "PeriodLesson",
                table: "Lesson",
                column: "L_Hour");

            migrationBuilder.CreateIndex(
                name: "TeacherLesson",
                table: "Lesson",
                column: "L_Teacher");

            migrationBuilder.CreateIndex(
                name: "idx_L_Untis_ID",
                table: "Lesson",
                column: "L_Untis_ID");

            migrationBuilder.CreateIndex(
                name: "idx_P_Account",
                table: "Pupil",
                column: "P_Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SchoolclassPupil",
                table: "Pupil",
                column: "P_Class");

            migrationBuilder.CreateIndex(
                name: "TeacherSchoolclass",
                table: "Schoolclass",
                column: "C_ClassTeacher");

            migrationBuilder.CreateIndex(
                name: "idx_T_Account",
                table: "Teacher",
                column: "T_Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SchoolclassTest",
                table: "Test",
                column: "TE_Class");

            migrationBuilder.CreateIndex(
                name: "PeriodTest",
                table: "Test",
                column: "TE_Lesson");

            migrationBuilder.CreateIndex(
                name: "TeacherTest",
                table: "Test",
                column: "TE_Teacher");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropTable(
                name: "Pupil");

            migrationBuilder.DropTable(
                name: "Test");

            migrationBuilder.DropTable(
                name: "Schoolclass");

            migrationBuilder.DropTable(
                name: "Period");

            migrationBuilder.DropTable(
                name: "Teacher");
        }
    }
}
