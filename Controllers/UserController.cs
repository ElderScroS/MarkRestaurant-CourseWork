using MarkRestaurant.Data.Repository;
using MarkRestaurant.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarkRestaurant.Controllers
{
    public class UserController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(
            OrderRepository orderRepository,
            UserManager<User> userManager,
            ProductRepository productRepository,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(string productId, string email)
        {
            var user = await GetUserByEmailAsync(email);
            var product = await _productRepository.GetProductById(Guid.Parse(productId));

            if (user == null || product == null)
            {
                return View("Error", new ErrorViewModel("Error", "The user or product was not found."));
            }

            await _orderRepository.AddOrderToOrders(product, user);

            return View("~/Views/Logged/LoggedMenu.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProductFromBasket(Guid productId, string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return View("Error", new ErrorViewModel("Error", "The user or product was not found."));
            }

            await _orderRepository.RemoveProductFromBasket(productId, user.Id);

            return View("~/Views/Account/Basket.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> ClearProductsInBasket(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return View("Error", new ErrorViewModel("Error", "The user or product was not found."));
            }

            await _orderRepository.ClearProductsByUser(user.Id);

            return View("~/Views/Account/Basket.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> FinishOrder(string email, double totalPrice)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return View("Error", new ErrorViewModel("Error", "The user or product was not found."));
            }

            var orders = await _orderRepository.FinishOrder(user.Id);
            if (orders == null || !orders.Any())
            {
                return View("Error", new ErrorViewModel("Error", "Order could not be completed."));
            }

            var orderNumber = GenerateOrderNumber();

            await _emailSender.SendReceiptEmailAsync(email, "Your Receipt from Mark Restaurant", orderNumber, user.Name, orders, totalPrice);

            return View("~/Views/Logged/LoggedIndex.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> Save(string email, string name, string surname, string middleName, int age, string phoneNumber, IFormFile profileImage)
        {
            var user = await GetUserByEmailAsync(email);

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return View("~/Views/Account/UserEdit.cshtml", user);
            }

            user.Name = name;
            user.Surname = surname;
            user.MiddleName = middleName;
            user.Age = age;
            user.PhoneNumber = phoneNumber;

            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "usersImg");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                user.ProfileImagePath = $"/usersImg/{uniqueFileName}";
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                return View("~/Views/Account/User.cshtml", user);
            }

            return View("~/Views/Account/UserEdit.cshtml", user);
        }
        private string GenerateOrderNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            return "#" + new string(Enumerable.Repeat(chars, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
