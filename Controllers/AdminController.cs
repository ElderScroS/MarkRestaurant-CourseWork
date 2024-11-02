using MarkRestaurant.Data.Repository;
using MarkRestaurant.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkRestaurant.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            OrderRepository orderRepository,
            UserManager<User> userManager,
            ProductRepository productRepository,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTodayFinishedOrder(Guid finishedOrderId)
        {
            await _orderRepository.RemoveFinishedOrder(finishedOrderId);

            return View("~/Views/Admin/AdminTodayFinishedOrders.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFinishedOrder(Guid finishedOrderId)
        {
            await _orderRepository.RemoveFinishedOrder(finishedOrderId);

            return View("~/Views/Admin/AdminFinishedOrders.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteActiveOrder(Guid productId, string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return View("Error", new ErrorViewModel("Error", "The order was not found."));
            }

            await _orderRepository.RemoveProductFromBasket(productId, user.Id);

            return View("~/Views/Admin/AdminActiveOrders.cshtml");
        }
        
        
        [HttpPost]
        public async Task<IActionResult> UpdateUser(string email, string name, string surname, string middleName, int age, string phoneNumber)
        {
            var user = await GetUserByEmailAsync(email);

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return View("Error", new ErrorViewModel("Error", "Error occurred while editing the user.", "Navigation", "AdminUsers"));
            }

            user.Name = name;
            user.Surname = surname;
            user.MiddleName = middleName;
            user.Age = age;
            user.PhoneNumber = phoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                var users = await _userManager.Users.ToListAsync();
                return View("~/Views/Admin/AdminUsers.cshtml", users);
            }

            return View("Error", new ErrorViewModel("Error", "Error occurred while editing the user.", "Navigation", "AdminUsers"));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return View("Error", new ErrorViewModel("Error", "The user was not found.", "Navigation", "AdminUsers"));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var users = await _userManager.Users.ToListAsync();
                return View("~/Views/Admin/AdminUsers.cshtml", users);
            }

            return View("Error", new ErrorViewModel("Error", "Error occurred while deleting the user.", "Navigation", "AdminUsers"));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProductFromMenu(Guid productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product == null)
            {
                return View("Error", new ErrorViewModel("Error", "The product was not found.", "Navigation", "AdminMenu"));
            }

            await _productRepository.DeleteProduct(product);

            return View("~/Views/Admin/AdminMenu.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToMenu(string category, string title, double price, IFormFile imageFile)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(category))
            {
                return View("Error", new ErrorViewModel("Error", "The product was not created. Invalid input data.", "Navigation", "AdminAddProduct"));
            }

            var existingProduct = await _productRepository.GetProductByTitleAndCategoryAsync(title, category);
            if (existingProduct != null)
            {
                return View("Error", new ErrorViewModel("Error", "The product already exists in the menu.", "Navigation", "AdminAddProduct"));
            }

            string imageUrl;

            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string categoryFolder = Path.Combine(uploadsFolder, category.ToLower());

                Directory.CreateDirectory(categoryFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(categoryFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                imageUrl = $"/images/{category.ToLower()}/{uniqueFileName}";
            }
            else
            {
                imageUrl = "/images/none.jpg";
            }

            Product product = new Product(category, title, price, imageUrl);

            await _productRepository.AddProduct(product);

            return View("~/Views/Admin/AdminMenu.cshtml");
        }
    }
}
