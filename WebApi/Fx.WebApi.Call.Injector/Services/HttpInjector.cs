using Newtonsoft.Json;

namespace Fx.WebApi.Call.Injector.Services
{
    public class HttpInjector : IHttpInjector
    {
        private readonly HttpClient _httpClient;
        private IConfiguration _configuration;
        private ILogger<HttpInjector> _logger;

        public HttpInjector(IConfiguration configuration, 
                            IHttpClientFactory httpClientFactory, 
                            ILogger<HttpInjector> logger)
        {
            _httpClient=httpClientFactory.CreateClient("Injector");
            _configuration = configuration;
            _logger = logger;
            ///_httpClient.BaseAddress = new Uri(_configuration["InjectorUrl"]);
            //_httpClient.DefaultRequestHeaders.Add("Accept", "Application/json");
        }

        public async Task<string> Headers()
        {
            string url = $"{_httpClient.BaseAddress}/headers";
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
            
        }

        public async Task<HttpResponseMessage> EvengridInjector()
        {
            string url = $"{_httpClient.BaseAddress}/EventGridInJector";
            return await _httpClient.GetAsync(url);
            
        }
    }
}
