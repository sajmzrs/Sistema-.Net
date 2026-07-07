using AP.Core.Business;
using AP.Data;
using AP.MVC.Filter;
using AP.MVC.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    [CustomAuthorizationFilter]
    public class ProductsController : BaseController
    {
        private ProductBusiness _business = new ProductBusiness();

        // GET: Products
        public ActionResult Index(int page = 1, int pageSize = 10, string criteria = null, string field = null)
        {
            if (pageSize <= 0) pageSize = 10;
            if (page <= 0) page = 1;

            var productsQuery = _business.GetProducts(id: 0).AsQueryable();
            var hasSearch = !string.IsNullOrWhiteSpace(criteria) && !string.IsNullOrWhiteSpace(field);

            if (hasSearch)
            {
                if (field == "Rating")
                {
                    if (decimal.TryParse(criteria, out var ratingValue))
                    {
                        productsQuery = productsQuery.Where(p => p.Rating >= ratingValue);
                    }
                    else
                    {
                        productsQuery = Enumerable.Empty<Product>().AsQueryable();
                    }
                }
                else
                {
                    var criteriaLower = criteria.ToLower();
                    switch (field)
                    {
                        case "ProductName":
                            productsQuery = productsQuery.Where(p => p.ProductName != null && p.ProductName.ToLower().Contains(criteriaLower));
                            break;
                        case "Description":
                            productsQuery = productsQuery.Where(p => p.Description != null && p.Description.ToLower().Contains(criteriaLower));
                            break;
                        case "ModifiedBy":
                            productsQuery = productsQuery.Where(p => p.ModifiedBy != null && p.ModifiedBy.ToLower().Contains(criteriaLower));
                            break;
                        default:
                            productsQuery = Enumerable.Empty<Product>().AsQueryable();
                            break;
                    }
                }
            }

            var totalItems = productsQuery.Count();
            var totalPages = totalItems == 0 ? 1 : (int)System.Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages) page = totalPages;

            var pagedProducts = productsQuery
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Criteria = criteria;
            ViewBag.Field = field;

            return View(pagedProducts);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _business.GetProducts((int)id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/DetailsModal/5
        public ActionResult DetailsModal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _business.GetProducts((int)id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }

            return PartialView("_DetailsModalContent", product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,InventoryID,SupplierID,Description,Rating,CategoryID,LastModified,ModifiedBy,CreatedBy")] Product product)
        {
            if (ModelState.IsValid)
            {
                if(_business.SaveOrUpdate(product))
                    return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _business.GetProducts((int)id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,InventoryID,SupplierID,Description,Rating,CategoryID,LastModified,ModifiedBy,CreatedBy")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (_business.SaveOrUpdate(product))
                    return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _business.GetProducts((int)id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = _business.GetProducts((int)id).FirstOrDefault();
            _business.Delete(product.ProductID);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Search")]
        public ActionResult Search(string criteria, string field)
        {
            // SOLID: principio tal Y
            return RedirectToAction("Index", new { page = 1, pageSize = 10, criteria, field });
        }

        // GET: Products/Filter
        public JsonResult Filter(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                return Json(Enumerable.Empty<ProductViewModel>(), JsonRequestBehavior.AllowGet);

            if (criteria == "rating")
            {
                var products = _business.GetProducts().Select(x => new ProductViewModel
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    CategoryID = x.CategoryID,
                    InventoryID = x.InventoryID,
                    SupplierID = x.SupplierID,
                    Description = x.Description,
                    Rating = x.Rating,
                    ModifiedBy = x.ModifiedBy,
                    LastModified = x.LastModified,
                    CreatedBy = x.CreatedBy,
                }).OrderByDescending(x => x.Rating).Take(10).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }

            return Json(Enumerable.Empty<ProductViewModel>(), JsonRequestBehavior.AllowGet);
        }

        // GET: Products
        public JsonResult JsonCall()
        {
            // SOLID: principio tal X
            return Json(new { message = "Todo bien" }, JsonRequestBehavior.AllowGet);
        }
    }
}
