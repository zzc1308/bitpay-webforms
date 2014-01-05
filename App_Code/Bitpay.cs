using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BitPayAPI
{
    public class BitPay
    {
        public string ApiKey { get; set; }
        public string BaseURL { get; set; }
        public string CreateInvoiceURL { get; set; }
        public string GetInvoiceURL { get; set; }

        public BitPay(string apiKey)
        {
            ApiKey = apiKey;
            BaseURL = "https://bitpay.com/api";
            CreateInvoiceURL = "/invoice";
            GetInvoiceURL = "/invoice/";

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public InvoiceResponse CreateInvoice(InvoiceRequest request, Log log)
        {
            if (request.Price <= 0)
                throw new Exception("Price must be greater than zero.");

            if (String.IsNullOrEmpty(request.Currency))
                throw new Exception("Currency must be specified.");

            log.RequestUrl = BaseURL + CreateInvoiceURL;
            log.RequestData = JsonConvert.SerializeObject(request);
            log.ResponseData = HttpPost(log.RequestUrl, log.RequestData);

            JObject obj = JObject.Parse(log.ResponseData);

            var errorObj = obj["error"];

            if (errorObj != null)
                return new InvoiceResponse() { ErrorType = errorObj["type"].ToString(), ErrorMessage = errorObj["message"].ToString() };

            return new InvoiceResponse() { Invoice = obj.ToObject<Invoice>() };
        }

        public InvoiceResponse GetInvoice(string invoiceId, Log log)
        {
            log.RequestUrl = BaseURL + GetInvoiceURL + invoiceId;
            log.ResponseData = HttpGet(log.RequestUrl);

            JObject obj = JObject.Parse(log.ResponseData);

            var errorObj = obj["error"];

            if (errorObj != null)
                return new InvoiceResponse() { ErrorType = errorObj["type"].ToString(), ErrorMessage = errorObj["message"].ToString() };

            return new InvoiceResponse() { Invoice = obj.ToObject<Invoice>() };
        }

        public static Invoice GetInvoiceFromJSON(string json)
        {
            return JsonConvert.DeserializeObject<Invoice>(json);
        }

        private string HttpPost(string url, string data)
        {
            using (WebClient wc = GetWebClient())
            {
                wc.Headers.Add("Content-Type", "application/json");

                return wc.UploadString(url, data);
            }
        }

        private string HttpGet(string url)
        {
            using (WebClient wc = GetWebClient())
            {
                return wc.DownloadString(url);
            }
        }

        private WebClient GetWebClient(string contentType = null)
        {
            WebClient wc = new WebClient();

            wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(ApiKey + ":"));

            return wc;
        }
    }

    public class InvoiceRequest
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("postData")]
        public Dictionary<string, string> PosData { get; set; }

        [JsonProperty("notificationURL")]
        public string NotificationURL { get; set; }

        [JsonProperty("transactionSpeed")]
        public string TransactionSpeed { get; set; }

        [JsonProperty("fullNotifications")]
        public bool? FullNotifications { get; set; }

        [JsonProperty("notificationEmail")]
        public string NotifcationEmail { get; set; }

        [JsonProperty("redirectURL")]
        public string RedirectURL { get; set; }

        [JsonProperty("orderID")]
        public string OrderID { get; set; }

        [JsonProperty("itemDesc")]
        public string ItemDesc { get; set; }

        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        [JsonProperty("physical")]
        public bool? Physical { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("buyerAddress1")]
        public string BuyerAddress1 { get; set; }

        [JsonProperty("buyerAddress2")]
        public string BuyerAddress2 { get; set; }

        [JsonProperty("buyerCity")]
        public string BuyerCity { get; set; }

        [JsonProperty("buyerState")]
        public string BuyerState { get; set; }

        [JsonProperty("buyerZip")]
        public string BuyerZip { get; set; }

        [JsonProperty("buyerCountry")]
        public string BuyerCountry { get; set; }

        [JsonProperty("buyerEmail")]
        public string BuyerEmail { get; set; }

        [JsonProperty("buyerPhone")]
        public string BuyerPhone { get; set; }

        public InvoiceRequest()
        {
        }
    }

    public class InvoiceResponse
    {
        public bool Success { get { return Invoice != null; } }
        public Invoice Invoice { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public InvoiceResponse()
        {
        }
    }

    public class Invoice
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("posData")]
        public Dictionary<string, string> PosData { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public InvoiceStatus Status { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("btcPrice")]
        public decimal BTCPrice { get; set; }

        [JsonProperty("invoiceTime")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime InvoiceTime { get; set; }

        [JsonProperty("expirationTime")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime ExpirationTime { get; set; }

        [JsonProperty("currentTime")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CurrentTime { get; set; }

        public Invoice()
        {
        }
    }

    public enum InvoiceStatus
    {
        New,
        Paid,
        Confirmed,
        Complete,
        Expired,
        Invalid
    }

    public class Log
    {
        public string RequestUrl { get; internal set; }
        public string RequestData { get; internal set; }
        public string ResponseData { get; internal set; }

        public Log()
        {
        }
    }

    public class JsonDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(reader.Value));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //throw new NotImplementedException();
        }
    }
}