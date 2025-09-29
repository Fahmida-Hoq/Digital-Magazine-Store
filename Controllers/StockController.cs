using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMagazineStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;
        private readonly object _stockRepo;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _stockRepository.GetStocks(sTerm);
            // ensure view model is not null
            var model = stocks ?? Enumerable.Empty<StockDisplayModel>();
            return View(model);
        }

        public async Task<IActionResult> ManageStock(int magazineId)
        {
            var existingStock = await _stockRepository.GetStockByMagazineId(magazineId);
            var stock = new StockDTO
            {
                MagazineId = magazineId,
                Quantity = existingStock != null
            ? existingStock.Quantity : 0

            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepository.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 

