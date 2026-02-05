using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Web.DTOs;

namespace UserManagement.Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<UserDto> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            Users = JsonSerializer.Deserialize<List<UserDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }
    }
}