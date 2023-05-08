﻿using System;
using Codecool.CodecoolShop.Helpers;
using Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Product = Domain.Product;
using Codecool.CodecoolShop.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Codecool.CodecoolShop.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Item>>("cart") ?? new List<Item>();
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.DefaultPrice * item.Quantity);
            return View();
        }

        [Route("buy/{id}")]
        public IActionResult Buy(string id)
        {
            _cartService.GetProductCategories();
            _cartService.GetSuppliers();
            if (_cartService.FindProductBy(id) == null) return RedirectToAction("Index");
            if (HttpContext.Session.GetObjectFromJson<List<Item>>("cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = _cartService.FindProductBy(id), Quantity = 1 });
                HttpContext.Session.SetObjectAsJson("cart", cart);
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<Item>>("cart");
                var index = IsExist(id, cart);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = _cartService.FindProductBy(id), Quantity = 1 });
                }
                HttpContext.Session.SetObjectAsJson("cart", cart);

            }

            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(string id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Item>>("cart");
            var index = IsExist(id, cart);
            cart.RemoveAt(index);
            HttpContext.Session.SetObjectAsJson("cart", cart);
            return RedirectToAction("Index");
        }

        [Route("update/{id}/{quantity}")]
        public IActionResult Update(string id,string quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Item>>("cart");
            var index = IsExist(id, cart);
            var isItInt = int.TryParse(quantity, out var result);
            if (result <= 0)
            {
                return Remove(id);
            }
            if (index != -1 && isItInt)
            {
                cart[index].Quantity = result;
            }
            HttpContext.Session.SetObjectAsJson("cart", cart);
            return RedirectToAction("Index");
        }

        private int IsExist(string id, List<Item> cart)
        {
            for (var i = 0; i < cart.Count; i++)
            {

                if (cart[i].Product.Id.ToString().Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        [HttpPost]
        public IActionResult InputQuantity(string id)
        {
            var quantity = Request.Form["quantity"].First();
            return Update(id, quantity);
        }
    }
}
