using ReactShop.Core;
using ReactShop.Core.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ReactShop.Web.WebApi
{
    public class CartController : ApiController
    {
        readonly ICheckoutManager checkoutManager;

        public CartController()
        {
            this.checkoutManager = AutoFacHelper.Resolve<ICheckoutManager>();
        }

        public CartController(ICheckoutManager checkoutManager)
        {
            this.checkoutManager = checkoutManager;
        }

        // POST: api/Cart
        [OutputCache(NoStore = true)]
        [ValidateHttpAntiForgeryToken]
        public CartDTO Post([FromBody]CartItemDTO value)
        {
            var cart = checkoutManager.GetCart();
            var cartItem = cart.CartItems.Where(i => i.SKU == value.SKU).SingleOrDefault();
            if (cartItem != null)
             {
                cartItem.Quantity = value.Quantity;
                var recalculatedCart = checkoutManager.GetCart(cart.CartItems);

                checkoutManager.SaveCart(cartItem);
                return recalculatedCart;
            }
            else
            {
                return cart;
            }
        }
    }
}
