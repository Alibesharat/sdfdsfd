using AdobeConectApi.ReadDataViewModel;
using AdobeConnectWebService.ApiViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        const string scocontents = "sco-contents";
        const string ShorcutUrl = "sco-shortcuts";
        const string AddMeetingUrl = "sco-update";
        const string UpdatePermissionUrl = "permissions-update";
        const string UpdatePrincipalUrl = "principal-update";
        const string AddUserToGroupUrl = "group-membership-update";
        const int ServerCout = 110;

        public AdobeConnectService(string Domin, string UserName, string Password)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            Client = new HttpClient(clientHandler);
            _Domin = Domin;
            _userName = UserName;
            _Password = Password;
        }

        public AddUserResultViewModel AddUserDataToMeeting(UserInfoViewModel userData, List<FileViewModel> meetings)
        {
            AddUserResultViewModel model=new AddUserResultViewModel();

            try
            {
                object obj = new object();
                var serverName = GetServerName(meetings, userData.GroupCode);
                if (!string.IsNullOrWhiteSpace(serverName))
                {
                    var meeting = FindMettingOnServers(serverName, userData.GroupCode);
                    var gr = AddGroup(serverName, GetUniqName(), $"Group for {userData.GroupCode}");
                    if (gr.Status.Code == "ok")
                    {
                        string Address = $@"http://{serverName}.{_Domin}/api/xml?action=";

                        var SetGroupToMeeting = SetPermmionUserToMeeting(Address, meeting.id, gr.Principal.Principalid);


                        var res = AddUser(Address, userData);

                        if (res.Status?.Code == "ok")
                        {
                            lock (obj)
                            {
                                if (userData.IsTeacher)
                                {
                                   var state= SetPermmionHostToMeeting(Address, meeting.id, res.Principal.Principalid);
                                    if (state.Status.Code == "ok")
                                    {
                                        model=new AddUserResultViewModel() { UserName = userData.UserName, url =$@"http://{serverName}.{_Domin}/{ meeting.url}", IsSucess = true };

                                    }
                                }
                                else
                                {
                                    var result = AddUserToGroup(Address, gr.Principal.Principalid, res.Principal.Principalid);
                                    if (res.Status.Code == "ok")
                                    {
                                        model = new AddUserResultViewModel() {Date=userData.Date, UserName = userData.UserName, url = $@"http://{serverName}.{_Domin}/{ meeting.url}", IsSucess = true };
                                    }
                                }

                            }
                        }
                        else
                        {
                            model = new AddUserResultViewModel() { Date = userData.Date, UserName = userData.UserName, url = $@"http://{serverName}.{_Domin}/{ meeting.url}", IsSucess = false,ExMessage=res.Status.Code };

                        }


                    }
                    else
                    {
                        model = new AddUserResultViewModel() { Date = userData.Date,UserName = userData.UserName,  url = userData.GroupCode, IsSucess = false, ExMessage = "گروه اضافه نشد " };

                    }
                }
                else
                {
                    model = new AddUserResultViewModel() { Date = userData.Date, UserName = userData.UserName,  url = userData.GroupCode, IsSucess = false, ExMessage = "میتینگ یافت نشد" };

                }
            }
            catch (Exception ex)
            {

                model = new AddUserResultViewModel() { Date = userData.Date, UserName = userData.UserName, url = userData.GroupCode, IsSucess = false, ExMessage = ex.Message };
            }

            return model;

        }

        private string GetServerName(List<FileViewModel> meetings, string groupCode)
        {
            foreach (var file in meetings)
            {
                if (file.Mettings.Contains(groupCode))
                {
                    return file.ServerName;
                }
            }
            return "";
        }

        private string GetUniqName()
        {
            return Guid.NewGuid().ToString().Substring(4, 10);
        }


        private (string id,string url) FindMettingOnServers(string ServerName, string Id)
        {
            object obj = new object();

            string Address = $@"http://{ServerName}.{_Domin}/api/xml?action=";
            Console.WriteLine($"== Search in  {Address}");
            if (Login(Address))
            {
                lock (obj)
                {

                    var gr = FindMeeting(Address, Id);
                    if (gr != null && gr.Status.Code == "ok")
                    {
                        if (gr.Scos != null)
                        {
                            var id = gr.Scos.Sco.Scoid;
                            return (id,gr.Scos.Sco.Urlpath);
                        }



                    }
                    return ("","");
                }
            }
            else
            {
                throw new Exception($"Can not Login With UserName {_userName} And Password {_Password} on Server {Address}");
            }

        }






        public List<AddMettingResultViewModel> AddMetingsToServers(List<FileViewModel> data)
        {
            object obj = new object();
            List<AddMettingResultViewModel> vm = new List<AddMettingResultViewModel>();
            foreach (var item in data)
            {
                lock (obj)
                {
                    string ServerUrl = $"http://{item.ServerName}.{_Domin}/api/xml?action=";
                    if (Login(ServerUrl))
                    {
                        lock (obj)
                        {
                            foreach (var file in item.Mettings)
                            {
                                try
                                {

                                    var res = AddMeeting(ServerUrl, file, $"c{file}");
                                    if (res.Status?.Code == "ok")
                                    {

                                        var result = SetPermission(ServerUrl, res.Sco.Scoid, "public-access", "denied");
                                        if (result.Status.Code == "ok")
                                        {
                                            vm.Add(new AddMettingResultViewModel() { MettingName = file, SeverAddress = $"http://{item.ServerName}.{_Domin}", IsSucess = true, Permission = "denied" });

                                        }
                                        else
                                        {
                                            vm.Add(new AddMettingResultViewModel() { MettingName = file, SeverAddress = $"http://{item.ServerName}.{_Domin}", IsSucess = true, Permission = "Noting" });

                                        }
                                    }
                                    else
                                    {
                                        vm.Add(new AddMettingResultViewModel() { MettingName = file, SeverAddress = $"http://{item.ServerName}.{_Domin}", IsSucess = false, ExMessage = res.Status.Code });

                                    }
                                }
                                catch (Exception ex)
                                {

                                    vm.Add(new AddMettingResultViewModel() { MettingName = file, SeverAddress = $"http://{item.ServerName}.{_Domin}", IsSucess = false, ExMessage = ex.Message });
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"Could Not Login in {ServerUrl} With : {_userName} - {_Password}");
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







        private string GetFolderId(string ServerAddress)
        {
            try
            {
                string url = $"{ServerAddress}{ShorcutUrl}";
                var data = Client.GetFromXmlAsync<ShortCutViewModel>(url);
                string FolderId = data.Shortcuts.Folders.FirstOrDefault(c => c.Type == "meetings").Scoid;
                return FolderId;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        private MeetingViewModel AddMeeting(string ServerAddress, string Name, string AddressPath)
        {
            string FolderId = GetFolderId(ServerAddress);
            string StartDate = "2006-10-01T09:00";
            string EndDate = "2006-10-01T17:00";
            string Descripton = "this a new meting";
            string parameters = $"&folder-id={FolderId}" +
                $"&description={Descripton}&name={Name}" +
                $"&type=meeting&lang=en&date-begin={StartDate}&date-end={EndDate}" +
                $"&url-path={AddressPath}";
            string url = $"{ServerAddress}{AddMeetingUrl}{parameters}";
            var data = Client.GetFromXmlAsync<MeetingViewModel>(url);
            if (data.Status.Code == "ok")
            {
                return data;
            }
            return null;
        }




        private stateViewModel SetPermmionHostToMeeting(string ServerAddress, string mettingId, string PrinciaplId)
        {

            var e = SetPermission(ServerAddress, mettingId, PrinciaplId, "view");
            var res = SetPermission(ServerAddress, mettingId, PrinciaplId, "host");
            if (res.Status.Code == "no-access")
            {
                var newId = (int.Parse(mettingId) - 1).ToString();
                Console.WriteLine($"new Id{ newId}");
                return SetPermmionHostToMeeting(ServerAddress, newId, PrinciaplId);
            }
            else
            {
                return res;

            }
        }

        private stateViewModel SetPermmionUserToMeeting(string ServerAddress, string mettingId, string GroupId)
        {

            var res = SetPermission(ServerAddress, mettingId, GroupId, "view");
            if (res.Status.Code == "no-access")
            {
                var newId = (int.Parse(mettingId) - 1).ToString();
                Console.WriteLine($"new Id{ newId}");
                return SetPermmionHostToMeeting(ServerAddress, newId, GroupId);
            }
            else
            {
                return res;

            }
        }





        private stateViewModel SetPermission(string ServerAdress, string SCoId, string principalid, string accessmodifer)
        {
            string parameter = $"&acl-id={SCoId}&principal-id={principalid}&permission-id={accessmodifer}";
            string updateurl = $"{ServerAdress}{UpdatePermissionUrl}{parameter}";
            var data = Client.GetFromXmlAsync<stateViewModel>(updateurl);
            return data;

        }



        public UserViewModel AddUser(string ServerAddress, UserInfoViewModel userdata)
        {
            try
            {
                string parameters = $"&first-name={userdata.Name}" +
               $"&last-name={userdata.LastName}&login={userdata.UserName}" +
               $"&password={userdata.Password}&type=user&has-children=0";
                string url = $"{ServerAddress}{UpdatePrincipalUrl}{parameters}";
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
                string Address = $@"http://{ServerAddress}.{_Domin}/api/xml?action=";

                string parameters = $"&name={Name}" +
              $"&login={Name}&display-uid={Name}" +
              $"&description={Description}&type=group&has-children=1";
                string url = $"{Address}{UpdatePrincipalUrl}{parameters}";
                var data = Client.GetFromXmlAsync<UserViewModel>(url);
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }





        private (stateViewModel, string) AddUserToGroup(string ServerAddress, string GroupId, string UserId)
        {

            string parameters = $"&group-id={GroupId}&principal-id={UserId}&is-member=true";
            string url = $"{ServerAddress}{AddUserToGroupUrl}{parameters}";
            var data = Client.GetFromXmlAsync<stateViewModel>(url);
            if (data.Status.Code == "ok")
            {

                return (data, GroupId);

            }
            else
            {
                var newid = int.Parse(GroupId) - 1;
                return AddUserToGroup(ServerAddress, newid.ToString(), UserId);
            }
        }


        public PrincipalViewModel FindUser(string ServerAddress, string key, string value)
        {

            string url = $"{ServerAddress}{PrincipallistUrl}&filter-{key}={value}";
            var data = Client.GetFromXmlAsync<PrincipalViewModel>(url);
            return data;


        }


        public MeetingsViewModel FindMeeting(string ServerAddress, string value)
        {

            string FolderId = GetFolderId(ServerAddress);
            string url = $"{ServerAddress}{scocontents}&sco-id={FolderId}&filter-type=meeting&filter-name={value}";
            var data = Client.GetFromXmlAsync<MeetingsViewModel>(url);
            return data;


        }
    }
}
