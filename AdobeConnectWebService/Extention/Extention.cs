using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AdobeConectApi
{
    public static class Extention
    {
        /// <summary>
        /// Serilaze an xml string To object
        /// </summary>
        /// <typeparam name="T">The Type You need to serializ xml into it</typeparam>
        /// <param name="XML">the xml string</param>
        /// <returns>return an object of type T</returns>
        public static T Serilaze<T>(this string XML)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(XML);
            var serializer = new XmlSerializer(typeof(T));

            using (var stream = new MemoryStream(byteArray))
            {
                var DATA = (T)serializer.Deserialize(stream);
                return DATA;
            }
        }



        /// <summary>
        /// Get Data from Client and Serilaze
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<T> GetFromXmlAsync<T>(this HttpClient client, string url)
        {
            try
            {
                var data = await client.GetStringAsync(url);
                if (data == null) throw new System.Exception($"Can not get data from {url}");
                var res = data.Serilaze<T>();
                if (res == null) throw new System.Exception($"can not serilaze data from {url} to type {typeof(T)}");
                return res;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                throw ex;
            }

        }


    }
}
