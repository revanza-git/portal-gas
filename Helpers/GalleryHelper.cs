using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Helpers
{
    public class GalleryHelper
    {
        static public String GetContentType(String FileName)
        {
            String ContentType = "video/" + FileName.Substring(FileName.IndexOf('.') + 1);
            return ContentType;
        }
    }
}
