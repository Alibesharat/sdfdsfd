using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ViewModels;

namespace AdobeConectApi.Service
{
    public class AdobeConnectService
    {
        HttpClient Client;
        readonly string BaseUrl;
        const string LoginUrl = "login";
        const string PrincipallistUrl = "principal-list";
        const string ShorcutUrl = "sco-shortcuts";
        const string AddMeetingUrl = "sco-update";
        const string UpdatePermissionUrl = "permissions-update";
        const string UpdatePrincipalUrl = "principal-update";
        const string AddUserToGroupUrl = "group-membership-update";
        public AdobeConnectService(string Ip)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            Client = new HttpClient(clientHandler);

            BaseUrl = $"https://{Ip}/api/xml?action=";
        }


        public async Task<bool> Login(string UserName, string Password)
        {
            string parameter = $"&login={UserName}&password={Password}";
            string url = $"{BaseUrl}{LoginUrl}{parameter}";
            var data = await Client.GetFromXmlAsync<TokenViewModel>(url);
            if (data.Status.Code == "ok")
            {
                //Client.DefaultRequestHeaders.Add("Token", data.OWASP_CSRFTOKEN.Token);
                //string cookie = "BREEZESESSION=breez3mpwg7bimtm68z7r; BreezeCCookie=conn-E0Y4-VSKE-357P-XDIU-7CFU-619U-61LO-65MS; BreezeLoginCookie=connect@ili.ir";
                //Client.DefaultRequestHeaders.Add("cookie", cookie);
                Console.WriteLine($"Login Sucees : user {UserName}");
                return true;

            }
            Console.WriteLine($"Login faild : user {UserName}");
            return false;


        }



        public async Task<object> GetPrincipallist()
        {
            string url = $"{BaseUrl}{PrincipallistUrl}";
            var data = await Client.GetFromXmlAsync<object>(url);
            return data;
        }


        public async Task<ShortCutViewModel> GetShortCut()
        {
            string url = $"{BaseUrl}{ShorcutUrl}";
            var data = await Client.GetFromXmlAsync<ShortCutViewModel>(url);
            return data;
        }


        private async Task<string> GetFolderId()
        {
            try
            {
                string url = $"{BaseUrl}{ShorcutUrl}";
                var data = await Client.GetFromXmlAsync<ShortCutViewModel>(url);
                string FolderId = data.Shortcuts.Folders.FirstOrDefault(c => c.Type == "meetings").Scoid;
                return FolderId;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        public async Task<MeetingViewModel> AddMeetingAddParticapnts(string Name, string AddressPath, string GroupId)
        {
            string FolderId = await GetFolderId();
            string StartDate = "2006-10-01T09:00";
            string EndDate = "2006-10-01T17:00";
            string Descripton = "this a new meting";
            string parameters = $"&folder-id={FolderId}" +
                $"&description={Descripton}&name={Name}" +
                $"&type=meeting&lang=en&date-begin={StartDate}&date-end={EndDate}" +
                $"&url-path={AddressPath}";
            string url = $"{BaseUrl}{AddMeetingUrl}{parameters}";
            var data = await Client.GetFromXmlAsync<MeetingViewModel>(url);
            if (data.Status.Code == "ok")
            {
                var res = await SetPermissionToMeeting(data.Sco.Scoid, "public-access", "denied");
                Console.WriteLine($"Set Permisson for {Name} : {res.Status.Code} - remove ");
                res = await SetPermissionToMeeting(data.Sco.Scoid, GroupId, "view");
                Console.WriteLine($"Set Permisson for {Name} : {res.Status.Code} - view ");


            }
            return data;
        }


        public stateViewModel AddHostToMeeting(MeetingViewModel data, string GroupId)
        {

            var e = SetPermissionToMeeting(data.Sco.Scoid, GroupId, "view").Result;
            Console.WriteLine($"Set Permisson for {GroupId} : {e.Status.Code} - view ");

            var res = SetPermissionToMeeting(data.Sco.Scoid, GroupId, "host").Result;
            Console.WriteLine($"Set Permisson for {GroupId} : {res.Status.Code} - host ");

            return res;
        }

        public stateViewModel AddHostToMeeting(string mettingId, string GroupId)
        {

            var e = SetPermissionToMeeting(mettingId, GroupId, "view").Result;
            Console.WriteLine($"Set Permisson for {GroupId} : {e.Status.Code} - view ");

            var res = SetPermissionToMeeting(mettingId, GroupId, "host").Result;
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


        private async Task<stateViewModel> SetPermissionToMeeting(string SCoId, string principalid, string accessmodifer)
        {
            string parameter = $"&acl-id={SCoId}&principal-id={principalid}&permission-id={accessmodifer}";
            string updateurl = $"{BaseUrl}{UpdatePermissionUrl}{parameter}";
            var data = await Client.GetFromXmlAsync<stateViewModel>(updateurl);
            return data;

        }



        public async Task<UserViewModel> AddUserAsync(string Name, string lastName, string login, string Password)
        {
            string parameters = $"&first-name={Name}" +
                $"&last-name={lastName}&login={login}" +
                $"&password={Password}&type=user&has-children=0";
            string url = $"{BaseUrl}{UpdatePrincipalUrl}{parameters}";
            var data = await Client.GetFromXmlAsync<UserViewModel>(url);
            return data;
        }


        public async Task<UserViewModel> AddGroupAsync(string Name, string Description)
        {

            string parameters = $"&name={Name}" +
                $"&login={Name}&display-uid={Name}" +
                $"&description={Description}&type=group&has-children=1";
            string url = $"{BaseUrl}{UpdatePrincipalUrl}{parameters}";
            var data = await Client.GetFromXmlAsync<UserViewModel>(url);
            return data;
        }





        public async Task<stateViewModel> AddUserToGroupASync(string GroupId, string UserId)
        {

            string parameters = $"&group-id={GroupId}&principal-id={UserId}&is-member=true";
            string url = $"{BaseUrl}{AddUserToGroupUrl}{parameters}";
            var data = await Client.GetFromXmlAsync<stateViewModel>(url);
            Console.WriteLine($"user :{UserId} Add To group : {GroupId} ... {data.Status.Code}");
            if (data.Status.Code == "ok")
            {
                return data;

            }
            else
            {
                var newid = int.Parse(GroupId) - 1;
                Console.WriteLine($"new Id : {newid}");
                return await AddUserToGroupASync(newid.ToString(), UserId);
            }
        }


        public async Task<PrincipalViewModel> FindUserAsync(string key, string value)
        {
            string url = $"{BaseUrl}{PrincipallistUrl}&filter-{key}={value}";
            var data = await Client.GetFromXmlAsync<PrincipalViewModel>(url);
            return data;
        }
    }
}
