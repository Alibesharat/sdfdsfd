using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdobeConnectWebService.ApiViewModels
{
    public class AddGroupResultViewModel
    {
        public string GroupName { get; set; }

        public string SeverAddress { get; set; }

        public bool IsSucess { get; set; }

        public string ExMessage { get; set; }
    }

    public class AddUserResultViewModel
    {
        public string UserName { get; set; }
        public string GroupName { get; set; }

        public string SeverAddress { get; set; }

        public bool IsSucess { get; set; }

        public string ExMessage { get; set; }
    }
}
