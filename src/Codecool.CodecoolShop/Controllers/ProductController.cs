using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Codecool.CodecoolShop.Daos;
using Codecool.CodecoolShop.Daos.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Codecool.CodecoolShop.Models;
using Codecool.CodecoolShop.Services;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Product = Domain.Product;

namespace Codecool.CodecoolShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            //ProductService = new ProductService(
            //    ProductDaoMemory.GetInstance(),
            //    ProductCategoryDaoMemory.GetInstance());
            _productService = productService;
        }

        public IActionResult Index()
        {
            //var products = ProductService.GetProductsForCategory(1);
            _logger.LogInformation("Opened index page");
            var pr = _productService.GetAllProducts();
            _productService.GetProductCategories();
            _productService.GetSuppliers();
            //_context.ProductCategories.ToList();
            //_context.Suppliers.ToList();
            return View(pr);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
