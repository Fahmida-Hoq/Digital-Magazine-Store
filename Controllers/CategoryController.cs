using DigitalMagazineStore.Models;
using DigitalMagazineStore.Models.DTOs;
using DigitalMagazineStore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMagazineStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repo;

        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entities = await _repo.GetCategories();

            var dtoList = entities.Select(e => new CategoryDTO
            {
                Id = e.Id,
                CategoryType = e.CategoryType
            }).ToList();

            return View(dtoList);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View(new CategoryDTO());
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var entity = new Category
            {
                CategoryType = dto.CategoryType
            };

            await _repo.AddCategory(entity);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _repo.GetCategoryById(id);
            if (entity == null) return NotFound();

            var dto = new CategoryDTO
            {
                Id = entity.Id,
                CategoryType = entity.CategoryType
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var entity = new Category
            {
                Id = dto.Id,
                CategoryType = dto.CategoryType
            };

            await _repo.UpdateCategory(entity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repo.GetCategoryById(id);
            if (entity != null)
            {
                await _repo.DeleteCategory(entity);
            }
            return RedirectToAction("Index");
        }
    }
}
