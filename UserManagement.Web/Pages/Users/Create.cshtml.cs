using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Web.DTOs;

namespace UserManagement.Web.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
                
        [BindProperty]
        public UserDto User { get; set; } = new();
        
        public List<GroupDto> AllGroups { get; set; } = new();
        
        [BindProperty]
        public List<Guid> SelectedGroupIds { get; set; } = new();
        
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadGroupsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {            
            if (string.IsNullOrWhiteSpace(User.Name) ||
                string.IsNullOrWhiteSpace(User.Surname) ||
                string.IsNullOrWhiteSpace(User.Email))
            {
                ErrorMessage = "Name, Surname and Email are required.";
                await LoadGroupsAsync();
                return Page();
            }

            User.Groups = SelectedGroupIds
                .Select(id => new GroupDto { Id = id })
                .ToList();

            var client = _httpClientFactory.CreateClient("ApiClient");

            var json = JsonSerializer.Serialize(User);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/users", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"API error ({(int)response.StatusCode}): {body}";
                await LoadGroupsAsync();
                return Page();
            }

            return RedirectToPage("/Users/Index");
        }

        private async Task LoadGroupsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("/api/groups");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            AllGroups = JsonSerializer.Deserialize<List<GroupDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GroupDto>();
        }
    }
}
