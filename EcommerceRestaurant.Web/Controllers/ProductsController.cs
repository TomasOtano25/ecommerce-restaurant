namespace EcommerceRestaurant.Web.Controllers
{
    using Data;
    using Data.Entities;
    using Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IUserHelper userHelper;

        // private readonly DataContext _context;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            this.productRepository = productRepository;
            this.userHelper = userHelper;
            // _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(this.productRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await this.productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewProduct)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (viewProduct.ImageFile != null && viewProduct.ImageFile.Length > 0)
                {

                    var guid = Guid.NewGuid().ToString();

                    /* var uidName = Path.GetFileNameWithoutExtension(viewProduct.ImageFile.FileName)
                        + guid
                        + Path.GetExtension(viewProduct.ImageFile.FileName); */
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Products", file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await viewProduct.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Products/{file}";
                }

                var product = ToProduct(viewProduct, path);

                //TODO: Change for the logged user
                product.User = await this.userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await this.productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(viewProduct);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await this.productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var view = this.ToProductViewModel(product);
            return View(view);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel viewProduct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = viewProduct.ImageUrl;

                    if (viewProduct.ImageFile != null && viewProduct.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        /* var uidName = Path.GetFileNameWithoutExtension(viewProduct.ImageFile.FileName)
                            + guid
                            + Path.GetExtension(viewProduct.ImageFile.FileName); */
                        var file = $"{guid}.jpg";

                        path = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\Products",
                            file);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await viewProduct.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/Products/{file}";
                    }

                    //TODO: Change for the logged user
                    viewProduct.User = await this.userHelper.GetUserByEmailAsync("tomasotano25@gmail.com");

                    var product = this.ToProduct(viewProduct, path);

                    await this.productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await this.productRepository.ExistAsync(viewProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(viewProduct);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await this.productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await this.productRepository.GetByIdAsync(id);
            await this.productRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private Product ToProduct(ProductViewModel viewProduct, string path)
        {
            return new Product
            {
                Id = viewProduct.Id,
                ImageUrl = path,
                IsAvailabe = viewProduct.IsAvailabe,
                LastPurchase = viewProduct.LastPurchase,
                LastSale = viewProduct.LastSale,
                Name = viewProduct.Name,
                Price = viewProduct.Price,
                Stock = viewProduct.Stock,
                User = viewProduct.User
            };
        }

        private ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                IsAvailabe = product.IsAvailabe,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }


    }
}
