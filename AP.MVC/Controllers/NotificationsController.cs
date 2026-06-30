using AP.Core.Business;
using AP.Data;
using AP.MVC.Filter;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    [CustomAuthorizationFilter]
    public class NotificationsController : BaseController
    {
        private NotificationBusiness _business = new NotificationBusiness();

        // GET: Notifications
        public ActionResult Index(int page = 1, int pageSize = 10, string criteria = null, string field = null, string filter = null)
        {
            if (pageSize <= 0) pageSize = 10;
            if (page <= 0) page = 1;

            var query = _business.GetNotifications(id: 0).AsQueryable();

            // Filtros secundarios tienen prioridad sobre la búsqueda por campo
            if (!string.IsNullOrWhiteSpace(filter))
            {
                // user_id de demo para los filtros por usuario
                int demoUserId = 1;
                switch (filter)
                {
                    case "last10":
                        query = _business.GetLast10ByUser(demoUserId).AsQueryable();
                        break;
                    case "first10":
                        query = _business.GetFirst10ByUser(demoUserId).AsQueryable();
                        break;
                    case "even":
                        query = _business.GetEven().AsQueryable();
                        break;
                    case "odd":
                        query = _business.GetOdd().AsQueryable();
                        break;
                    case "active":
                        query = _business.GetByStatus(false).AsQueryable();
                        break;
                    case "inactive":
                        query = _business.GetByStatus(true).AsQueryable();
                        break;
                }
            }
            else if (!string.IsNullOrWhiteSpace(criteria) && !string.IsNullOrWhiteSpace(field))
            {
                var criteriaLower = criteria.ToLower();
                switch (field)
                {
                    case "message":
                        query = query.Where(n => n.message != null && n.message.ToLower().Contains(criteriaLower));
                        break;
                    case "user_id":
                        if (int.TryParse(criteria, out int uid))
                            query = query.Where(n => n.user_id == uid);
                        else
                            query = Enumerable.Empty<Notification>().AsQueryable();
                        break;
                    case "created_at":
                        query = query.Where(n => n.created_at != null && n.created_at.ToString().Contains(criteria));
                        break;
                    default:
                        query = Enumerable.Empty<Notification>().AsQueryable();
                        break;
                }
            }

            var totalItems = query.Count();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages) page = totalPages;

            var paged = query
                .OrderBy(n => n.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Criteria = criteria;
            ViewBag.Field = field;
            ViewBag.Filter = filter;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_NotificationsTable", paged);
            }

            return View(paged);
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Notification notification = _business.GetNotifications((int)id).FirstOrDefault();
            if (notification == null)
                return HttpNotFound();

            return View(notification);
        }

        // GET: Notifications/DetailsModal/5
        public ActionResult DetailsModal(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Notification notification = _business.GetNotifications((int)id).FirstOrDefault();
            if (notification == null)
                return HttpNotFound();

            return PartialView("_DetailsModalContent", notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,message,is_read,created_at")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_business.SaveOrUpdate(notification))
                        return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar la notificación: " + ex.Message);
                }
            }
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Notification notification = _business.GetNotifications((int)id).FirstOrDefault();
            if (notification == null)
                return HttpNotFound();

            return View(notification);
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,message,is_read,created_at")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_business.SaveOrUpdate(notification))
                        return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar la notificación: " + ex.Message);
                }
            }
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Notification notification = _business.GetNotifications((int)id).FirstOrDefault();
            if (notification == null)
                return HttpNotFound();

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _business.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Search")]
        public ActionResult Search(string criteria, string field)
        {
            return RedirectToAction("Index", new { page = 1, pageSize = 10, criteria, field });
        }

        // GET: Notifications/JsonCall
        public JsonResult JsonCall()
        {
            return Json(new { message = "Todo bien desde Notificaciones" }, JsonRequestBehavior.AllowGet);
        }
    }
}
