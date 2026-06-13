using ReaB_Bakery.Models;
using Microsoft.EntityFrameworkCore;

namespace ReaB_Bakery.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CustomizationRequest> CustomizationRequests { get; set; }
        public DbSet<CustomOrder> CustomOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Order-OrderItem relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CustomOrder>()
    .Property(o => o.ProductPrice)
    .HasColumnType("decimal(18,2)");


            // Seed sample data
            modelBuilder.Entity<Product>().HasData(

                 // ── Bento Collection ──────────────────────────────
                 new Product
                 {
                     Id = 1,
                     Name = "Bento Cake",
                     Description = "A cute single-serving bento cake, beautifully decorated just for you. Perfect for birthdays, self-love treats, or a sweet surprise.",
                     Price = 150m,
                     ImageUrl = "/images/cake1.jpeg",
                     Category = "Bento Collection",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 2,
                     Name = "Bento + 2 Cupcakes",
                     Description = "Our signature bento cake paired with 2 matching decorated cupcakes. Great for sharing a little celebration.",
                     Price = 200m,
                     ImageUrl = "/images/cake2.jpeg",
                     Category = "Bento Collection",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 3,
                     Name = "Bento + 5 Cupcakes",
                     Description = "A bento cake with 5 beautifully decorated cupcakes — ideal for small gatherings and intimate celebrations.",
                     Price = 280m,
                     ImageUrl = "/images/bento-5cupcakes.jpeg",
                     Category = "Bento Collection",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 4,
                     Name = "Bento + 8 Cupcakes",
                     Description = "Our most generous bento bundle — one stunning bento cake with 8 matching cupcakes for the whole table.",
                     Price = 350m,
                     ImageUrl = "/images/bento-8cupcakes.jpeg",
                     Category = "Bento Collection",
                     IsAvailable = true
                 },

                 // ── Standard Cakes ────────────────────────────────
                 new Product
                 {
                     Id = 5,
                     Name = "15 cm Cake",
                     Description = "A petite 15 cm single-layer celebration cake, perfectly sized for 6–8 guests. Choose your flavour and we'll handle the rest.",
                     Price = 320m,
                     ImageUrl = "/images/cake-15cm.jpeg",
                     Category = "Standard Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 6,
                     Name = "18 cm Cake",
                     Description = "Our most popular size — an 18 cm cake serving 10–12 guests beautifully. Customizable flavour and design.",
                     Price = 420m,
                     ImageUrl = "/images/cake-18cm.jpeg",
                     Category = "Standard Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 7,
                     Name = "20 cm Cake",
                     Description = "A generous 20 cm cake that serves 14–16 guests. Perfect for birthdays, baby showers, and everything in between.",
                     Price = 520m,
                     ImageUrl = "/images/cake-20cm.jpeg",
                     Category = "Standard Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 8,
                     Name = "Square Cake",
                     Description = "A stylish square-shaped cake with a modern finish. Available in various sizes — great for a unique, elegant look.",
                     Price = 480m,
                     ImageUrl = "/images/cake-square.jpeg",
                     Category = "Standard Cakes",
                     IsAvailable = true
                 },

                 // ── Layer Cakes ───────────────────────────────────
                 new Product
                 {
                     Id = 9,
                     Name = "2 Layer Cake",
                     Description = "Two luscious layers of cake and filling, stacked high and decorated to impress. A step up for any special occasion.",
                     Price = 550m,
                     ImageUrl = "/images/cake-2layer.jpeg",
                     Category = "Layer Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 10,
                     Name = "3 Layer Cake",
                     Description = "Three decadent layers of flavour and frosting. This showstopper cake is perfect for milestone celebrations.",
                     Price = 700m,
                     ImageUrl = "/images/cake-3layer.jpeg",
                     Category = "Layer Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 11,
                     Name = "4 Layer Cake",
                     Description = "Four stunning layers of cake — tall, dramatic, and absolutely delicious. Built to make a lasting impression.",
                     Price = 850m,
                     ImageUrl = "/images/cake-4layer.jpeg",
                     Category = "Layer Cakes",
                     IsAvailable = true
                 },

                 // ── Wedding & Event Cakes ─────────────────────────
                 new Product
                 {
                     Id = 12,
                     Name = "2 Tier Cake",
                     Description = "An elegant two-tier cake crafted for weddings, engagements, and formal events. Designed and decorated to your vision.",
                     Price = 1200m,
                     ImageUrl = "/images/cake-2tier.jpeg",
                     Category = "Wedding & Event Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 13,
                     Name = "3 Tier Cake",
                     Description = "A breathtaking three-tier masterpiece for your most important day. Every detail is handcrafted with love and precision.",
                     Price = 1800m,
                     ImageUrl = "/images/cake-3tier.jpeg",
                     Category = "Wedding & Event Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 14,
                     Name = "4 Tier Cake",
                     Description = "The ultimate statement cake — four magnificent tiers for grand weddings and large events. Truly unforgettable.",
                     Price = 2400m,
                     ImageUrl = "/images/cake-4tier.jpeg",
                     Category = "Wedding & Event Cakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 15,
                     Name = "Custom Quote",
                     Description = "Have something truly unique in mind? Request a custom quote and we'll bring your dream cake to life, whatever the vision.",
                     Price = 0m,
                     ImageUrl = "/images/cake-custom.jpeg",
                     Category = "Wedding & Event Cakes",
                     IsAvailable = true
                 },

                 // ── Cupcakes ──────────────────────────────────────
                 new Product
                 {
                     Id = 16,
                     Name = "6 Cupcakes",
                     Description = "Half a dozen beautifully decorated cupcakes in your choice of flavour. Perfect for small treats and gifts.",
                     Price = 120m,
                     ImageUrl = "/images/cupcakes-6.jpeg",
                     Category = "Cupcakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 17,
                     Name = "12 Cupcakes",
                     Description = "A full dozen decorated cupcakes — great for parties, offices, or anyone who believes more is always better.",
                     Price = 220m,
                     ImageUrl = "/images/cupcakes-12.jpeg",
                     Category = "Cupcakes",
                     IsAvailable = true
                 },

                 new Product
                 {
                     Id = 18,
                     Name = "Custom Cupcake Box",
                     Description = "A bespoke box of cupcakes designed around your theme, colours, and occasion. Tell us your vision and we'll bake it.",
                     Price = 180m,
                     ImageUrl = "/images/cupcakes-custom.jpeg",
                     Category = "Cupcakes",
                     IsAvailable = true
                 }
             );
        }
    }
}