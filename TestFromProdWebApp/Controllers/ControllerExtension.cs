using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestFromProdWebApp.Models;

namespace TestFromProdWebApp.Controllers
{
    public static class ControllerExtensions
    {
        public static ToastMessage AddToastMessage(this HomeController controller, string title, string message, ToastType toastType = ToastType.Info)
        {
            Toastr toastr = controller.TempData["Toastr"] as Toastr;

            toastr = toastr ?? new Toastr();

            var toastMessage = toastr.AddToastMessage(title, message, toastType);

            controller.TempData["Toastr"] = toastr;

            return toastMessage;
        }
    }
}
