using EcommProject_1147.DataAccess.Repository;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using EcommProject_1147.Models.ViewModels;
using EcommProject_1147.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommProject_1147.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CartController(IUnitofWork unitofwork)
        {
            _unitofWork = unitofwork;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public List<ShoppingCart> ListCart { get; private set; }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null)
            {
                ShoppingCartVM = new ShoppingCartVM();
                {
                    ListCart = new List<ShoppingCart>();
                };
                return View(ShoppingCartVM);
            }
            //***
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claims.Value,
                includeProperties: "Product"),
                OrderHeader = new OrderHeader(),
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault
                (au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                if (list.Product.Description.Length > 100) 
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            return View(ShoppingCartVM);
        }
        public IActionResult plus(int id) 
        {
            var cart = _unitofWork.ShoppingCart.Get(id);
            if(cart == null) return NotFound();
            cart.Count += 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cart = _unitofWork.ShoppingCart.Get(id);
            if (cart == null) return NotFound();
            if(cart.Count==1)
                cart.Count=1;
            else
                cart.Count-=1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var cart = _unitofWork.ShoppingCart.Get(id);
            if (cart == null) return NotFound();
            _unitofWork.ShoppingCart.Remove(cart);
            _unitofWork.Save();
            //session count
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                var count = _unitofWork.ShoppingCart.GetAll
                    (Sc => Sc.ApplicationUserId == claims.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == Claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser=_unitofWork.ApplicationUser
                .FirstOrDefault(au=>au.Id==Claims.Value);
            foreach(var list in ShoppingCartVM.ListCart) 
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,list.Product.Price,
                    list.Product.Price50,list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if(list.Product.Description.Length>100) 
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost() 
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var Claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.
                    FirstOrDefault(au => au.Id == Claims.Value);

                ShoppingCartVM.ListCart = _unitofWork.ShoppingCart.GetAll
                    (sc => sc.ApplicationUserId == Claims.Value,
                    includeProperties: "Product");
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                ShoppingCartVM.OrderHeader.ApplicationUserId = Claims.Value;
                //ShoppingCartVM.OrderHeader.TrackingNumber = Guid.NewGuid().ToString();
                _unitofWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _unitofWork.Save();
                foreach (var list in ShoppingCartVM.ListCart)
                {
                    list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                        list.Product.Price50, list.Product.Price100);
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        ProductId = list.ProductId,
                        OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                        Price = list.Price,
                        Count = list.Count,
                    };
                    ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                    _unitofWork.OrderDetails.Add(orderDetails);
                    _unitofWork.Save();
                }
                _unitofWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
                _unitofWork.Save();
                //Session Count
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
                //****
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }
            catch(Exception ex)
            {
                throw ex;
            }
         
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
