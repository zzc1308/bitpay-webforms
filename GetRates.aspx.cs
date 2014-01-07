using BitPayAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GetRates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // replace with your api key
        string apiKey = "123ABC";

        var bitPay = new BitPay(apiKey);
        var log = new Log();

        var rates = bitPay.GetRates(log);


        // debug
        GridView gv = new GridView() { DataSource = rates };
        gv.DataBind();
        form1.Controls.Add(gv);
        
    }
}