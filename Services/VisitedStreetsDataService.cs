using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vamdrup_rundt.Models;

namespace Vamdrup_rundt.Services
{
    public class VisitedStreetsDataService
    {
        private readonly string baseString = "http://172.20.10.2:5299/api/";
        public async Task<bool> PostVisitedStreet(VisitedStreetsModel visitedStreetsModel)
        {
            var url = baseString + "VisitedStreet";
            var userJson = JsonConvert.SerializeObject(visitedStreetsModel);
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Error: {errorContent}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<List<StreetModel>> GetAllUsersActiveStreets(string email)
        {
            var url = baseString + "GetAllUsersActiveStreets?email="+ email;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var streets = JsonConvert.DeserializeObject<List<StreetModel>>(jsonResponse);
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
