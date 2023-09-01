using System;
using System.Collections.Generic;
using System.Linq;
using Codecool.CodecoolShop.Controllers;
using Codecool.CodecoolShop.Data;
using Data;
using Domain;
using Microsoft.Extensions.Logging;

namespace Codecool.CodecoolShop.Services
{
    public class CartService : ICartService
    {

        private readonly ILogger<CartController> _logger;
        private CodecoolshopContext _context;

        public CartService(ILogger<CartController> logger, CodecoolshopContext context)
        {
            _logger = logger;
            _context = context;
            CodecoolshopContext.IfDbEmptyAddNewItems(context);
        }

        public List<Domain.Product> GetAllProducts()
        {
            var products = _context.Products.ToList();
            return products;
        }

        public List<Domain.ProductCategory> GetProductCategories()
        {
            var productCategories = _context.ProductCategories.ToList();
            return productCategories;
        }

        public List<Domain.Supplier> GetSuppliers()
        {
            var suppliers = _context.Suppliers.ToList();
            return suppliers;
        }

        public List<Domain.Product> GetProductsByCategory(int categoryId)
        {
            var products = _context.Products.Where(p => p.ProductCategory.Id == categoryId).ToList();
            return products;
        }

        public List<Domain.Product> GetProductsBySupplier(int supplierId)
        {
            var products = _context.Products.Where(p => p.Supplier.Id == supplierId).ToList();
            return products;
        }

        public Domain.Product FindProductById(string id)
        {
            if (_context.Products.Any(product => product.Id.ToString() == id))
            {
                return _context.Products.First(product => product.Id.ToString() == id);
            }
            _logger.LogInformation($"There is no product with id {id}");
            throw new ArgumentException("Wrong Id");
        }

        public bool SaveCartToDb(string UserId, string items)
        {
            var cart = new Cart();
            cart.UserId = UserId;
            cart.ItemsJson = items;
            if (_context.Carts.Any(cart => cart.UserId == UserId))
            {
                _context.Carts.First(cart => cart.UserId == UserId).ItemsJson = items;
                _context.SaveChanges();
            }
            else if (cart.ItemsJson != null)
            {
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }
            
            return true;
        }
        public string ReadCartFromDb(string UserId)
        {
            var cart = "";
            if (_context.Carts.Any(cart => cart.UserId == UserId))
            {
                cart = _context.Carts.First(cart => cart.UserId == UserId).ItemsJson;
            }
            return cart;
        }
    }
}
