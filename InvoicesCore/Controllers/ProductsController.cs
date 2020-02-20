using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoices.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InvoicesCore.Controllers
{
    public class ProductsController : Controller
    {
        public ProductsController()
        {
            ReconcileData.Init();
        }

        public async Task<string> All()
        {
            var products = ReconcileData.Products;
            var productsJson = JsonConvert.SerializeObject(products);
            return productsJson;
        }
    }
}