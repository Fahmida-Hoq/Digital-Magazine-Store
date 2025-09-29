using DigitalMagazineStore.Models.DTOs;

namespace DigitalMagazineStore.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int magazineId, int qty);
        Task<int> RemoveItem(int magazineId);
        Task<Cart> GetUserCart();

        Task<int> GetCartItemCount(string userId = "");
        Task<Cart> GetCart(string userId);
       Task<bool> DoCheckout(CheckoutModel model);
       
    }
}
