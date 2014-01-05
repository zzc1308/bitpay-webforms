using BitPayAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // replace with your api key
        string apiKey = "123ABC";

        var bitPay = new BitPay(apiKey);
        var request = new InvoiceRequest();
        var log = new Log();

        // Required POST fields:
        request.Price = 99.99m;
        request.Currency = "GBP";

        // Optional Payment Notification (IPN) fields:
        request.NotificationURL = string.Format("{0}://{1}/notifications/bitpay/", Request.Url.Scheme, Request.Url.Host);
        request.PosData = new Dictionary<string, string>() { { "OrderId", "123456789" } };
        request.TransactionSpeed = "low";
        request.FullNotifications = true;
        request.NotifcationEmail = "notifications@example.com";

        // Optional Order Handling fields:
        request.RedirectURL = string.Format("{0}://{1}/checkout/complete/", Request.Url.Scheme, Request.Url.Host);

        // Optional Buyer Information to display:
        request.OrderID = "123456789";
        request.ItemDesc = "Wrist Watch";
        request.ItemCode = "WW";
        request.Physical = true;
        request.BuyerName = "Joe Blogs";
        request.BuyerAddress1 = "Example Address 1";
        request.BuyerAddress2 = "Example Address 2";
        request.BuyerCity = "Example City";
        request.BuyerState = "Example State";
        request.BuyerZip = "Example ZIP";
        request.BuyerCountry = "Example Country";
        request.BuyerEmail = "buyer@example.com";
        request.BuyerPhone = "(+00) 00000 000000";

        // start test mode (this sends requests to the URL's specified below, comment out this whole block to connect to the live bitpay server)
        bool testMode = true;

        if (testMode)
        {
            bitPay.BaseURL = Request.Url.ToString().ToLower().Replace("/default.aspx", "");
            bitPay.CreateInvoiceURL = "/test/invoice.ashx";
            bitPay.GetInvoiceURL = "/test/invoice.ashx?id=";
        }
        // end test code
        
        try
        {
            var response = bitPay.CreateInvoice(request, log);

            if (response.Success)
            {
                // debug the response
                List<string> data = new List<string>();

                data.Add("<h2>Success</h2>");
                data.Add("id: " + response.Invoice.Id);
                data.Add("url: " + response.Invoice.Url);

                if (response.Invoice.PosData.Count > 0)
                    data.Add("posData: " + response.Invoice.PosData.Select(x => x.Key + " = " + x.Value).Aggregate((a, b) => a + ", " + b));

                data.Add("status: " + response.Invoice.Status.ToString());
                data.Add("price: " + response.Invoice.Price.ToString());
                data.Add("currency: " + response.Invoice.Currency);
                data.Add("btcPrice: " + response.Invoice.BTCPrice.ToString());
                data.Add("invoiceTime: " + response.Invoice.InvoiceTime.ToString());
                data.Add("expirationTime: " + response.Invoice.ExpirationTime.ToString());
                data.Add("currentTime: " + response.Invoice.CurrentTime.ToString());

                Dump(data);
            }
            else
            {
                // debug the response
                List<string> data = new List<string>() { "<h2>Error</h2>", "error type: " + response.ErrorType, "error message: " + response.ErrorMessage };

                Dump(data);
            }
        }
        catch (Exception ex)
        {
            // debug the log data and the exception message
            List<string> data = new List<string>()
            {
                "<h2>Exeption</h2>",
                "request URL: " + log.RequestUrl,
                "request Data: " + log.RequestData,
                "response Data: " + log.ResponseData,
                "exception: " + ex.Message
            };

            Dump(data);
        }
    }

    private void Dump(List<string> data)
    {
        litDump.Text = data.Select(x => string.Format("<p>{0}</p>", x)).Aggregate((a, b) => a + b);
    }
}