using AuthorizationServer.Models.Account;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    [Route("")]
    public class AccountController : Controller
    {
        private AuthDbContext dbContext;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IClientStore clientStore;

        public AccountController(AuthDbContext dbContext, IIdentityServerInteractionService interaction, IClientStore clientStore)
        {
            this.dbContext = dbContext;
            this.interaction = interaction;
            this.clientStore = clientStore;
        }

        [HttpGet("consent")]
        public async Task<IActionResult> Consent(string returnUrl)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpGet("account/login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("account/login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == loginModel.Username);
            var request = interaction.GetAuthorizationContextAsync(loginModel.ReturnUrl).Result;
            if (user != null)
            {
                if (user.Password == loginModel.Password.Sha256())
                {
                    await AuthenticationManagerExtensions.SignInAsync(HttpContext, user.Id.ToString(), user.Username, new Claim("Name", loginModel.Username));
                    interaction.GrantConsentAsync(request, new ConsentResponse
                    {
                        ScopesConsented = new[] { "offline_access", "api" }
                    }).Wait();
                    return Redirect(loginModel.ReturnUrl);
                }
            }
            return Redirect("~/");
        }
    }
}
