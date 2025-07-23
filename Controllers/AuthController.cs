using Duende.IdentityServer.Services;
using HomeTrack.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeTrack.IdentityServer.Controllers;

[Route("auth")]
public class AuthController(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    IIdentityServerInteractionService interactionService) : Controller
{
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IIdentityServerInteractionService _interactionService = interactionService;

    [HttpGet("login")]
    public IActionResult Login(string returnUrl)
    {
        var viewModel = new LoginViewModel()
        {
            ReturnUrl = returnUrl
        };
        return View(viewModel);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await _userManager.FindByNameAsync(viewModel.UserName);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Login error");
            return View(viewModel);
        }

        var result = await _signInManager.PasswordSignInAsync(viewModel.UserName,
            viewModel.Password, false, false);

        if (result.Succeeded && viewModel.ReturnUrl is not null)
        {
            return Redirect(viewModel.ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, "Error occured");

        return View(viewModel);
    }

    [HttpGet("register")]
    public IActionResult Register(string returnUrl)
    {
        var viewModel = new RegisterViewModel()
        {
            ReturnUrl = returnUrl
        };
        return View(viewModel);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = new AppUser()
        {
            UserName = viewModel.UserName,
        };

        var result = await _userManager.CreateAsync(user, viewModel.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return Redirect(viewModel.ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, "Error occured");
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        await _signInManager.SignOutAsync();

        var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

        if (logoutRequest.PostLogoutRedirectUri != null)
        {
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
        else
        {
            return View();
        }
    }
}
