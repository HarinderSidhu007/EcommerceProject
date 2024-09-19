using Dapper;
using EcommProject_1147.DataAccess.Repository;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using EcommProject_1147.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EcommProject_1147.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        public CoverTypeController(IUnitofWork unitofWork)
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
            //  return Json(new { data = _unitOfWork.CoverType.GetAll() });
            return Json(new { data = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_GetCoverTypes) });
        }
        #endregion
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CoverTypeInDb = _unitOfWork.CoverType.Get(id);
            if (CoverTypeInDb == null)
                return Json(new { success = false, message = "something went wrong while delete data!!!" });
            //_unitOfWork.CoverType.Remove(CoverTypeInDb);
            //_unitOfWork.Save();
            DynamicParameters param= new DynamicParameters();
            param.Add("id", id);
            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCoverType,param);
            return Json(new { success = true, message = "data successfully deleted!!!" });
        }

        public IActionResult Upsert(int? id)
        {
            CoverType covertype = new CoverType();
            if (id == null) return View(covertype);
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());
            covertype = _unitOfWork.SP_CALL.oneRecord<CoverType>(SD.Proc_GetCoverType,param);
            // covertype = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (covertype == null) return NotFound();
            return View(covertype);   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            DynamicParameters param= new DynamicParameters();
            param.Add("id", coverType.Id);
            if (coverType.Id == 0)
                
            _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCoverType, param);
            // _unitOfWork.CoverType.Add(covertype);
           
            else
                param.Add("name", coverType.Name);
            _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCoverType, param);
            //   _unitOfWork.CoverType.Update(covertype);
            // _unitOfWork.Save();
            
            return RedirectToAction("Index");
        }

    }
}

