using MailKit.Search;
using MarkRestaurant.Data;
using MarkRestaurant.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MarkRestaurant.Data.Repository
{
    public class DashboardRepository
    {
        private readonly MarkRestaurantDbContext _context;

        public DashboardRepository(MarkRestaurantDbContext context)
        {
            _context = context;
        }

        // Подсчет общего количества заказов
        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.FinishedOrders.CountAsync();
        }

        // Вовзращение общего количества заказов
        public async Task<List<FinishedOrder>> GetFinishedOrdersAsync()
        {
            return await _context.FinishedOrders
                .Include(o => o.Product) 
                .Include(o => o.User)
                .ToListAsync();
        }

        // Вовзращение общего количества заказов
        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        // Законченные заказы сегодня
        public async Task<List<FinishedOrder>> GetTodayFinishedOrdersAsync()
        {
            return await _context.FinishedOrders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.CompletedAt.Date == DateTime.Today)
                .ToListAsync();
        }

        // Подсчет общей прибыли от всех заказов
        public async Task<double> GetTotalRevenueAsync()
        {
            var totalRevenue = await _context.FinishedOrders
                .SumAsync(o => o.Product.Price * o.Quantity);
            return Math.Round(totalRevenue);
        }

        // Подсчет количества зарегистрированных пользователей
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        // Подсчет количества активных заказов (в корзине, но не завершенных)
        public async Task<int> GetActiveOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        // Законченные заказы сегодня
        public async Task<int> GetTodayFinishedOrdersCountAsync()
        {
            return await _context.FinishedOrders
                .Where(o => o.CompletedAt.Date == DateTime.Today)
                .CountAsync();
        }

        // Получить средний доход на одного пользователя
        public async Task<double> GetAverageRevenuePerUserAsync()
        {
            var totalRevenue = await GetTotalRevenueAsync();
            var totalUsers = await GetTotalUsersCountAsync();

            return totalUsers > 0 ? totalRevenue / totalUsers : 0;
        }

        // Топ 3 юзера
        public async Task<List<User>> GetTopUsersAsync()
        {
            return await _context.FinishedOrders
                .GroupBy(order => order.UserId) 
                .Select(group => new
                {
                    UserId = group.Key,
                    OrderCount = group.Count() 
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(3)
                .Join(_context.Users, 
                    x => x.UserId,
                    user => user.Id,
                    (x, user) => user) 
                .ToListAsync();
        }

        public async Task<List<SalesData>> GetWeeklySalesAsync()
        {
            var salesData = await _context.FinishedOrders
                .Where(o => o.CompletedAt >= DateTime.Now.AddDays(-6))
                .GroupBy(o => o.CompletedAt.Date)
                .Select(g => new SalesData
                {
                    Day = g.Key.ToString("dddd", CultureInfo.InvariantCulture),
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    TotalSales = Math.Round(g.Sum(o => o.Product.Price * o.Quantity), 2)
                })
                .ToListAsync();

            var result = new List<SalesData>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i).Date;
                var salesForDay = salesData.FirstOrDefault(s => s.Date == date.ToString("dd/MM/yyyy"));

                if (salesForDay != null)
                {
                    result.Add(salesForDay);
                }
                else
                {
                    result.Add(new SalesData
                    {
                        Day = date.ToString("dddd", CultureInfo.InvariantCulture),
                        Date = date.ToString("dd/MM/yyyy"),
                        TotalSales = 0
                    });
                }
            }

            return result;
        }

        public async Task<List<UserRegistrationData>> GetWeeklyUserRegistrationsAsync()
        {
            var registrationData = await _context.Users
                .Where(u => u.RegistrationDate >= DateTime.Now.AddDays(-6))
                .GroupBy(u => u.RegistrationDate.Date)
                .Select(g => new UserRegistrationData
                {
                    Day = g.Key.ToString("dddd", CultureInfo.InvariantCulture),
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    RegistrationCount = g.Count()
                })
                .ToListAsync();

            var result = new List<UserRegistrationData>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i).Date;
                var registrationsForDay = registrationData.FirstOrDefault(r => r.Date == date.ToString("dd/MM/yyyy"));

                if (registrationsForDay != null)
                {
                    result.Add(registrationsForDay);
                }
                else
                {
                    result.Add(new UserRegistrationData
                    {
                        Day = date.ToString("dddd", CultureInfo.InvariantCulture),
                        Date = date.ToString("dd/MM/yyyy"),
                        RegistrationCount = 0
                    });
                }
            }

            return result;
        }

        public class SalesData
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public double TotalSales { get; set; }
        }

        public class UserRegistrationData
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public int RegistrationCount { get; set; }
        }
    }
}
