using AdobeConectApi.ReadDataViewModel;
using AdobeConnectWebService.ApiViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ViewModels;

namespace AdobeConectApi.IO
{
    public class FileService
    {

        string _Path;
        string _UserDataPath;
        public FileService(string BasePath, string UserDataPath)
        {
            _Path = BasePath;
            _UserDataPath = UserDataPath;
        }



        public object obj = new object();
        public void Rename()
        {

            foreach (var folder in Directory.GetDirectories(_Path))
            {
                lock (obj)
                {
                    foreach (var path in Directory.GetFiles(folder))
                    {
                        lock (obj)
                        {
                            string newpath = path.Replace("csv", "xlsx");

                            File.Move(path, newpath);
                            Console.WriteLine($"rename from {path} to {newpath}");
                        }

                    }
                }


            }


        }


        public List<FileViewModel> GetMeetings(string rootpath)
        {
            List<FileViewModel> lst = new List<FileViewModel>();


            string FullPath = Path.Combine(rootpath, "wwwroot", _Path);

            foreach (var thisPath in Directory.GetFiles(FullPath))
            {
                FileViewModel vm = new FileViewModel();
                string filename = Path.GetFileNameWithoutExtension(thisPath);
                vm.ServerName = filename;
                try
                {
                    vm.Mettings = File.ReadAllLines(thisPath).ToList();

                }
                catch (Exception)
                {

                    Console.WriteLine(filename);
                    continue;
                }
                lst.Add(vm);

            }

            return lst;
        }



        public List<AddUserResultViewModel> GetUsers(string rootpath)
        {
            string FullPath = Path.Combine(rootpath, "wwwroot", _UserDataPath, "users.json");
            var users = File.ReadAllLines(FullPath).ToList();
            var result = string.Join("},", users.ToArray());
            result = result.Replace("}", "},");
            result = result.TrimEnd(',');
            result = $"[{result}]";
            List<AddUserResultViewModel> ls = JsonSerializer.Deserialize<List<AddUserResultViewModel>>(result);

            return ls;
        }




        public void LogGetRepeaded()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int i = 1;
            foreach (var Folder in Directory.GetDirectories(_Path))
            {
                string ServerName = Folder.Split('\\').LastOrDefault();
                foreach (var thisPath in Directory.GetFiles(Folder))
                {

                    string filename = Path.GetFileNameWithoutExtension(thisPath);
                    //filename = filename.Split("-")[0];

                    try
                    {
                        dic.Add(filename, thisPath);

                    }
                    catch (Exception ex)
                    {
                        var data = dic.FirstOrDefault(c => c.Key == filename);
                        string newServerName = data.Value.Split('\\').FirstOrDefault();

                        Console.WriteLine($"({i}) {filename} - {ServerName} -- {data.Key} - {newServerName}");
                        i++;
                        continue;
                    }

                }


            }



        }


        public bool WriteFileInJson(AddUserResultViewModel data, string rootpath, bool IsSuccess)
        {
            string FullPath = Path.Combine(rootpath, "wwwroot", _UserDataPath);

            ensurePathExist(FullPath);
            string jsonData = JsonSerializer.Serialize(data);
            if (IsSuccess)
            {
                File.AppendAllText($@"{FullPath}\users.json", jsonData);

            }
            else
            {
                File.AppendAllText($@"{FullPath}\faild.json", jsonData);

            }
            return true;


        }


        private void ensurePathExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
