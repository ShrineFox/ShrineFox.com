﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShrineFoxCom
{
    public partial class Wiki : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("https://amicitia.miraheze.org/wiki/Main_Page");
        }
    }
}