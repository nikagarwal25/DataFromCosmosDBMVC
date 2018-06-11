using Microsoft.SupplyChain.Shipment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestFromProdWebApp.Models;
using TestFromProdWebApp.Repository;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json.Linq;

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
            SalesOrderTypeList.Add(new SelectListItem() { Text = "Z4OM", Value = "Z4OM" });
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
            SearchList.Add(new SelectListItem() { Text = "SerialNumber", Value = "SerailNumber" });
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
        public async Task<JsonResult> PostToService(string correlationId, string environment, string serviceName)
        {
            PostReponse responseObj = new PostReponse();
            try
            {
                //Search
                if (String.Compare(environment, "SIT") == 0)
                {
                    //Prod blob storage
                    string payload = string.Empty;
                    string response = string.Empty;
                    string prodstorageAccountName = "";
                    string prodstorageAccountKey = "";
                    string prodcontainerName = "";
                    string prodConnectionString = string.Format(@"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                                                 prodstorageAccountName, prodstorageAccountKey);
                    var cloudStorageAccount = CloudStorageAccount.Parse(prodConnectionString);
                    var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(prodcontainerName);

                    CloudBlockBlob blobSource = cloudBlobContainer.GetBlockBlobReference(String.Format("{0}.json", correlationId));
                    if (blobSource.Exists())
                    {
                        payload = blobSource.DownloadText();
                    }
                    if (String.Compare(serviceName, "CMShipment") == 0)
                    {
                        response = await PostCMShipmentSIT(correlationId, payload);
                        responseObj.Status = true;
                        responseObj.ResponseMessage = response;
                    }
                    else
                    {
                        response = await PostRCShipmentSIT(correlationId, payload);
                        responseObj.Status = true;
                        responseObj.ResponseMessage = response;
                        this.AddToastMessage("Success!", $"{correlationId} has been successfully posted to {environment}", ToastType.Success);
                    }
                    //SIT Blob storage
                }
            }
            catch (RestApiSubmitterException ex)
            {
                responseObj.ResponseMessage = ex.Message;
                responseObj.Status = false;
                var errorMessage = JObject.Parse(ex.Message)["errors"][0]["message"].ToString();
                this.AddToastMessage("Error!", $"{correlationId}: {errorMessage}", ToastType.Error);
            }
            catch (Exception ex)
            {
                responseObj.Status = false;
                responseObj.ResponseMessage = ex.Message;
                this.AddToastMessage("Error!", $"{correlationId} not posted to {environment}: {ex.Message}", ToastType.Error);
            }
            return Json(responseObj, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> PostCMShipmentSIT(string correlationId, string payload)
        {
            try
            {
                string authority = "https://login.microsoftonline.com/scsdirectorydev.onmicrosoft.com";
                string clientId = "";
                string appKey = "";
                string resourceId = "https://supplychainservices.microsoft.com/";
                string apimSubscriptionKey = "something";
                int delayTimeInSecondBetweenRetrys = 1;
                int maxRetries = 1;
                string baseUrl = @"";
                string relativeUrl = "?CorrelationId={0}";
                string apiName = "something";
                RestApiSubmitter restapiSubmitter = new RestApiSubmitter(authority, clientId, appKey, resourceId, apimSubscriptionKey, delayTimeInSecondBetweenRetrys, maxRetries, baseUrl, relativeUrl, apiName);
                return await restapiSubmitter.Submit(correlationId, payload);
            }
            catch (RestApiSubmitterException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> PostRCShipmentSIT(string correlationId, string payload)
        {
            try
            {
                string authority = "https://login.microsoftonline.com/scsdirectorydev.onmicrosoft.com";
                string clientId = "";
                string appKey = "";
                string resourceId = "https://supplychainservices.microsoft.com/";
                string apimSubscriptionKey = "something";
                int delayTimeInSecondBetweenRetrys = 1;
                int maxRetries = 1;
                string baseUrl = @"https://";
                string relativeUrl = "?CorrelationId={0}";
                string apiName = "something";
                RestApiSubmitter restapiSubmitter = new RestApiSubmitter(authority, clientId, appKey, resourceId, apimSubscriptionKey, delayTimeInSecondBetweenRetrys, maxRetries, baseUrl, relativeUrl, apiName);
                return await restapiSubmitter.Submit(correlationId, payload);
            }
            catch (RestApiSubmitterException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}