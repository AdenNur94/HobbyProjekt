using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ReactShop.Web.ReactConfig), "Configure")]

namespace ReactShop.Web
{
    public static class ReactConfig
    {
        public static void Configure()
        {
            ReactSiteConfiguration.Configuration
                .AddScript("~/Scripts/showdown.js")
                .AddScript("~/Scripts/react-bootstrap.js")
                .AddScript("~/Scripts/Components.jsx")
                .AddScript("~/Scripts/Cart.jsx")
                .AddScript("~/Scripts/CheckoutSuccess.jsx");
        }
    }
}