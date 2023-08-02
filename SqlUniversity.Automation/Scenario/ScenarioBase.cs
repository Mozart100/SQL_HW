using SqlUniversity.Model.Dtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace SqlUniversity.Automation.Scenario
{
    public abstract class ScenarioBase
    {
        public ScenarioBase(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public abstract string ScenarioName { get; }


        protected abstract Task RunScenario();


        public async Task StartRunScenario()
        {
            Console.WriteLine($"Valid scenarios: {ScenarioName} base url = {BaseUrl} started.");
            await RunScenario();

            Console.WriteLine($"Valid scenarios: {ScenarioName} finished successfully.");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("-------------------------------------------------------------------");
        }

        protected async Task<TDto> Get<TDto>(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // Check the response status
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<TDto>();

                    return res;
                }

                throw new Exception("Server failed.");
            }
        }
        protected async Task<TResponse> DeleteCommand<TResponse>(string url) where TResponse : class
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, options);
                    return responseData;
                }

                throw new Exception($"Failed to perform DELETE request to {url}");
            }

        }

        protected async Task<TResponse> RunPostCommand<TRequest, TResponse>(string url, TRequest request, bool isPostRequest = true) where TRequest : class
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                if (isPostRequest)
                {
                    response = await client.PostAsync(url, content);
                }
                else
                {
                    response = await client.PutAsync(url, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, options);
                    return responseData;
                }

                throw new Exception($"Failed Populate in {url}");
            }
        }

    }
}
