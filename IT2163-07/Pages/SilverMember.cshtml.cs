using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT2163_07.Pages
{
    [Authorize(Roles = "Silver")]
    public class SilverModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
