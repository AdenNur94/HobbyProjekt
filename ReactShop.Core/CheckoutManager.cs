using ReactShop.Core.DTOs;
using ReactShop.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace ReactShop.Core
{
    public class CheckoutManager : ICheckoutManager
    {
        private string serverFilePath;

        public CheckoutManager()
        {
        }


        public CheckoutManager(string serverFilePath)
        {
            this.serverFilePath = serverFilePath;
        }

        public CheckoutSummaryDTO GetCheckoutSummary()
        {
            var cartItems = GetCartItems();
            var subtotal = cartItems.Sum(i => i.Subtotal);
            var discountRule = DiscountManager.Instance.GetDiscount(subtotal);
            var discountValue = discountRule.CalculatedDiscount;
            var total = subtotal - discountValue;

            return new CheckoutSummaryDTO
            {
                OrderNumber = "123456789",
                DeliveryUpToNWorkingDays = 4,
                Total = total,
                CustomerInfo = GetDummyCustomerInfo(),
                CartItems = cartItems
            };
        }

        public CartDTO GetCart()
        {
            return GetCart(GetCartItems());
        }

        public CartDTO GetCart(List<CartItemDTO> cartItems)
        {
            var subtotal = cartItems.Sum(i => i.Subtotal);
            var discountRule = DiscountManager.Instance.GetDiscount(subtotal);
            var discountValue = discountRule.CalculatedDiscount;
            var total = subtotal - discountValue;

            return new CartDTO
            {
                Subtotal = subtotal,
                DiscountRate = discountRule.Rate * 100M,
                DiscountValue = discountValue,
                Total = total,
                CartItems = cartItems
            };
        }


        public void SaveCart(CartItemDTO newOrEditItem)
        {
            try
            {
                if (newOrEditItem.Quantity < 0)
                    newOrEditItem.Quantity = 0;

                using (var db = new Context())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var product = db.Product.Where(p => p.SKU == newOrEditItem.SKU).Single();

                        var cartItem =
                            (from ci in db.CartItem
                             join p in db.Product on ci.ProductId equals p.Id
                             where p.SKU == newOrEditItem.SKU
                             select ci)
                            .SingleOrDefault();

                        if (cartItem != null)
                        {
                            if (newOrEditItem.Quantity == 0)
                                db.CartItem.Remove(cartItem);
                            else
                            {
                                cartItem.Quantity = newOrEditItem.Quantity;
                                cartItem.Product = product;
                            }
                        }
                        else
                        {
                            db.CartItem.Add(new CartItem
                            {
                                Product = product,
                                Quantity = newOrEditItem.Quantity
                            });
                        }

                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
        }

        public List<ProductDTO> GetProducts()
        {
            using (var db = new Context())
            {
                return db.Product
                    .Select(i =>
                    new ProductDTO
                    {
                        Id = i.Id,
                        SKU = i.SKU,
                        SmallImagePath = i.SmallImagePath,
                        Description = i.Description,
                        Price = i.Price
                    }).ToList();
            }
        }

        private List<CartItemDTO> GetCartItems()
        {
            using (var db = new Context())
            {
                return 
                    (from ci in db.CartItem
                    from p in db.Product.Where(p => p.Id == ci.ProductId)
                    select new { ci, p })
                    .Select(i =>
                    new CartItemDTO
                    {
                        Id =  i.ci.Id,
                        SKU = i.p.SKU,
                        SmallImagePath = i.p.SmallImagePath,
                        Description = i.p.Description,
                        Price = i.p.Price,
                        Quantity = i.ci.Quantity
                    }).ToList();
            }
        }

        private CustomerInfoDTO GetDummyCustomerInfo()
        {
            return new CustomerInfoDTO
            {
                CustomerName = "Namn Efternamn",
                PhoneNumber = "(46) 73-2033095",
                Email = "namn@email.com",
                DeliveryAddress = "Nygatan 1"
            };
        }

        public Context InitializeDB()
        {
            var db = new Context();

            if (!db.Database.Exists())
            {
                db.Database.CreateIfNotExists();

                var products = new string[] 
                { 
                    "Shirt|3399",
                    "Hoodie|3399",
                    "Jeans|1399",
                    "Belt|1099",
                    "T-shirt|1099",
                    "Drinking Glass|1099",
                    "Jacket|4399",
                    "Shoes|1099",
                    "Socks|299",
                };

                var index = 1;
                foreach (var p in products)
                {
                    var description = p.Split('|')[0];
                    var price = decimal.Parse(p.Split('|')[1]) / 100M;

                    var product = 
                    db.Product.Add(new Product
                    {
                        SKU = Guid.NewGuid().ToString(),
                        SmallImagePath = string.Format("Images/Products/small_{0}.jpg", index),
                        LargeImagePath = string.Format("Images/Products/large_{0}.jpg", index),
                        Description = description,
                        Price = price
                    });

                    var cartItem =
                    db.CartItem.Add(new CartItem
                    {
                        Product = product,
                        Quantity = 1
                    });

                    index++;
                }

                db.SaveChanges();

            }
            return db;
        }
    }
}
