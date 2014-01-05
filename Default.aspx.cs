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
        bool testMode = true;

        string apiKey = "123ABC";

        var bitPay = new BitPay(apiKey);

        if (testMode)
        {
            bitPay.BaseURL = string.Format("{0}://{1}:{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port);
            bitPay.CreateInvoiceURL = "/test/invoice.ashx";
            bitPay.GetInvoiceURL = "/test/invoice.ashx?id=";
        }

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
        
        try
        {
            var response = bitPay.CreateInvoice(request, log);

            if (response.Success)
            {
                // DEBUG THE RESPONSE:
                List<string> data = new List<string>();

                data.Add("id: " + response.Invoice.Id);
                data.Add("url: " + response.Invoice.Url);
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
                // DEBUG THE RESPONSE
                List<string> data = new List<string>() { "error type: " + response.ErrorType, "error message: " + response.ErrorMessage };

                Dump(data);
            }
        }
        catch (Exception ex)
        {
            // DEBUG THE LOG DATA AND EXCEPTION MESSAGE
            List<string> logData = new List<string>()
            {
                "request URL: " + log.RequestUrl,
                "request Data: " + log.RequestData,
                "response Data: " + log.ResponseData,
                "exception: " + ex.Message
            };

            logData.ForEach(x => Response.Write(x));
        }
    }

    private void Dump(List<string> data)
    {
        litDump.Text = data.Select(x => string.Format("<p>{0}</p>", x)).Aggregate((a, b) => a + b);
    }
}