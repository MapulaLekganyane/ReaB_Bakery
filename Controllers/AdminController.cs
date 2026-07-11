using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReaB_Bakery.Data;

namespace ReaB_Bakery.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard?status=Pending
        public async Task<IActionResult> Dashboard(string? status)
        {
            var query = _context.CustomOrders.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderedAt)
                .ToListAsync();

            ViewBag.SelectedStatus = status ?? "All";
            ViewBag.StatusCounts = new Dictionary<string, int>
            {
                ["All"] = await _context.CustomOrders.CountAsync(),
                ["Pending"] = await _context.CustomOrders.CountAsync(o => o.Status == "Pending"),
                ["In Progress"] = await _context.CustomOrders.CountAsync(o => o.Status == "In Progress"),
                ["Completed"] = await _context.CustomOrders.CountAsync(o => o.Status == "Completed")
            };

            ViewBag.CompletedRevenue = await _context.CustomOrders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.ProductPrice);

            ViewBag.FilteredTotal = orders.Sum(o => o.ProductPrice);

            return View(orders);
        }
        // POST: /Admin/UpdateStatus - quick status change from the order list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? returnStatus)
        {
            var order = await _context.CustomOrders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order {order.OrderReference} marked as {status}.";
            return RedirectToAction("Dashboard", new { status = returnStatus });
        }

        // GET: /Admin/OrderDetail/5
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.CustomOrders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: /Admin/OrderDetail/5 - edit status, price, and design notes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDetail(
            int id,
            string status,
            decimal productPrice,
            string designNotes,
            DateTime deliveryDate)
        {
            var order = await _context.CustomOrders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            order.Status = status;
            order.ProductPrice = productPrice;
            order.DesignNotes = designNotes ?? string.Empty;
            order.DeliveryDate = deliveryDate;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order {order.OrderReference} updated.";
            return RedirectToAction("OrderDetail", new { id = order.Id });
        }
    }
}