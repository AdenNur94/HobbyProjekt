using ReactShop.Core;
using ReactShop.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ReactShop.Web.Controllers
{
    public class NewProductController : Controller
    {
        readonly ICheckoutManager checkoutManager;

        public NewProductController()
        {
            this.checkoutManager = AutoFacHelper.Resolve<ICheckoutManager>();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Submit([FromBody]NewProductDTO newProduct)
        {
            checkoutManager.SaveCart(new CartItemDTO
            {
                SKU = Guid.NewGuid().ToString(),
                Description  = newProduct.Description,
                Price = newProduct.UnitPrice,
                Quantity = 1
            });
            return RedirectToAction("Cart", "Home");
        }
    }
}


