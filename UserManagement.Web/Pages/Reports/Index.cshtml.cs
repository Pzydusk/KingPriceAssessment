using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Web.DTOs;

namespace UserManagement.Web.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public int TotalUsers { get; set; }
        public List<UsersPerGroupDto> UsersPerGroup { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                var countResponse = await client.GetAsync("/api/users/count");
                countResponse.EnsureSuccessStatusCode();

                var countJson = await countResponse.Content.ReadAsStringAsync();
                TotalUsers = JsonSerializer.Deserialize<int>(countJson);

                var perGroupResponse = await client.GetAsync("/api/users/perGroup");
                perGroupResponse.EnsureSuccessStatusCode();

                var perGroupJson = await perGroupResponse.Content.ReadAsStringAsync();
                UsersPerGroup = JsonSerializer.Deserialize<List<UsersPerGroupDto>>(perGroupJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
