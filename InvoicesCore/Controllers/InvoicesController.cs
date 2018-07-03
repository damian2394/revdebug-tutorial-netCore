using System;
using System.Linq;
using Invoices.DataAccess;
using Invoices.Models;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Invoices.Controllers
{
    public class InvoicesController : Controller
    {
        private InvoicesContext db;

        public InvoicesController(InvoicesContext invoicesContext)
        {
            db = invoicesContext;
        }

        // GET: Invoices
        public ActionResult Index()
        {
           // var invoices = db.Invoices.Include(i => i.Contractor);
            return View(db.Invoices.Include(x => x.Contractor).Include(x => x.InvoiceEntries).ThenInclude(x => x.Product).ToList());
        }

        // GET: Invoices/Details/5
        public ActionResult Details(string id)
        {
            ReconcileData.Init();
            if (id == null)
            {
                return BadRequest();
            }
            Invoice invoice = db.Invoices.Include(x => x.InvoiceEntries).ThenInclude(x => x.Product).FirstOrDefault(x => x.Number == int.Parse(id));
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "FirstName");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromBody] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "FirstName", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "FirstName", invoice.AccountId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromBody] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "FirstName", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reconsile(string id)
        {
            Invoice invoice = db.Invoices.FirstOrDefault(x => x.Number == int.Parse(id));

            invoice.Reconciled = CalculateReconcilation(invoice.InvoiceId);
            db.Entry(invoice).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details",new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private double CalculateReconcilation(string id)
        {
            double reconsiled = 0;
            var rows = ReconcileData.InvoiceEntries.Select($"InvoiceId ='{id}'");
            var Products = ReconcileData.Products;
            foreach (var row in rows)
            {
                var product = Products.Select($"ProductId = '{row["ProductId"]}'").FirstOrDefault();
                var productName = product["Label"];
                var unitPrice = double.Parse((string)product["UnitPrice"]);
                var taxRate = Int32.Parse((string)product["Tax"]);
                var quantity = int.Parse((string)row["quantity"]);
                var tax = unitPrice * taxRate / 100;

                reconsiled += (unitPrice + tax) * quantity;
            }
            return reconsiled;
        }
    }
}
