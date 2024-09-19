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
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _context;
        public UnitofWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            CoverType= new CoverTypeRepository(context);
            SP_CALL = new SP_CALL(context);
            Product= new ProductRepository(context);
            Company= new CompanyRepository(context);
        }
        public ICategoryRepository Category { private set; get; }

        public ICoverTypeRepository CoverType { private set; get; }
        public  ISP_CALL SP_CALL { private set; get; }
        public IProductRepository Product { private set; get; }
        public ICompanyRepository Company { private set; get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
