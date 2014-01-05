<%@ WebHandler Language="C#" Class="InvoiceStatus" %>

using BitPayAPI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;

public class InvoiceStatus : IHttpHandler
{
    public void ProcessRequest (HttpContext context)
    {
        string json = "";

        using (StreamReader readStream = new StreamReader(context.Request.InputStream, Encoding.UTF8))
        {
            json = readStream.ReadToEnd();
        }

        var invoice = BitPay.GetInvoiceFromJSON(json);
        
        // update database with invoice status etc
        
        context.Response.ContentType = "text/plain";
        context.Response.Write("OK");
    }

    public bool IsReusable { get { return false; } }
}