using AutoRealmProject.Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoRealmProject.Frontend
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public UserNameViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            var firstName = user?.FirstName ?? "User";
            return View("Default", firstName);
        }
    }
}
