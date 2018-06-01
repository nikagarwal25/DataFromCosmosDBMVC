using Microsoft.SupplyChain.Shipment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestFromProdWebApp.Models;
using TestFromProdWebApp.Repository;

namespace TestFromProdWebApp.Controllers
{
    public class HomeController : Controller
    {
        List<Shipment> shipmentObj = new List<Shipment>();
       
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await CosmosDBRepository<Shipment>.GetItems();
            SearchViewModel searchViewObj = new SearchViewModel();
            searchViewObj.ShipmentResult = items.ToList();
            return View(searchViewObj);
        }

        [HttpPost]
        public async Task<PartialViewResult> Search(Shipment obj)
        {
            var items = await CosmosDBRepository<Shipment>.GetItems();
            shipmentObj = items.ToList();
            if(obj!= null)
            {
                if(!String.IsNullOrEmpty(obj.ServiceName))
                {
                    shipmentObj = shipmentObj.Where(m => m.ServiceName == obj.ServiceName).ToList();
                }
                if (!String.IsNullOrEmpty(obj.SenderName))
                {
                    shipmentObj = shipmentObj.Where(m => m.SenderName == obj.SenderName).ToList();
                }

            }
            return PartialView("SearchResult", shipmentObj);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}