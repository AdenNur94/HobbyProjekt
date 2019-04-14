using ReactShop.Core.DTOs;
using System.Collections.Generic;

namespace ReactShop.Core
{
    public interface ICheckoutManager
    {
        CartDTO GetCart();
        void SaveCart(CartItemDTO modifiedItem);
        CartDTO GetCart(List<CartItemDTO> cartItems);
        CheckoutSummaryDTO GetCheckoutSummary();
        List<ProductDTO> GetProducts();
        Context InitializeDB();
    }
}