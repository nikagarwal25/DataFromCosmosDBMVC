using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestFromProdWebApp.Models
{
    public class Shipment
    {
        public string ServiceName { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string CorrelationId { get; set; }
        public string ProcessType { get; set; }
        public int PurchasOrderCount { get; set; }
        public int SalesOrderCount { get; set; }
        public int DeliveryOrderCount { get; set; }
        public int PurchasOrderLineCount { get; set; }
        public int SalesOrderLineCount { get; set; }
        public int DeliveryOrderLineCount { get; set; }
        public string LoadId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int TotalPalletCount { get; set; }
        public int TotalCartonCount { get; set; }
        public int ItemCount { get; set; }
        public List<string> Scenario { get; set; }
        public List<string> SerialNumber { get; set; }
    }

    public class Pallet
    {
        public string PalletSkuMix { get; set; }
        public string PalletLoadForm { get; set; }
        public List<Carton> Carton { get; set; }
    }

    public class Carton
    {
        public string CartonSkuMix { get; set; }
        public string CartonLoadForm { get; set; }
        public bool IsProductSerailized { get; set; }
        public string ProductId { get; set; }
    }
}
