using System;
using System.Net.Http;

namespace LinuxRemoteTerminal.Utils
{
    internal class GeoUtil
    {
        public static string GetCountry(string IP)
        {
            string ipAddress = IP;
            string apiKey = "at_MXBGlr1pdK0pPiwl6mndcn3TO4Aw1";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync($"https://geo.ipify.org/api/v1?apiKey={apiKey}&ipAddress={ipAddress}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;
                        dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                        string countryName = responseObject.location.country + ", " + responseObject.location.city;

                        return countryName;
                    }
                    else
                    {
                        return "-";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Chyba!";
            }
        }
    }
}
