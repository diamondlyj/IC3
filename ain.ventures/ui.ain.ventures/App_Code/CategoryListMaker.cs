using AIn.Ventures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AIn.Ventures.UI.App_Code
{
    public class CategoryListMaker
    {
        public static HtmlString MakeList(List<Category> mainCat)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("length = " + mainCat.Count());
            List<Category> cat = new List<Category>();
            sb.Append("<ul class = 'CategoryList' style='display:none'>");
            foreach (var c in mainCat)
            {
                string idValue = sanitizeString(c.Name);
                sb.AppendLine(String.Concat("<li class = 'CategoryListChildren' id ='", idValue, "' " , ""));
                if (c.Categories.Count() > 0)
                {
                    sb.AppendLine(String.Concat("<a data-action = 'revealChildren' data-params = '", idValue, "'>"));
                    sb.AppendLine("+ ");
                    sb.AppendLine("</a>");
                }
                sb.AppendLine(String.Concat("<a data-action= 'addCategory', data-params ='",idValue,"' class ='CategoryName'>"));
                sb.AppendLine(c.Name);
                sb.AppendLine("</a>");
                if (c.Categories.Count > 0)
                {
                    if (c.Categories.Count() > 0)
                    {
                        sb.Append(MakeList(c.Categories));
                    }
                }
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");
            return new HtmlString(sb.ToString());
        }
        public static string sanitizeString(string s)
        {
            s = s.Replace(' ', '_');
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (!(Char.IsLetter(c) || char.IsNumber(c) || c.Equals('_')))
                {
                    s = s.Remove(i, 1);
                }
            }
            return s;
        }
    }
}