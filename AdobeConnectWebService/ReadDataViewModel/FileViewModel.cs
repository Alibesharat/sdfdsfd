using System.Collections.Generic;

namespace AdobeConectApi.ReadDataViewModel
{
    public class FileViewModel
    {
        public FileViewModel()
        {
            Files = new Dictionary<string, string>();
        }
        public string ServerName { get; set; }

       public Dictionary<string, string> Files { get; set; }

    }
}
