using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vamdrup_rundt.Models;

namespace Vamdrup_rundt.Services
{
    public class StreetDataService
    {
        //private readonly string baseString = "http://192.168.9.119:5299/api/";
        private readonly string baseString = "http://srv589522.hstgr.cloud:5299/api/";
        public async Task<List<StreetModel2>> GetAllStreetsInCity(int postnummer)
        {
            var url = baseString + "StreetData/"  + postnummer;
            Debug.WriteLine(url);
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        var streets = JsonConvert.DeserializeObject<List<StreetModel2>>(jsonResponse);
                        return streets;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Error: {errorContent}");
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;

            }
        }
    }
}
