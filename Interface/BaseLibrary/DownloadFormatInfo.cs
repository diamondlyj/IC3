using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{
    //bla bla 
    public enum AcceptedFormats
    {
        PDF, XLSX, CSV
    }

    public class DownloadFormatInfo
    {
        public string ContentType { get; }
        public string Extension { get; }
        public static Dictionary<AcceptedFormats, DownloadFormatInfo> Formats = new Dictionary<AcceptedFormats, DownloadFormatInfo> {
            { AcceptedFormats.CSV, new AI.V2.BaseLibrary.DownloadFormatInfo("text/csv", ".csv") },
            { AcceptedFormats.XLSX, new AI.V2.BaseLibrary.DownloadFormatInfo("applications/ms-excel", ".xlsx") },
            {AcceptedFormats.PDF, new AI.V2.BaseLibrary.DownloadFormatInfo("application/pdf", ".pdf") }
        };
        //blah blah

        public DownloadFormatInfo(string ContentType, string Extension)
        {
            this.ContentType = ContentType;
            this.Extension = Extension;

        }

    }
}
