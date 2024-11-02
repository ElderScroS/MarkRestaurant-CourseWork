using MarkRestaurant.Data;
using MarkRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarkRestaurant.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly MarkRestaurantDbContext _context;

        public AuthController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            MarkRestaurantDbContext context,
            IEmailSender emailSender
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string passwordHash)
        {
            var admin = _context.Admins.SingleOrDefault(a => a.Username == email);

            if (admin != null)
            {
                var passwordHasher = new PasswordHasher<Admin>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, passwordHash);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    var users = _userManager.Users.ToList();
                    return View("~/Views/Admin/AdminUsers.cshtml", users);
                }
            }

            var user = await GetUserByEmailAsync(email);

            if (user == null || user.EmailConfirmed == false)
            {
                return View("Error", new ErrorViewModel("Error", "Invalid username or password, or email has not been confirmed"));
            }

            var result = await _signInManager.PasswordSignInAsync(email, passwordHash, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return View("~/Views/Account/UserEdit.cshtml", user);
            }

            return View("Error", new ErrorViewModel("Error", "Invalid login attempt"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string passwordHash)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await GetUserByEmailAsync(email);

                if (existingUser != null)
                {
                    return View("Error", new ErrorViewModel("Error", "User already exists"));
                }

                var user = new User(email, passwordHash);
                var result = await _userManager.CreateAsync(user, passwordHash);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, Request.Scheme);

                    await _emailSender.SendConfirmationEmailAsync(email, "Confirm Your Account",
                        $"Please confirm your account by clicking the button below:<br><a href='{confirmationLink}'>Confirm Account</a>");


                    return RedirectToAction("EmailConfirm", "Navigation");
                }

                return View("Error", new ErrorViewModel("Registration Failed", "Registration failed. Please check your details."));
            }

            return View("Error", new ErrorViewModel("Error", "Something went wrong"));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error", new ErrorViewModel("Error", "Something went wrong"));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("ConfirmEmailSucces", "Navigation");
            }
            else
            {
                return View("Error", new ErrorViewModel("Error", "Something went wrong"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View("Error", new ErrorViewModel("Empty", "Email is required."));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                return View("Error", new ErrorViewModel("Invalid email", "Invalid email or email has not been confirmed."));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPasswordView", "Auth", new { userId = user.Id, token }, Request.Scheme);

            await _emailSender.SendForgotPasswordEmailAsync(email, resetLink);

            return RedirectToAction("ForgotPassword", "Navigation");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPasswordView(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            ViewData["Token"] = token;

            return View("ResetPasswordView", user);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId, string token, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                await _emailSender.SendPasswordChangedEmailAsync(user.Email);

                return RedirectToAction("ResetPasswordSucces", "Navigation");
            }

            foreach (var error in result.Errors)
            {
                return View("Error", new ErrorViewModel("Error", error.Description));
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
