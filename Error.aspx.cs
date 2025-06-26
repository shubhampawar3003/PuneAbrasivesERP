using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string errorMessage = Request.QueryString["msg"];
        string currentPageUrl = Request.QueryString["url"];

        if (!string.IsNullOrEmpty(errorMessage))
        {
            lblErrorMessage.Text = errorMessage + currentPageUrl;

        }
    }
}