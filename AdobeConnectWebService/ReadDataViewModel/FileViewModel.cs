using System.Collections.Generic;

namespace AdobeConectApi.ReadDataViewModel
{
    public class FileViewModel
    {
        public FileViewModel()
        {
            Mettings = new List<string>();
        }
        public string ServerName { get; set; }

       public List<string> Mettings { get; set; }

    }
}
