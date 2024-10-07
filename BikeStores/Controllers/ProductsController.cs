using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BikeStores.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;

namespace BikeStores.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BikeStoresContext _context;

        public ProductsController(BikeStoresContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, string? brand, string sortOrder = "name_asc") // Imposta un valore di default al sort in modo che cambi l'ordine già quando si trova nella pagina products
        {
            // Imposta i parametri di ordinamento
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            // Recupero delle marche per il menù a tendina
            var brands = await _context.Brands.OrderBy(b => b.BrandName).ToListAsync();
            ViewBag.Brands = new SelectList(brands, "BrandName", "BrandName", brand);

            // Inizializza la query sui prodotti
            var productsQuery = _context.Products
                .Include(p => p.Brand)      // Include il riferimento alla marca
                .Include(p => p.Category)   // Include il riferimento alla categoria
                .AsQueryable();             // Serve per mantenere la query costruita

            // Filtro per nome parziale
            if (!String.IsNullOrEmpty(searchString))
            {
                string patrialName = searchString.ToLower();
                productsQuery = productsQuery.Where(s => s.ProductName.ToLower().Contains(patrialName));
            }

            ViewData["CurrentFilter"] = searchString; // Passa il filtro alla View

            // Filtro per marca
            if (!string.IsNullOrEmpty(brand))
            {
                productsQuery = productsQuery.Where(p => p.Brand.BrandName == brand);
            }

            // Ordinamento per nome (crescente o decrescente)
            switch (sortOrder)
            {
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.ProductName);
                    break;
            }

            // Converte la query finale in lista solo alla fine
            var products = await productsQuery.ToListAsync();

            ViewBag.Header = "Products List";

            return View(products);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Recupera i dettagli del prodotto e la quantità totale
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Stocks) // Include la relazione con i magazzini
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Calcola la quantità totale disponibile
            ViewBag.TotalQuantity = product.Stocks.Sum(s => s.Quantity);

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,BrandId,CategoryId,ModelYear,ListPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,BrandId,CategoryId,ModelYear,ListPrice")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
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
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
