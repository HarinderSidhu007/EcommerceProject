using EcommProject_1147.DataAccess.Data;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using EcommProject_1147.Utility;
using Microsoft.AspNetCore.Mvc;

namespace EcommProject_1147.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitofWork unitofWork, ApplicationDbContext context)
        {
            _unitofWork = unitofWork;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList(); //aspnetuser
            var roles = _context.Roles.ToList();//aspnetroles
            var userRoles = _context.UserRoles.ToList();//aspnetuserroles
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.CompanyId != null)
                {
                    user.company = new Company()
                    {
                        Name = _unitofWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
                if (user.CompanyId == null)
                {
                    user.company = new Company()
                    {
                        Name = ""
                    };
                }
                
             

            }
            //Remove admin role user from list
            var admiuser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(admiuser);
            return Json(new { data = userList });

        }
    }
        #endregion
}

