namespace AdobeConnectWebService.ApiViewModels
{
    public class AddMettingResultViewModel
    {
        public string MettingName { get; set; }

        public string SeverAddress { get; set; }

        public bool IsSucess { get; set; }

        public string ExMessage { get; set; }

        public string Permission { get; set; }
    }

    public class AddUserResultViewModel
    {
        public string UserName { get; set; }
        public string url { get; set; }


        public bool IsSucess { get; set; }

        public string ExMessage { get; set; }
    }
}
