using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EcommProject_1147.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CompanyController(IUnitofWork unitofWork) 
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Json (new {data=_unitofWork.Company.GetAll()});
        }
        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            var CompanyInDb = _unitofWork.Company.Get(id);
            if (CompanyInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data!!!" });
            _unitofWork.Company.Remove(CompanyInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "data deleted successfully!!!" });
        }
        #endregion
        public IActionResult Upsert(int?id)
        {
            Company company = new Company();
            if (id == null) return View(company);
            company= _unitofWork.Company.Get(id.GetValueOrDefault());
            if(company==null) return NotFound(); 
                return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (company == null) return NotFound();
            if(!ModelState.IsValid) return View(company);
            if(company.Id== 0)
                _unitofWork.Company.Add(company);
            else
            _unitofWork.Company.Update(company);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
