using Microsoft.AspNetCore.Mvc;
using Shopping.Data;
using Shopping.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Shopping.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DBContext _context;
        public ShoppingCartController(DBContext dbContext)
        {
            _context = dbContext;

        }
        [HttpPost]
        public IActionResult Index(int? Id, int Quantity)
        {
            if (!Id.HasValue)
            {
                return NotFound();
            }
            List<OrderDetail> cart;
            // 從Session中獲取購物車數據
            var cartJson = HttpContext.Session.GetString("cart");
            // 如果購物車為空，創建新的購物車
            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<OrderDetail>();
            }
            else
            {
                // 反序列化現有的購物車數據
                cart = JsonSerializer.Deserialize<List<OrderDetail>>(cartJson);

            }
            OrderDetail existingItem = cart.FirstOrDefault(x => x.ProductId == Id);
            if (existingItem != null)
            {
                existingItem.Quantity += Quantity;
                // 更新Session中的購物車數據
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));
                // 重定向到購物車頁面
                return RedirectToAction("ShoppingCart");
            }
            Product product = _context.Product.FirstOrDefault(x => x.Id == Id);
            if (product == null)
            {
                return NotFound();
            }

            OrderDetail newOrderDetail = new OrderDetail();
            newOrderDetail.ProductId = product.Id;
            newOrderDetail.ProductName = product.ProductName;
            newOrderDetail.ImagePath = product.ImagePath;
            newOrderDetail.Quantity = Quantity;
            newOrderDetail.Price = product.Price;

            cart.Add(newOrderDetail);
            // 更新Session中的購物車數據
            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));

            // 重定向到購物車頁面
            return RedirectToAction("ShoppingCart");
        }
        public IActionResult ShoppingCart()
        {
            List<OrderDetail> cart;
            // 從Session中獲取購物車數據
            var cartJson = HttpContext.Session.GetString("cart");
            // 如果購物車為空，創建新的購物車
            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<OrderDetail>();
            }
            else
            {
                // 反序列化現有的購物車數據
                cart = JsonSerializer.Deserialize<List<OrderDetail>>(cartJson);

            }

            return View(cart);
        }
        [HttpPost]
        public IActionResult ShoppingCart(List<OrderDetail> orderDetails)
        {
            return RedirectToAction("checkout");
        }
        [HttpPost]
        public IActionResult Update(int Id, int Quantity)
        {
            List<OrderDetail> cart;
            // 從Session中獲取購物車數據
            var cartJson = HttpContext.Session.GetString("cart");
            // 如果購物車為空，創建新的購物車
            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<OrderDetail>();
            }
            else
            {
                // 反序列化現有的購物車數據
                cart = JsonSerializer.Deserialize<List<OrderDetail>>(cartJson);

            }
            OrderDetail existingItem = cart.FirstOrDefault(x => x.ProductId == Id);
            if (existingItem != null)
            {
                existingItem.Quantity = Quantity;
                // 更新Session中的購物車數據
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));
            }

            return RedirectToAction("ShoppingCart");
        }
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            List<OrderDetail> cart;
            // 從Session中獲取購物車數據
            var cartJson = HttpContext.Session.GetString("cart");
            // 如果購物車為空，創建新的購物車
            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<OrderDetail>();
            }
            else
            {
                // 反序列化現有的購物車數據
                cart = JsonSerializer.Deserialize<List<OrderDetail>>(cartJson);

            }
            OrderDetail existingItem = cart.FirstOrDefault(x => x.ProductId == Id);
            
            if (existingItem != null)
            {
                cart.Remove(existingItem);
                // 更新Session中的購物車數據
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));
            }
            
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult checkout()
        {
            return View();
        }
        [HttpPost]
        public IActionResult checkout(Order order)
        {
            if (ModelState.IsValid) 
            {
                List<OrderDetail> cart;
                // 從Session中獲取購物車數據
                var cartJson = HttpContext.Session.GetString("cart");
                // 如果購物車為空，創建新的購物車
                if (string.IsNullOrEmpty(cartJson))
                {
                    cart = new List<OrderDetail>();
                }
                else
                {
                    // 反序列化現有的購物車數據
                    cart = JsonSerializer.Deserialize<List<OrderDetail>>(cartJson);

                }
                foreach (OrderDetail detail in cart) 
                {
                    order.OrderDetails.Add(detail);
                }
                _context.Add(order);
                _context.SaveChanges();
                return RedirectToAction("complete",new {id=order.Id });
            }

            return View();
        }

        public IActionResult complete(int? Id)
        {
            if (!Id.HasValue) 
            {
                return NotFound();
            }
            Order order = _context.Order.Include(x => x.OrderDetails).FirstOrDefault(x => x.Id == Id);
            if (order == null) 
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
