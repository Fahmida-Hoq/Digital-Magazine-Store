using DigitalMagazineStore.Constants;
using DigitalMagazineStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalMagazineStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminOperationsController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;
        public AdminOperationsController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderRepository.UserOrders(true);
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

        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _userOrderRepository.GetOrderById(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = $"Order with id:{orderId} not found.";
                return RedirectToAction(nameof(AllOrders));
            }

            var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
                new SelectListItem
                {
                    Value = orderStatus.Id.ToString(),
                    Text = orderStatus.StatusName,
                    Selected = order.OrderStatusId == orderStatus.Id
                });

            var data = new UpdateOrderStatusModel
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
                        new SelectListItem
                        {
                            Value = orderStatus.Id.ToString(),
                            Text = orderStatus.StatusName,
                            Selected = orderStatus.Id == data.OrderStatusId
                        });

                    data.OrderStatusList = orderStatusList;
                    TempData["ErrorMessage"] = "Invalid data provided.";
                    return View(data);
                }

                await _userOrderRepository.ChangeOrderStatus(data);
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating.";
            }

            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
