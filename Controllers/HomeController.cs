using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using X.PagedList;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
       
        private DBContext _dbContext;

        public HomeController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult>  Index(int? page,int? CategoryId)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;
            List<Banner> banners = await _dbContext.Banners.ToListAsync();
            List<ProductCategory> productCategories=await _dbContext.ProductCategory.ToListAsync();
            var products = _dbContext.Product.Where(x => x.Status == ProductStatus.¤W¬[);
            if (CategoryId.HasValue) 
            {
                products = products.Where(x => x.ProductCategoryId == CategoryId);
            }
            ViewBag.Banners = banners;
            ViewBag.ProductCategories = productCategories;

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Detail(int? Id)
        {
            if (!Id.HasValue) 
            {
                return NotFound();
            }
            Product product=_dbContext.Product.Include(x=>x.ProductCategory).FirstOrDefault(x => x.Id == Id);
            if (product ==null)
            {
                return NotFound();
            }

            return View(product);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
