using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chain_entities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modified_on = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chain_entities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "store_entities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chain_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    number = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    address_street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address_postal_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    address_city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone_number_country_code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    phone_number_number = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    store_owner_first_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    store_owner_last_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modified_on = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_entities", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_entities_chain_entities_chain_id",
                        column: x => x.chain_id,
                        principalTable: "chain_entities",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "chain_entities",
                columns: new[] { "id", "created_on", "modified_on", "name" },
                values: new object[,]
                {
                    { new Guid("5ec199f0-4c26-4d20-bbdd-39ac844237b8"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "OptiView" },
                    { new Guid("7eaf61d2-6d20-4ef7-9e2c-3e230456ae3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "ClearSight" },
                    { new Guid("d3271c28-5e1d-4011-9f79-aa573a9f2e3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocalPoint" },
                    { new Guid("e887f9ce-2096-4832-8587-f96d9c7d8bc7"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocusOptics" }
                });

            migrationBuilder.InsertData(
                table: "store_entities",
                columns: new[] { "id", "address_city", "address_postal_code", "address_street", "store_owner_first_name", "store_owner_last_name", "phone_number_country_code", "phone_number_number", "chain_id", "created_on", "email", "modified_on", "name", "number" },
                values: new object[,]
                {
                    { new Guid("1e8d0a09-b7b9-4a94-a293-cff36e767bea"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", null, new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@Independent1.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "Independent Downtown 1", 9 },
                    { new Guid("a2f309d9-e908-4ee1-9365-b72f7d088764"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", null, new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@Independent2.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "Independent Downtown 2", 10 },
                    { new Guid("e0aa46d0-3e3f-43e3-beb0-d71dfbc8500a"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", null, new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@Independent3.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "Independent Downtown 3", 11 },
                    { new Guid("ea911dd7-044d-45ff-b380-e04b5a8c1aa9"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", null, new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@Independent4.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "Independent Downtown 4", 12 },
                    { new Guid("2277a2ce-6623-4879-a3b5-709ba9fa625e"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("e887f9ce-2096-4832-8587-f96d9c7d8bc7"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@FocusOptics.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocusOptics Downtown 2", 6 },
                    { new Guid("23273be7-91ad-4095-a891-f10f0633a420"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("7eaf61d2-6d20-4ef7-9e2c-3e230456ae3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@ClearSight.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "ClearSight Downtown 2", 8 },
                    { new Guid("2ecacee5-f3c8-43e0-9ca6-379c49248afb"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("e887f9ce-2096-4832-8587-f96d9c7d8bc7"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@FocusOptics.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocusOptics Downtown 1", 5 },
                    { new Guid("4a15bcc2-9104-46e7-8a6c-b2030c1d573b"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("7eaf61d2-6d20-4ef7-9e2c-3e230456ae3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@ClearSight.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "ClearSight Downtown 1", 7 },
                    { new Guid("6c458b7e-c560-47c0-9e7c-5e66adf87192"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("5ec199f0-4c26-4d20-bbdd-39ac844237b8"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@OptiView.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "OptiView Downtown 2", 2 },
                    { new Guid("7d7e8194-b981-478a-baee-57982fa3f699"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("d3271c28-5e1d-4011-9f79-aa573a9f2e3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@FocalPoint.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocalPoint Downtown 2", 4 },
                    { new Guid("82d72abf-0045-4aaa-9d8b-78f28b437052"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("5ec199f0-4c26-4d20-bbdd-39ac844237b8"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@OptiView.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "OptiView Downtown 1", 1 },
                    { new Guid("d84a8b08-7d18-4c23-a8ac-efc11c585d74"), "Aalborg", "9000", "TestStreet 1", "John", "Doe", "+45", "12345678", new Guid("d3271c28-5e1d-4011-9f79-aa573a9f2e3c"), new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "TestMail@FocalPoint.dk", new DateTime(2026, 12, 1, 16, 22, 44, 0, DateTimeKind.Unspecified), "FocalPoint Downtown 1", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_chain_entities_created_on",
                table: "chain_entities",
                column: "created_on");

            migrationBuilder.CreateIndex(
                name: "ix_chain_entities_name",
                table: "chain_entities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_store_entities_chain_id",
                table: "store_entities",
                column: "chain_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_entities_name",
                table: "store_entities",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_store_entities_number",
                table: "store_entities",
                column: "number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "store_entities");

            migrationBuilder.DropTable(
                name: "chain_entities");
        }
    }
}
