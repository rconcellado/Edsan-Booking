using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Repositories;
using EdsanBooking.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the DbContext for PostgreSQL
builder.Services.AddDbContext<BookingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Secure session cookie
    options.Cookie.IsEssential = true;
});

// Register the RoomRepository
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<GuestRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<CheckInRepository>();
builder.Services.AddScoped<ChargeRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<SettingRepository>();
builder.Services.AddScoped<PoolRepository>();
builder.Services.AddScoped<AccountRepository>();

// Register the RoomService
builder.Services.AddHttpClient<IDashBoardService, DashBoardService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

// Register the RoomService
builder.Services.AddHttpClient<IRoomService, RoomService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<IGuestService, GuestService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<ICheckInService, CheckInService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<IChargeService, ChargeService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<ISettingService, SettingService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

builder.Services.AddHttpClient<IPoolService, PoolService>(client =>
{
    // You can configure the base address or any default headers here if needed
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    // Example: client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Configure Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
