using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReaB_Bakery.Migrations
{
    /// <inheritdoc />
    public partial class Recreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomizationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspirationPhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakeFlavour = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherFlavour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CakeColour = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspirationPhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderReference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "ImageUrl", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Bento Collection", "A cute single-serving bento cake, beautifully decorated just for you. Perfect for birthdays, self-love treats, or a sweet surprise.", "/images/bento-cake.jpeg", true, "Bento Cake", 150m },
                    { 2, "Bento Collection", "Our signature bento cake paired with 2 matching decorated cupcakes. Great for sharing a little celebration.", "/images/bento-2cupcakes.jpeg", true, "Bento + 2 Cupcakes", 200m },
                    { 3, "Bento Collection", "A bento cake with 5 beautifully decorated cupcakes — ideal for small gatherings and intimate celebrations.", "/images/bento-5cupcakes.jpeg", true, "Bento + 5 Cupcakes", 280m },
                    { 4, "Bento Collection", "Our most generous bento bundle — one stunning bento cake with 8 matching cupcakes for the whole table.", "/images/bento-8cupcakes.jpeg", true, "Bento + 8 Cupcakes", 350m },
                    { 5, "Standard Cakes", "A petite 15 cm single-layer celebration cake, perfectly sized for 6–8 guests. Choose your flavour and we'll handle the rest.", "/images/cake-15cm.jpeg", true, "15 cm Cake", 320m },
                    { 6, "Standard Cakes", "Our most popular size — an 18 cm cake serving 10–12 guests beautifully. Customizable flavour and design.", "/images/cake-18cm.jpeg", true, "18 cm Cake", 420m },
                    { 7, "Standard Cakes", "A generous 20 cm cake that serves 14–16 guests. Perfect for birthdays, baby showers, and everything in between.", "/images/cake-20cm.jpeg", true, "20 cm Cake", 520m },
                    { 8, "Standard Cakes", "A stylish square-shaped cake with a modern finish. Available in various sizes — great for a unique, elegant look.", "/images/cake-square.jpeg", true, "Square Cake", 480m },
                    { 9, "Layer Cakes", "Two luscious layers of cake and filling, stacked high and decorated to impress. A step up for any special occasion.", "/images/cake-2layer.jpeg", true, "2 Layer Cake", 550m },
                    { 10, "Layer Cakes", "Three decadent layers of flavour and frosting. This showstopper cake is perfect for milestone celebrations.", "/images/cake-3layer.jpeg", true, "3 Layer Cake", 700m },
                    { 11, "Layer Cakes", "Four stunning layers of cake — tall, dramatic, and absolutely delicious. Built to make a lasting impression.", "/images/cake-4layer.jpeg", true, "4 Layer Cake", 850m },
                    { 12, "Wedding & Event Cakes", "An elegant two-tier cake crafted for weddings, engagements, and formal events. Designed and decorated to your vision.", "/images/cake-2tier.jpeg", true, "2 Tier Cake", 1200m },
                    { 13, "Wedding & Event Cakes", "A breathtaking three-tier masterpiece for your most important day. Every detail is handcrafted with love and precision.", "/images/cake-3tier.jpeg", true, "3 Tier Cake", 1800m },
                    { 14, "Wedding & Event Cakes", "The ultimate statement cake — four magnificent tiers for grand weddings and large events. Truly unforgettable.", "/images/cake-4tier.jpeg", true, "4 Tier Cake", 2400m },
                    { 15, "Wedding & Event Cakes", "Have something truly unique in mind? Request a custom quote and we'll bring your dream cake to life, whatever the vision.", "/images/cake-custom.jpeg", true, "Custom Quote", 0m },
                    { 16, "Cupcakes", "Half a dozen beautifully decorated cupcakes in your choice of flavour. Perfect for small treats and gifts.", "/images/cupcakes-6.jpeg", true, "6 Cupcakes", 120m },
                    { 17, "Cupcakes", "A full dozen decorated cupcakes — great for parties, offices, or anyone who believes more is always better.", "/images/cupcakes-12.jpeg", true, "12 Cupcakes", 220m },
                    { 18, "Cupcakes", "A bespoke box of cupcakes designed around your theme, colours, and occasion. Tell us your vision and we'll bake it.", "/images/cupcakes-custom.jpeg", true, "Custom Cupcake Box", 180m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomizationRequests");

            migrationBuilder.DropTable(
                name: "CustomOrders");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
