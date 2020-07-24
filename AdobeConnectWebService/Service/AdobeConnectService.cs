using AdobeConectApi.ReadDataViewModel;
using AdobeConnectWebService.ApiViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using ViewModels;

namespace AdobeConectApi.Service
{
    public class AdobeConnectService
    {
        HttpClient Client;
        readonly string _Domin;
        readonly string _userName;
        readonly string _Password;
        const string LoginUrl = "login";
        const string PrincipallistUrl = "principal-list";
        const string ShorcutUrl = "sco-shortcuts";
        const string AddMeetingUrl = "sco-update";
        const string UpdatePermissionUrl = "permissions-update";
        const string UpdatePrincipalUrl = "principal-update";
        const string AddUserToGroupUrl = "group-membership-update";

        public AdobeConnectService(string Domin, string UserName, string Password)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            Client = new HttpClient(clientHandler);

            _Domin = Domin;
        }

        public List<AddUserResultViewModel> AddUserDataToGroups(UserDataViewModel userData)
        {
            List<AddUserResultViewModel> ls = new List<AddUserResultViewModel>();

            try
            {
                object obj = new object();
                var data = FindMettingOnServers(userData.GroupCode);
                if (!string.IsNullOrWhiteSpace(data.Item1))
                {

                    foreach (var item in userData.userInfo)
                    {
                        lock (obj)
                        {
                            var res = AddUser(data.Item1, item.Name, item.UserName, item.Password);

                            if (res.Status?.Code == "Ok")
                            {
                                lock (obj)
                                {
                                    var result = AddUserToGroupASync(data.Item2, res.Principal.Principalid);
                                    if (res.Status.Code == "Ok")
                                    {
                                        ls.Add(new AddUserResultViewModel() { UserName = item.UserName, SeverAddress = data.Item1, GroupName = data.Item2, IsSucess = true });
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    ls.Add(new AddUserResultViewModel() { UserName = "", SeverAddress = "", GroupName = userData.GroupCode, IsSucess = false, ExMessage = "گروه یافت نشد" });

                }
            }
            catch (Exception ex)
            {

                ls.Add(new AddUserResultViewModel() { UserName = "", SeverAddress = "", GroupName = userData.GroupCode, IsSucess = false, ExMessage = ex.Message });
            }

            return ls;
        }

        private (string, string) FindMettingOnServers(string Id)
        {
            object obj = new object();
            List<string> servers = new List<string>();
            for (int i = 1; i < 51; i++)
            {
                servers.Add($"ac{i}");
            }
            foreach (var server in servers)
            {
                string Address = $@"{server}.{_Domin}";
                Console.WriteLine($"== Search in  {Address}");
                if (Login(Address))
                {
                    lock (obj)
                    {

                        var gr = FindUser(Address, "login", Id);
                        if (gr != null && gr.Status.Code == "ok")
                        {
                            if (gr.Principallist.Principal.Count() > 0)
                            {
                                var id = gr.Principallist.Principal.FirstOrDefault().Principalid;
                                return (Address, id);
                            }



                        }
                        return ("", "");
                    }
                }
                else
                {
                    throw new Exception($"Can not Login With UserName {_userName} And Password {_Password} on Server {Address}");
                }
            }
            return "";
        }

        public List<AddGroupResultViewModel> AddGroupsToServers(List<FileViewModel> data)
        {
            object obj = new object();
            List<AddGroupResultViewModel> vm = new List<AddGroupResultViewModel>();
            foreach (var item in data)
            {
                lock (obj)
                {
                    string ServerUrl = $"http://{item.ServerName}.{_Domin}/api/xml?action=";
                    if (Login(ServerUrl))
                    {
                        lock (obj)
                        {
                            foreach (var file in item.Files)
                            {
                                try
                                {
                                    var res = AddGroup(ServerUrl, file.Key, $"Gr{file.Key}");
                                    if (res.Status?.Code == "Ok")
                                    {
                                        vm.Add(new AddGroupResultViewModel() { GroupName = file.Key, SeverAddress = ServerUrl, IsSucess = true });
                                    }
                                    else
                                    {
                                        vm.Add(new AddGroupResultViewModel() { GroupName = file.Key, SeverAddress = ServerUrl, IsSucess = false, ExMessage = res.Status.Code });

                                    }
                                }
                                catch (Exception ex)
                                {

                                    vm.Add(new AddGroupResultViewModel() { GroupName = file.Key, SeverAddress = ServerUrl, IsSucess = false, ExMessage = ex.Message });
                                }
                            }
                        }
                    }
                }

            }
            return vm;
        }

        private bool Login(string ServerAddress)
        {
            string parameter = $"&login={_userName}&password={_Password}";
            string url = $"{ServerAddress}{LoginUrl}{parameter}";
            var data = Client.GetFromXmlAsync<TokenViewModel>(url);
            if (data.Status.Code == "ok")
            {
                //Client.DefaultRequestHeaders.Add("Token", data.OWASP_CSRFTOKEN.Token);
                //string cookie = "BREEZESESSION=breez3mpwg7bimtm68z7r; BreezeCCookie=conn-E0Y4-VSKE-357P-XDIU-7CFU-619U-61LO-65MS; BreezeLoginCookie=connect@ili.ir";
                //Client.DefaultRequestHeaders.Add("cookie", cookie);
                Console.WriteLine($"Login Sucees : user {_userName}");
                return true;

            }
            Console.WriteLine($"Login faild : user {_userName}");
            return false;


        }



        private object GetPrincipallist()
        {
            string url = $"{BaseUrl}{PrincipallistUrl}";
            var data = Client.GetFromXmlAsync<object>(url);
            return data;
        }


        private ShortCutViewModel GetShortCut()
        {
            string url = $"{BaseUrl}{ShorcutUrl}";
            var data = Client.GetFromXmlAsync<ShortCutViewModel>(url);
            return data;
        }


        private string GetFolderId()
        {
            try
            {
                string url = $"{BaseUrl}{ShorcutUrl}";
                var data = Client.GetFromXmlAsync<ShortCutViewModel>(url);
                string FolderId = data.Shortcuts.Folders.FirstOrDefault(c => c.Type == "meetings").Scoid;
                return FolderId;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        public MeetingViewModel AddMeetingAddParticapnts(string Name, string AddressPath, string GroupId)
        {
            string FolderId = GetFolderId();
            string StartDate = "2006-10-01T09:00";
            string EndDate = "2006-10-01T17:00";
            string Descripton = "this a new meting";
            string parameters = $"&folder-id={FolderId}" +
                $"&description={Descripton}&name={Name}" +
                $"&type=meeting&lang=en&date-begin={StartDate}&date-end={EndDate}" +
                $"&url-path={AddressPath}";
            string url = $"{BaseUrl}{AddMeetingUrl}{parameters}";
            var data = Client.GetFromXmlAsync<MeetingViewModel>(url);
            if (data.Status.Code == "ok")
            {
                var res = SetPermissionToMeeting(data.Sco.Scoid, "public-access", "denied");
                Console.WriteLine($"Set Permisson for {Name} : {res.Status.Code} - remove ");
                res = SetPermissionToMeeting(data.Sco.Scoid, GroupId, "view");
                Console.WriteLine($"Set Permisson for {Name} : {res.Status.Code} - view ");


            }
            return data;
        }


        public stateViewModel AddHostToMeeting(MeetingViewModel data, string GroupId)
        {

            var e = SetPermissionToMeeting(data.Sco.Scoid, GroupId, "view");
            Console.WriteLine($"Set Permisson for {GroupId} : {e.Status.Code} - view ");

            var res = SetPermissionToMeeting(data.Sco.Scoid, GroupId, "host");
            Console.WriteLine($"Set Permisson for {GroupId} : {res.Status.Code} - host ");

            return res;
        }

        public stateViewModel AddHostToMeeting(string mettingId, string GroupId)
        {

            var e = SetPermissionToMeeting(mettingId, GroupId, "view");
            Console.WriteLine($"Set Permisson for {GroupId} : {e.Status.Code} - view ");

            var res = SetPermissionToMeeting(mettingId, GroupId, "host");
            Console.WriteLine($"Set Permisson for {GroupId} : {res.Status.Code} - host ");
            if (res.Status.Code == "no-access")
            {
                var newId = (int.Parse(mettingId) - 1).ToString();
                Console.WriteLine($"new Id{ newId}");
                return AddHostToMeeting(newId, GroupId);
            }
            else
            {
                return res;

            }
        }


        private stateViewModel SetPermissionToMeeting(string SCoId, string principalid, string accessmodifer)
        {
            string parameter = $"&acl-id={SCoId}&principal-id={principalid}&permission-id={accessmodifer}";
            string updateurl = $"{BaseUrl}{UpdatePermissionUrl}{parameter}";
            var data = Client.GetFromXmlAsync<stateViewModel>(updateurl);
            return data;

        }



        public UserViewModel AddUser(string ServerAddres, string Name, string login, string Password)
        {
            try
            {
                string parameters = $"&first-name={Name}" +
               $"&last-name=''&login={login}" +
               $"&password={Password}&type=user&has-children=0";
                string url = $"{ServerAddres}{UpdatePrincipalUrl}{parameters}";
                var data = Client.GetFromXmlAsync<UserViewModel>(url);
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public UserViewModel AddGroup(string ServerAddress, string Name, string Description)
        {
            try
            {
                string parameters = $"&name={Name}" +
              $"&login={Name}&display-uid={Name}" +
              $"&description={Description}&type=group&has-children=1";
                string url = $"{ServerAddress}{UpdatePrincipalUrl}{parameters}";
                var data = Client.GetFromXmlAsync<UserViewModel>(url);
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }





        public stateViewModel AddUserToGroupASync(string GroupId, string UserId)
        {

            string parameters = $"&group-id={GroupId}&principal-id={UserId}&is-member=true";
            string url = $"{BaseUrl}{AddUserToGroupUrl}{parameters}";
            var data = Client.GetFromXmlAsync<stateViewModel>(url);
            Console.WriteLine($"user :{UserId} Add To group : {GroupId} ... {data.Status.Code}");
            if (data.Status.Code == "ok")
            {
                return data;

            }
            else
            {
                var newid = int.Parse(GroupId) - 1;
                Console.WriteLine($"new Id : {newid}");
                return AddUserToGroupASync(newid.ToString(), UserId);
            }
        }


        public PrincipalViewModel FindUser(string ServerAddress, string key, string value)
        {

            string url = $"{ServerAddress}{PrincipallistUrl}&filter-{key}={value}";
            var data = Client.GetFromXmlAsync<PrincipalViewModel>(url);
            return data;


        }
    }
}
