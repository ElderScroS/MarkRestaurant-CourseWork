using MarkRestaurant;
using MarkRestaurant.Data;
using MarkRestaurant.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<MarkRestaurantDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<MarkRestaurantDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MarkRestaurantDbContext"),
                          options => options.EnableRetryOnFailure());
});

builder.Services.AddTransient<ProductRepository>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<DashboardRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MarkRestaurantDbContext>();

    context.Database.EnsureCreated();

    await context.SeedDataAsync();
    await context.SeedAdminAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
