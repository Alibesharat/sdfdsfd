using AdobeConectApi.ReadDataViewModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AdobeConectApi.IO
{
    public class FileService
    {

        string _Path;
        public FileService(string BasePath)
        {
            _Path = BasePath;
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


            string FullPath = Path.Combine(rootpath, _Path);
            FileViewModel vm = new FileViewModel();
            foreach (var thisPath in Directory.GetFiles(FullPath))
            {

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


        public bool WriteFileInJsonAsync(List<LandingViewModel> data, string path)
        {
            ensurePathExist(path);
            string json = JsonSerializer.Serialize(data.ToArray());
            System.IO.File.WriteAllText($@"{path}\users.json", json);
            Console.WriteLine($"write  data { data.Count}");
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
