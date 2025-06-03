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
    public class CartsController : Controller
    {
        private readonly GroceryContext _context;

        public CartsController(GroceryContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var groceryContext = _context.Cart.Include(c => c.Product);
            return View(await groceryContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }
        public async Task<ActionResult> AddToCart() {
            Console.WriteLine("Add to Cart is processing");
            return RedirectToAction(nameof(Index));
        }
        public async Task<ActionResult> userCarts()
        {
            //Get the current logged-in user ID
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Check if User is authenticated
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }
                //Fetch all cart items for the user, including related Product data
                var userCartss = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                return View(userCartss);
            }
        }
        //Post: Cart/AddtoCarts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToCart(int productId, decimal quantity = 1)
        {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not Found");
            }
            var cart = new Cart
            {
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = product.Price * quantity,
                UserId = userIdString,
                CreatedDate = DateTime.Now
            };
            //Add to database
            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products");
        }
        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,TotalPrice,UserId,CreatedDate")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,TotalPrice,UserId,CreatedDate")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}
