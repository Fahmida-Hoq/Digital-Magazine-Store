using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalMagazineStore.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
             UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int magazineId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");

                var shoppingcart = await GetCart(userId);
                if (shoppingcart is null)
                {
                    shoppingcart = new Cart { UserId = userId };
                    _db.Carts.Add(shoppingcart);
                }
                await _db.SaveChangesAsync();

                var cartItem = _db.CartDetails
                    .FirstOrDefault(a => a.CartId == shoppingcart.Id && a.MagazineId == magazineId);

                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var magazine = _db.Magazines.Find(magazineId)
                     ?? throw new ArgumentException("book not found");
                    cartItem = new CartDetail
                    {
                        MagazineId = magazineId,
                        CartId = shoppingcart.Id,
                        Quantity = qty,
                        UnitPrice = magazine.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var cartItemCount = await GetCartItemCount(userId);
                return cartItemCount;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> RemoveItem(int magazineId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");

                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.CartId == cart.Id && a.MagazineId == magazineId);
                if (cartItem is null)
                    throw new InvalidOperationException("No items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity--;

                await _db.SaveChangesAsync();

                var cartItemCount = await GetCartItemCount(userId);
                return cartItemCount;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Cart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid userid");

            var Cart = await _db.Carts
                .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Magazine)
                                  .ThenInclude(a => a.Stock)
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Magazine)
                                  .ThenInclude(a => a.Category)
                                  .Where(a => a.UserId == userId)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync();

            return Cart;
        }

        public async Task<Cart> GetCart(string userId)
        {
            var shoppingcart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            return shoppingcart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var data = await _db.Carts
                                .Include(c => c.CartDetails)
                                .Where(c => c.UserId == userId)
                                .SelectMany(c => c.CartDetails)
                                .CountAsync();
            return data;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");

                var cartDetail = _db.CartDetails
                                   .Where(a => a.CartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Cart is empty");

                var pendingRecord = _db.OrderStatuses
                                       .FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new Exception("Order status does not have pending status");

                var order = new Order
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // Save order first

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        MagazineId = item.MagazineId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Order = order
                    };
                    _db.OrderDetails.Add(orderDetail);

                    var stock = await _db.Stocks
                                         .FirstOrDefaultAsync(a => a.MagazineId == item.MagazineId);
                    if (stock == null)
                        throw new InvalidOperationException("Stock is null");

                    if (item.Quantity > stock.Quantity)
                        throw new InvalidOperationException(
                            $"Only {stock.Quantity} item(s) are available in stock");

                    stock.Quantity -= item.Quantity;
                }

                _db.CartDetails.RemoveRange(cartDetail);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
