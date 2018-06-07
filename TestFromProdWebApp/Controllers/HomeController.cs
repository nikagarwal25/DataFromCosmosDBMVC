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
            ViewBag.RecordCount = items.Count();
            GetDropDowns();
            return View(searchViewObj);
        }

        public void GetDropDowns()
        {
            List<SelectListItem> ServicesList = new List<SelectListItem>();
            ServicesList.Add(new SelectListItem() { Text = "All", Value = "All" });
            ServicesList.Add(new SelectListItem() { Text = "CMShipment", Value = "CMShipment" });
            ServicesList.Add(new SelectListItem() { Text = "RCShipment", Value = "RCShipment" });
            ViewBag.ServiceNameList = ServicesList;

            List<SelectListItem> ScenariosList = new List<SelectListItem>();
            ScenariosList.Add(new SelectListItem() { Text = "All", Value = "All" });
            ScenariosList.Add(new SelectListItem() { Text = "FullPallet", Value = "FullPallet" });
            ScenariosList.Add(new SelectListItem() { Text = "MixedPallet", Value = "MixedPallet" });
            ScenariosList.Add(new SelectListItem() { Text = "LooseCarton", Value = "LooseCarton" });
            ViewBag.ScenariosList = ScenariosList;

            List<SelectListItem> SalesOrderTypeList = new List<SelectListItem>();
            SalesOrderTypeList.Add(new SelectListItem() { Text = "All", Value = "All" });
            SalesOrderTypeList.Add(new SelectListItem() { Text = "Z4MO", Value = "Z4OM" });
            SalesOrderTypeList.Add(new SelectListItem() { Text = "Z4OR", Value = "Z4OR" });
            ViewBag.SalesOrderTypeList = SalesOrderTypeList;

            List<SelectListItem> SenderNameList = new List<SelectListItem>();
            SenderNameList.Add(new SelectListItem() { Text = "All", Value = "All" });
            SenderNameList.Add(new SelectListItem() { Text = "JDM1", Value = "JDM1" });
            SenderNameList.Add(new SelectListItem() { Text = "IQOR", Value = "IQOR" });
            ViewBag.SenderNameList = SenderNameList;

            List<SelectListItem> SearchList = new List<SelectListItem>();
            SearchList.Add(new SelectListItem() { Text = "All", Value = "All" });
            SearchList.Add(new SelectListItem() { Text = "LoadId", Value = "LoadId" });
            SearchList.Add(new SelectListItem() { Text = "SerailNumber", Value = "SerailNumber" });
            SearchList.Add(new SelectListItem() { Text = "CorrelationId", Value = "CorrelationId" });
            ViewBag.SearchList = SearchList;

        }

        [HttpPost]
        public async Task<PartialViewResult> SearchHomePage(Shipment obj)
        {
            var items = await CosmosDBRepository<Shipment>.GetItems();
            shipmentObj = items.ToList();
            if (obj != null)
            {
                if (!String.IsNullOrEmpty(obj.ServiceName) && obj.ServiceName != "All")
                {
                    shipmentObj = shipmentObj.Where(m => m.ServiceName == obj.ServiceName).ToList();
                }
                if (!String.IsNullOrEmpty(obj.Scenario[0]) && obj.Scenario[0] != "All")
                {
                    shipmentObj = shipmentObj.Where(m => m.Scenario.Contains(obj.Scenario[0])).ToList();
                }
                if (!String.IsNullOrEmpty(obj.SalesOrderType[0]) && obj.SalesOrderType[0] != "All")
                {
                    shipmentObj = shipmentObj.Where(m => m.SalesOrderType.Contains(obj.SalesOrderType[0])).ToList();
                }
                if (!String.IsNullOrEmpty(obj.SenderName) && obj.SenderName != "All")
                {
                    shipmentObj = shipmentObj.Where(m => m.SenderName == obj.SenderName).ToList();
                }

            }
            ViewBag.RecordCount = shipmentObj.Count();

            return PartialView("SearchResult", shipmentObj);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ActionName("SearchFromCosmosDB")]
        public ActionResult SearchFromCosmosDB()
        {
            //Search
            return View();
        }

        [ActionName("Search")]
        public ActionResult Search()
        {
            GetDropDowns();
            SearchViewModel searchViewObj = new SearchViewModel();
            searchViewObj.ShipmentResult = new List<Shipment>();
            return View(searchViewObj);
        }

        [HttpPost]
        [ActionName("PostToService")]
        public ActionResult PostToService(string correlationId, string environment)
        {
            //Search
            return View();
        }
    }
}