using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vamdrup_rundt.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace Vamdrup_rundt.Services
{
    public class UserService
    {
        private readonly string baseString = "http://192.168.9.119:5299/api/userdata/";

        public async Task<bool> LogUserInAsync(UserModel user)
        {
            var url = baseString + "login";
            Debug.WriteLine($"Attempting to connect to URL: {url}");
            var userJson = JsonConvert.SerializeObject(user);
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");
            Debug.WriteLine(content.ToString()+ " hest");
            Debug.WriteLine(userJson.ToString() + " Ko");
            try
            {
                var response = await HttpClientSingleton.Client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var cookies = HttpClientSingleton.Handler.CookieContainer.GetCookies(new Uri(baseString));
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error: {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException httpRequestEx)
            {
                Debug.WriteLine($"HttpRequestException: {httpRequestEx.Message}");
                if (httpRequestEx.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {httpRequestEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}