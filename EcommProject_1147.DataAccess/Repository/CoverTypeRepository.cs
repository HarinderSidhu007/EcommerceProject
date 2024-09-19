using EcommProject_1147.DataAccess.Data;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject_1147.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
