using System.Diagnostics;
using DigitalMagazineStore.Models;
using DigitalMagazineStore.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMagazineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger , IHomeRepository homeRepository)
        {
            _homeRepository=homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index( string sterm="", int categoryID=0)
        {
            
            IEnumerable<Magazine> magazines= await _homeRepository.GetMagazines(sterm, categoryID);
            IEnumerable<Category> categories= await _homeRepository.Categories();
            MagazineDisplay magazineModel = new MagazineDisplay
            {
                Magazines= magazines,
                Categories= categories ,
                STerm = sterm,
                CategoryID=categoryID
            };


            return View(magazineModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
