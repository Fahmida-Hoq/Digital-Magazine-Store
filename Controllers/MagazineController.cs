using DigitalMagazineStore.Constants;
using DigitalMagazineStore.Models;
using DigitalMagazineStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalMagazineStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class MagazineController : Controller
    {
        private readonly IMagazineRepository _magazineRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IFileService _fileService;

        public MagazineController(IMagazineRepository magazineRepo, ICategoryRepository categoryRepo, IFileService fileService)
        {
            _magazineRepo = magazineRepo;
            _categoryRepo = categoryRepo;
            _fileService = fileService;
        }

        // Show all magazines
        public async Task<IActionResult> Index()
        {
            var magazines = await _magazineRepo.GetMagazines();
            return View(magazines);
        }

        // GET AddMagazine
        public async Task<IActionResult> AddMagazine()
        {
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.CategoryType,
                Value = category.Id.ToString(),
            });

            MagazineDTO magazineToAdd = new() { CategoryList = categorySelectList };
            return View(magazineToAdd);
        }

        // POST AddMagazine
        [HttpPost]
        public async Task<IActionResult> AddMagazine(MagazineDTO magazineToAdd)
        {
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.CategoryType,
                Value = category.Id.ToString(),
            });
            magazineToAdd.CategoryList = categorySelectList;

            if (!ModelState.IsValid)
                return View(magazineToAdd);

            try
            {
                if (magazineToAdd.CoverImageFile != null)
                {
                    if (magazineToAdd.CoverImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file cannot exceed 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(magazineToAdd.CoverImageFile, allowedExtensions);
                    magazineToAdd.CoverImage = imageName;
                }

                Magazine magazine = new()
                {
                    Id = magazineToAdd.Id,
                    Title = magazineToAdd.Title,
                    AuthorName = magazineToAdd.AuthorName,   
                    Price = magazineToAdd.Price,
                    Image = magazineToAdd.CoverImage,
                    CategoryId = magazineToAdd.CategoryId
                };

                await _magazineRepo.AddMagazine(magazine);
                TempData["SuccessMessage"] = "Magazine added successfully";  
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(magazineToAdd);
            }
        }

        // GET UpdateMagazine
        public async Task<IActionResult> UpdateMagazine(int id)
        {
            var magazine = await _magazineRepo.GetMagazineById(id);
            if (magazine == null)
            {
                TempData["ErrorMessage"] = $"Magazine with id: {id} not found";
                return RedirectToAction(nameof(Index));
            }

            var categorySelectList = (await _categoryRepo.GetCategories()).Select(c => new SelectListItem
            {
                Text = c.CategoryType,
                Value = c.Id.ToString(),
                Selected = c.Id == magazine.CategoryId
            });

            MagazineDTO dto = new()
            {
                Id = magazine.Id,
                Title = magazine.Title,
                AuthorName = magazine.AuthorName,   
                Price = magazine.Price,
                CoverImage = magazine.Image,
                CategoryId = magazine.CategoryId,
                CategoryList = categorySelectList
            };

            return View(dto);
        }

        // POST UpdateMagazine
        [HttpPost]
        public async Task<IActionResult> UpdateMagazine(MagazineDTO dto)
        {
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(c => new SelectListItem
            {
                Text = c.CategoryType,
                Value = c.Id.ToString(),
                Selected = c.Id == dto.CategoryId
            });
            dto.CategoryList = categorySelectList;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                string oldImage = dto.CoverImage;

                if (dto.CoverImageFile != null)
                {
                    if (dto.CoverImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file cannot exceed 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(dto.CoverImageFile, allowedExtensions);
                    dto.CoverImage = imageName;

                    if (!string.IsNullOrWhiteSpace(oldImage))
                        _fileService.DeleteFile(oldImage);
                }

                Magazine magazine = new()
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    AuthorName = dto.AuthorName,
                    Price = dto.Price,
                    Image = dto.CoverImage,
                    CategoryId = dto.CategoryId
                };

                await _magazineRepo.UpdateMagazine(magazine);
                TempData["SuccessMessage"] = "Magazine updated successfully";  
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return View(dto);
            }
        }

        // Delete
        public async Task<IActionResult> DeleteMagazine(int id)
        {
            try
            {
                var magazine = await _magazineRepo.GetMagazineById(id);
                if (magazine == null)
                {
                    TempData["ErrorMessage"] = $"Magazine with id: {id} not found";
                }
                else
                {
                    await _magazineRepo.DeleteMagazine(magazine);

                    if (!string.IsNullOrWhiteSpace(magazine.Image))
                        _fileService.DeleteFile(magazine.Image);

                    TempData["SuccessMessage"] = "Magazine deleted successfully"; 
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
