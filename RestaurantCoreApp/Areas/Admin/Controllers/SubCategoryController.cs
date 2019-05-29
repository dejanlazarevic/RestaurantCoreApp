using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantCoreApp.Data;
using RestaurantCoreApp.Models;
using RestaurantCoreApp.Models.ViewModels;
using RestaurantCoreApp.Utility;

namespace RestaurantCoreApp.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
		private readonly ApplicationDbContext _db;

		[TempData]
		public string StatusMessage { get; set; }

		public SubCategoryController(ApplicationDbContext db)
		{
			_db = db;
		}

		//GET Index
		public async Task<IActionResult> Index()
		{
			var subCategories = await _db.SubCategory.Include(s=>s.Category).ToListAsync();
			return View(subCategories);
		}

		// GET- CREATE
		public async Task<IActionResult> Create()
		{
			SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
			{
				CategoryList = await _db.Category.ToListAsync(),
				SubCategory = new Models.SubCategory(),
				SubCategoryList = await _db.SubCategory.OrderBy(p=>p.Name).Select(p=>p.Name).Distinct().ToListAsync()
			};

			return View(model);
		}

		//POST - CREATE
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
		{
			if (ModelState.IsValid)
			{
				var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

				if (doesSubCategoryExists.Count() > 0)
				{
					//Error
					StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name. ";
				}
				else
				{
					_db.SubCategory.Add(model.SubCategory);
					await _db.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}

			}
			SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
			{
				CategoryList = await _db.Category.ToListAsync(),
				SubCategory = model.SubCategory,
				SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
				StatusMessage = StatusMessage
			};
			return View(modelVm);
		}

		[ActionName("GetSubCategory")]
		public async Task<IActionResult> GetSubCategory(int id)
		{
			List<SubCategory> subCategories = new List<SubCategory>();
			subCategories = await (from subCategory in _db.SubCategory
							 where subCategory.CategoryId == id
							 select subCategory).ToListAsync();
			return Json(new SelectList(subCategories, "Id", "Name"));
		}

		// GET- EDIT
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

			if (subCategory == null)
			{
				return NotFound();
			}

			SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
			{
				CategoryList = await _db.Category.ToListAsync(),
				SubCategory = subCategory,
				SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
			};

			return View(model);
		}

		//POST - EDIT
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
		{
			if (ModelState.IsValid)
			{
				var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

				if (doesSubCategoryExists.Count() > 0)
				{
					//Error
					StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name. ";
				}
				else
				{
					var subCatFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
					subCatFromDb.Name = model.SubCategory.Name;

					await _db.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}

			}
			SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
			{
				CategoryList = await _db.Category.ToListAsync(),
				SubCategory = model.SubCategory,
				SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
				StatusMessage = StatusMessage
			};
			return View(modelVm);
		}


		// GET- DETAILS
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

			if (subCategory == null)
			{
				return NotFound();
			}

			return View(subCategory);
		}

		// GET- DELETE
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

			if (subCategory == null)
			{
				return NotFound();
			}

			return View(subCategory);
		}

		//POST - DELETE
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
			if(subCategory == null)
			{
				return View();
			}
			_db.SubCategory.Remove(subCategory);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		/*
		 //POST - DELETE
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			var category = await _db.Category.FindAsync(id);
			if (category == null)
			{
				return View();
			}
			_db.Category.Remove(category);
			await _db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		 */


	}
}