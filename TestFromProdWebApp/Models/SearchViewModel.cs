using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestFromProdWebApp.Models
{
    public class SearchViewModel
    {
        public Shipment Shipment { get; set; }
        public List<Shipment> ShipmentResult { get; set; }
    }
}