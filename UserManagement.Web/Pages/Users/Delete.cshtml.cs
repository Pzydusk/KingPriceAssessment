using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Web.DTOs;

namespace UserManagement.Web.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public UserDto User { get; set; } = new();

        [BindProperty]
        public Guid Id { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Id = id;

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "User not found.";
                return RedirectToPage("/Users/Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            User = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new UserDto();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"/api/users/{Id}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"API error ({(int)response.StatusCode}): {body}";
                return RedirectToPage("/Users/Index");
            }

            return RedirectToPage("/Users/Index");
        }
    }
}
