using EcommProject_1147.DataAccess.Repository;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EcommProject_1147.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        public CategoryController(IUnitofWork unitofWork)
        {
            _unitOfWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }
        #endregion
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CategoryInDb = _unitOfWork.Category.Get(id);
            if(CategoryInDb==null)
                return Json(new {success= false, message="something went wrong while delete data!!!"});
            _unitOfWork.Category.Remove(CategoryInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data successfully deleted!!!" });
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View(category); //create
                                                   //Edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category) 
        {
        if (category == null) return NotFound();
        if (!ModelState.IsValid) return View(category);
        if(category.Id==0)
                _unitOfWork.Category.Add(category);
        else
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}

