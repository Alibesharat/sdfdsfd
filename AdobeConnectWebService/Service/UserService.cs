using AdobeConectApi.ReadDataViewModel;
using System.Collections.Generic;
using System.Data;
using WorkExcle;

namespace AdobeConectApi.Service
{
    public class UserService
    {
        ReadExcleFile f;


        public void Init(string FullPath)
        {
            f = new ReadExcleFile(FullPath);

        }

        public List<CSVUserViewModel> GetUsers(string SheetName)
        {
            List<CSVUserViewModel> ls = new List<CSVUserViewModel>();
            var data = f.GetAllCoulumnsFromSheet(SheetName);
            foreach (DataRow item in data.Rows)
            {
                CSVUserViewModel c = new CSVUserViewModel()
                {
                    FirstName = item.ItemArray[0].ToString(),
                    LastName = item.ItemArray[1].ToString(),
                    Login = string.IsNullOrWhiteSpace(item.ItemArray[2].ToString()) ? item.ItemArray[0].ToString() : item.ItemArray[2].ToString(),
                    Password = item.ItemArray[3].ToString(),
                    Email = item.ItemArray[4].ToString(),
                };
                ls.Add(c);
            }
            return ls;
        }

        public List<CSVUserViewModel> GetUpdatedUsers(string SheetName)
        {
            List<CSVUserViewModel> ls = new List<CSVUserViewModel>();
            var data = f.GetAllCoulumnsFromSheet(SheetName);
            foreach (DataRow item in data.Rows)
            {
                string pass = item.ItemArray[2].ToString();
                if(!string.IsNullOrWhiteSpace(pass) && int.TryParse(pass,out int _))
                {
                    CSVUserViewModel c = new CSVUserViewModel()
                    {
                        FirstName = item.ItemArray[0].ToString(),
                        LastName = item.ItemArray[1].ToString(),
                        Login = item.ItemArray[3].ToString(),
                        Password = pass,
                        NewCode = item.ItemArray[6].ToString(),
                        timing = $"{item.ItemArray[4]}-{item.ItemArray[5]}"
                    };
                    ls.Add(c);
                }
               
            }
            return ls;
        }

        public List<LandingViewModel> GetLanding(string ServerAddress, string SheetName)
        {
            List<LandingViewModel> ls = new List<LandingViewModel>();
            var data = f.GetAllCoulumnsFromSheet(SheetName);
            foreach (DataRow item in data.Rows)
            {
                LandingViewModel c = new LandingViewModel()
                {

                    Id = string.IsNullOrWhiteSpace(item.ItemArray[2].ToString()) ? item.ItemArray[0].ToString() : item.ItemArray[2].ToString(),
                    Url = $@"{ServerAddress}/c{SheetName}"
                };
                ls.Add(c);
            }
            return ls;
        }

        public List<LandingViewModel> GetTiming(string SheetName)
        {
            List<LandingViewModel> ls = new List<LandingViewModel>();
            var data = f.GetAllCoulumnsFromSheet(SheetName);
            foreach (DataRow item in data.Rows)
            {
                LandingViewModel c = new LandingViewModel()
                {

                    Id = $"{ item.ItemArray[0]}-{item.ItemArray[4]}",
                    Url = $"{item.ItemArray[1]}-{item.ItemArray[2]}"
                };
                ls.Add(c);
            }
            return ls;
        }


    }



}
