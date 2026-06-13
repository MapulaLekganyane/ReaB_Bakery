using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ReaB_Bakery.Data;
using ReaB_Bakery.Helpers;
using ReaB_Bakery.Models;

namespace ReaB_Bakery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _cart;
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(
            ApplicationDbContext context,
            ShoppingCart cart,
            EmailService emailService,
            ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _cart = cart;
            _emailService = emailService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.CartCount = _cart.Items.Sum(i => i.Quantity);
        }
        // ===========================
        // HOME / MENU
        // ===========================
        public IActionResult Index()
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsAvailable)
                    .ToList();

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products on Home page");

                // fallback so the site NEVER crashes
                return View(new List<Product>());
            }
        }

        public async Task<IActionResult> Menu(string category = null)
        {
            var products = _context.Products.Where(p => p.IsAvailable);

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            ViewBag.Category = category;
            ViewBag.Categories = await _context.Products
                .Where(p => p.IsAvailable)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.GroupedProducts = await _context.Products
                .Where(p => p.IsAvailable)
                .GroupBy(p => p.Category)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            return View(await products.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCustomization(
    int productId,
    string productName,
    string customerName,
    string customerEmail,
    string customerPhone,
    DateTime eventDate,
    string designNotes,
    IFormFile inspirationPhoto)
        {
            // 1. Save photo if uploaded
            string photoPath = null;
            if (inspirationPhoto != null && inspirationPhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath, "uploads", "customizations");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(inspirationPhoto.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await inspirationPhoto.CopyToAsync(stream);

                photoPath = $"/uploads/customizations/{fileName}";
            }

            // 2. Save to database
            var request = new CustomizationRequest
            {
                ProductId = productId,
                ProductName = productName,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                EventDate = eventDate,
                DesignNotes = designNotes,
                InspirationPhotoPath = photoPath,
                SubmittedAt = DateTime.Now,
                Status = "Pending"
            };
            _context.CustomizationRequests.Add(request);
            await _context.SaveChangesAsync();

            // 3. Email the bakery owner
            await _emailService.SendCustomizationRequestAsync(
                customerName, customerEmail, customerPhone, productName,
                eventDate, designNotes, photoPath);

            TempData["CustomSuccess"] =
                $"Thank you {customerName}! Your customization request has been sent. We'll be in touch soon! 🎂";

            return RedirectToAction("Menu");
        }
        // ===========================
        // ABOUT / CONTACTS
        // ===========================
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        // ===========================
        // CART METHODS
        // ===========================

        // Display full shopping cart page
        public IActionResult Cart()
        {
            // Pass your ShoppingCart model to the view
            return View(_cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound();

            _cart.AddItem(product.Id, product.Name, product.Price, quantity, product.ImageUrl);
            return RedirectToAction("Menu");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            _cart.UpdateQuantity(productId, quantity);
            return RedirectToAction("Cart");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId);
            return RedirectToAction("Cart");
        }

        public IActionResult ClearCart()
        {
            _cart.Clear();
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult GetCartSummary()
        {
            int totalItems = _cart.Items.Sum(i => i.Quantity);
            return Json(new { totalItems });
        }





        // ===========================
        // CHECKOUT
        // ===========================
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
                return RedirectToAction("Menu");

            // Create a new order with items from the cart
            var order = new Order
            {
                Items = _cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price
                }).ToList(),
                Subtotal = _cart.Items.Sum(i => i.Price * i.Quantity),
                DeliveryFee = 20,
                Total = _cart.Items.Sum(i => i.Price * i.Quantity) + 20
            };

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (!_cart.Items.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(order);
            }

            // Ensure Notes is not null
            order.Notes ??= string.Empty;

            order.Subtotal = _cart.Items.Sum(i => i.Price * i.Quantity);
            order.DeliveryFee = 20;
            order.Total = order.Subtotal + order.DeliveryFee;
            order.Status = "Pending";
            order.OrderDate = DateTime.Now;

            order.Items = _cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // EF Core will save Order + Items

            _ = _emailService.SendOrderConfirmationAsync(
                order.CustomerEmail,
                order.CustomerName,
                order.PaymentMethod,
                order.Total

            );
            // Send admin notification
            _ = _emailService.SendAdminOrderNotificationAsync(order);

            _cart.Clear();

            TempData["SuccessMessage"] = "Your order has been placed successfully!";
            TempData["OrderNumber"] = order.OrderNumber;

            return RedirectToAction("OrderConfirmation");
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceCustomOrder(CustomOrder order, IFormFile InspirationPhoto)
        {
            if (InspirationPhoto != null && InspirationPhoto.Length > 0)
            {
                var folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "orders");
                Directory.CreateDirectory(folder);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(InspirationPhoto.FileName)}";
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await InspirationPhoto.CopyToAsync(stream);
                order.InspirationPhotoPath = $"/uploads/orders/{fileName}";
            }

            order.OtherFlavour ??= string.Empty;
            order.CustomColour ??= string.Empty;
            order.DesignNotes ??= string.Empty;
            order.InspirationPhotoPath ??= string.Empty;
            order.OrderReference = $"REAB-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
            order.OrderedAt = DateTime.Now;
            order.Status = "Pending";

            var flavour = order.CakeFlavour == "Other" && !string.IsNullOrEmpty(order.OtherFlavour)
                ? order.OtherFlavour : order.CakeFlavour;

            var colour = order.CakeColour == "Custom" && !string.IsNullOrEmpty(order.CustomColour)
                ? order.CustomColour : order.CakeColour;

            _context.CustomOrders.Add(order);
            await _context.SaveChangesAsync();

            await _emailService.SendCustomOrderAsync(order, flavour, colour);

            var waMessage = Uri.EscapeDataString(
                $"🎂 New Order: {order.OrderReference}\n" +
                $"Product: {order.ProductName}\n" +
                $"Customer: {order.CustomerName}\n" +
                $"Phone: {order.CustomerPhone}\n" +
                $"Flavour: {flavour}\n" +
                $"Colour: {colour}\n" +
                $"Delivery: {order.DeliveryDate:dd MMM yyyy}\n" +
                $"Notes: {order.DesignNotes}");

            TempData["OrderReference"] = order.OrderReference;
            TempData["CustomerName"] = order.CustomerName;
            TempData["WhatsAppUrl"] = $"https://wa.me/27760708055?text={waMessage}";

            return RedirectToAction("CustomOrderConfirmation");
        }

        public IActionResult CustomOrderConfirmation()
        {
            return View();
        }



        // ===========================
        // ORDER CONFIRMATION
        // ===========================
        public IActionResult OrderConfirmation() => View();

        // ===========================
        // ERROR
        // ===========================
        public IActionResult Error() => View();
    }
}
