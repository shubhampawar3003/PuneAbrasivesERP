using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class testcount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string abc = TextBox1.Text;
        int i = 0;
        foreach (var item in abc)
        {
            i++;
        }
        Response.Write(i);
    }
}