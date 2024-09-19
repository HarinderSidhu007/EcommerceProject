using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using EcommProject_1147.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EcommProject_1147.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitofWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofwork.Product.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productIdDb = _unitofwork.Product.Get(id);
            if (productIdDb == null)
                return Json(new { success = false,
                message = "something went wrong while delete data!!!" });
            //Image Delete
            var webRootPath= _webHostEnvironment.WebRootPath;
            var imagePath=Path.Combine(webRootPath,productIdDb.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(imagePath)) 
            {
                System.IO.File.Delete(imagePath);
            }
            //Data
            _unitofwork.Product.Remove(productIdDb);
            _unitofwork.Save();
            return Json(new { success = true, message = "data deleted successfully!!!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofwork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                })
            };
            if (id == null) return View(productVM);//create
            //Edit
            productVM.Product = _unitofwork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null) return NotFound();
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid)
            {
                var WebRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(WebRootPath, @"images\Products");
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                    if (productVM.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(WebRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;

                    }
                }
                if (productVM.Product.Id == 0)

                    _unitofwork.Product.Add(productVM.Product);

                else

                    _unitofwork.Product.Update(productVM.Product);
                _unitofwork.Save();
                return RedirectToAction("Index");

            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitofwork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString(),
                    }),
                    CoverTypeList = _unitofwork.CoverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString(),
                    })
                };
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitofwork.Product.Get(productVM.Product.Id);

                }
                return View(productVM);


            }

        }
    }
}
