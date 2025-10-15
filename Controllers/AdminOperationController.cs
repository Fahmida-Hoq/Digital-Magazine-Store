using DigitalMagazineStore.Constants;
using DigitalMagazineStore.Data;
using DigitalMagazineStore.Models;
using DigitalMagazineStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DigitalMagazineStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminOperationsController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;
        private readonly ApplicationDbContext _context;

        public AdminOperationsController(IUserOrderRepository userOrderRepository, ApplicationDbContext context)
        {
            _userOrderRepository = userOrderRepository;
            _context = context;
        }

        
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderRepository.UserOrders();
            return View(orders);
        }


        
        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderRepository.TogglePaymentStatus(orderId);
                TempData["SuccessMessage"] = "Payment status updated successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to update payment status.";
            }
            return RedirectToAction(nameof(AllOrders));
        }

        
        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = $"Order with ID {orderId} not found.";
                return RedirectToAction(nameof(AllOrders));
            }

            var orderStatuses = await _context.OrderStatuses.ToListAsync();

            var model = new UpdateOrderStatusModel
            {
                OrderId = order.Id,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = new SelectList(orderStatuses, "Id", "StatusName", order.OrderStatusId)
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                var orderStatuses = await _context.OrderStatuses.ToListAsync();
                model.OrderStatusList = new SelectList(orderStatuses, "Id", "StatusName", model.OrderStatusId);
                TempData["ErrorMessage"] = "Invalid data provided. Please try again.";
                return View(model);
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == model.OrderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(AllOrders));
            }

            order.OrderStatusId = model.OrderStatusId;

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error while updating order status.";
            }

            return RedirectToAction(nameof(AllOrders));
        }

      
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
