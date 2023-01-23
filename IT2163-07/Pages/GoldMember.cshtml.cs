using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT2163_07.Pages
{
    [Authorize(Roles ="Gold")]
    public class GoldMemberModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
