using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSpeedTest
{
    class Config
    {
        public string URL { get; set; }
        public int TimeOut { get; set; }
        public string CSVLocation { get; set; }
        public string LogLocation { get; set; }
        public string CertificationAttributes { get; set; }
        public string DownloadSpeedAttributes { get; set; }
        public string DownloadUnitsAttributes { get; set; }
        public string ShowMoreInfoAttributes { get; set; }
        public string UploadSpeedAttributes { get; set; }
        public string UploadUnitsAttributes { get; set; }
    }
}
