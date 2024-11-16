using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Admin.Helpers
{
    public class StringHelper
    {
        public static String GetFirstParagraph(String Content)
        {
            String paragraph;
            Match m = Regex.Match(Content, @"<p.*>\s*(.+?)\s*</p>");
            if (m.Success)
            {
                paragraph = m.Groups[1].Value;
                return Regex.Replace(paragraph, @"<img.*src=.*data-filename=.*>",String.Empty);
            }
            else
            {
                return "";
            }
        }
    }
}
