<%@ WebHandler Language="C#" Class="invoice" %>

using System;
using System.Web;

public class invoice : IHttpHandler
{   
    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "application/javascript";
        context.Response.Write(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("invoice.json")));
    }
 
    public bool IsReusable { get { return false; } }
}