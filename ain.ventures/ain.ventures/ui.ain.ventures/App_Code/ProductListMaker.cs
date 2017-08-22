using AIn.Ventures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Windows.Forms;


namespace AIn.Ventures.UI.App_Code
{
    public class ProductListMaker
    {
        public static HtmlString MakeList(Component c){
            var returnStr = "";
            returnStr += "<ul class = 'ProductList'>";
            returnStr += "<li class = 'ProductList'>";
            returnStr += c.ComponentType + ": " + c.Name;
            if (c.Children.Count > 0)
            {
                foreach(Component child in c.Children)
                {
                    returnStr += MakeList(child);
                }
            }
            returnStr += "</li>";
            returnStr += "</ul>";
            return new HtmlString(returnStr);
        }
    }

    
}