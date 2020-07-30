using System.Collections.Generic;

namespace AdobeConnectWebService.ApiViewModels
{
    public class UserDataViewModel
    {

        public List<UserInfoViewModel> userInfo { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsTeacher { get; set; }

        public string GroupCode { get; set; }

        public string Date { get; set; }

    }
}
