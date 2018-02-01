using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TextSearchTesting
{
    public partial class testing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(TextBox1.Text, TextBox2.Text, RegexOptions.IgnoreCase) && (TextBox1.Text.Length == TextBox2.Text.Length))
                Label1.Text = "true";
            else
                Label1.Text = "false";
        }
    }
}