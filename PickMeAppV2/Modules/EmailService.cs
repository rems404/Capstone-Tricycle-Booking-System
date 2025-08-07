using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PickMeAppV2.ForePages;

namespace PickMeAppV2.Modules
{
    public class EmailService
    {
        public async Task<bool> ResetPasswordService(String UserId)
        {
            bool ReturnVal = false;

            var id = UserId;

            var client = HttpClientService.Client;
            var values = new Dictionary<String, string>
            {
                { "userid", id }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("forgot-password-email.php", content);
            var result = await response.Content.ReadAsStringAsync();

            if (result.Contains("email sent"))
            {
                ReturnVal = true;
            } 

            return ReturnVal;
        }

        public async Task<bool> RequestConfirmation(String UserId, String action)
        {
            var id = UserId;

            var client = HttpClientService.Client;
            var values = new Dictionary<String, string>
            {
                { "userid", id }, { "action", action }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("request-confirmation.php", content);
            var result = await response.Content.ReadAsStringAsync();

            return result.Contains("email sent");
        }
    }

    public static class HttpClientService
    {
        private static readonly HttpClient _client;

        static HttpClientService()
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(30); // Optional
            _client.BaseAddress = new Uri("http://localhost/PMA/phpmailer/"); // Optional
        }

        public static HttpClient Client => _client;
    }
}
