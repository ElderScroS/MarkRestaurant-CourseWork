using MarkRestaurant.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkRestaurant.Controllers
{
    public class NavigationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ProductRepository _productRepository;

        public NavigationController(
            UserManager<User> userManager,
            ProductRepository productRepository
        )
        {
            _userManager = userManager;
            _productRepository = productRepository;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        #region  Logged

        [HttpPost]
        public async Task<IActionResult> LIndex(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Logged/LoggedIndex.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> LMenu(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Logged/LoggedMenu.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> LAbout(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Logged/LoggedAbout.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> LBookTable(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Logged/LoggedBookTable.cshtml", user);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Account/UserEdit.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> EnterIn(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Account/User.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> Basket(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Account/Basket.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordView(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return RedirectToAction("ResetPasswordView", user);
        }

        #endregion

        #region Admin

        [HttpPost]
        public IActionResult AdminAddProduct() => View("~/Views/Admin/AdminAddProduct.cshtml");
        [HttpPost]
        public IActionResult AdminDashboard() => View("~/Views/Admin/AdminDashboard.cshtml");
        [HttpPost]
        public IActionResult AdminTodayFinishedOrders() => View("~/Views/Admin/AdminTodayFinishedOrders.cshtml");
        [HttpPost]
        public IActionResult AdminFinishedOrders() => View("~/Views/Admin/AdminFinishedOrders.cshtml");
        [HttpPost]
        public IActionResult AdminActiveOrders() => View("~/Views/Admin/AdminActiveOrders.cshtml");
        [HttpPost]
        public IActionResult AdminMenu() => View("~/Views/Admin/AdminMenu.cshtml");
        [HttpPost]
        public async Task<IActionResult> AdminEditUser(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return View("~/Views/Admin/AdminEditUser.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> AdminUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View("~/Views/Admin/AdminUsers.cshtml", users);
        }

        #endregion

        #region Email 

        public IActionResult ResetPasswordView() => View("~/Views/Auth/ResetPasswordView.cshtml");
        public IActionResult EmailConfirm() => View("~/Views/Auth/EmailConfirm.cshtml");
        public IActionResult ConfirmEmailSucces() => View("~/Views/Auth/ConfirmEmailSucces.cshtml");
        public IActionResult ForgotPassword() => View("~/Views/Auth/ForgotPassword.cshtml");
        public IActionResult ResetPasswordSucces() => View("~/Views/Auth/ResetPasswordSucces.cshtml");

        #endregion
        
    }
}
