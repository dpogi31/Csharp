using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grocery.Data;
using Grocery.Models;
using System.Security.Claims;

namespace Grocery.Controllers
{
    public class OrdersController : Controller
    {
        private readonly GroceryContext _context;

        public OrdersController(GroceryContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }
        public async Task<IActionResult> Reciept()
        {
            //Process data
            return View();
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }
        //POST : Orders/Checkout
        public async Task<IActionResult> Checkout(string paymentMethod)
        {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var userCarts = await _context.Cart
                 .Where(c => c.UserId == userIdString)
                 .ToArrayAsync();

            var order = new Order
            {
                userId = userIdString,
                orderDate = DateTime.Now,
                status = "Pending",
                totalAmount = userCarts.Sum(c => c.TotalPrice)
            };
            _context.Order.Add(order);
            await _context.SaveChangesAsync(); //saving order to db

            var orderItems = userCarts.Select(c => new OrderItem
            {
                orderId = order.Id,
                cartId = c.Id,
                quantity = c.Quantity,
                price = c.TotalPrice / c.Quantity, //unit price
                totalPrice = c.TotalPrice,
                discount = 0


            } ).ToList();

            _context.OrderItem.AddRange(orderItems);
            var payment = new Payment
            {
                userId = userIdString,
                orderId = order.Id,
                paymentMethod = paymentMethod,
                amount = order.totalAmount,
            };
            _context.Payment.Add(payment);
            _context.Cart.RemoveRange(userCarts);
            await _context.SaveChangesAsync();

            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,userId,orderDate,status,totalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,userId,orderDate,status,totalAmount")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
