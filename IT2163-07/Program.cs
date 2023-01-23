using AspNetCore.ReCaptcha;
using IT2163_07.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("GoogleRecaptcha"));
builder.Services.AddReCaptcha(option =>
{
    option.SiteKey = builder.Configuration["GoogleCaptchaSitKey"];
    option.SecretKey = builder.Configuration["GoogleCaptchaSecretKey"];
    option.Version = ReCaptchaVersion.V3;
    option.ScoreThreshold = 0.5;
       
});

builder.Services.AddSingleton<EmailSender>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
}).AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders(); 
builder.Services.AddDataProtection();
builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
    Config.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    Config.Cookie.MaxAge = TimeSpan.FromMinutes(10);
    Config.AccessDeniedPath = "/Errors/403";
    Config.SlidingExpiration = true;
});
builder.Services.AddTransient<SMSSender>();
builder.Services.Configure<SMSOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePagesWithRedirects("/Errors/{0}");

app.MapRazorPages();

app.Run();
