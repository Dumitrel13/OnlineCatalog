using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Repository;
using OnlineCatalog.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineCatalog;
using OnlineCatalog.Models;
using OnlineCatalog.Helpers;
using OnlineCatalog.Helpers.Interfaces;
using OnlineCatalog.Middleware;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString") ??
                       throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

//Deploy
builder.Services.Configure<IISOptions>(options =>
{
    options.ForwardClientCertificate = false;
});


//DbContext configuration
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.
    GetConnectionString("DefaultConnectionString")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
	{
		//options.SignIn.RequireConfirmedAccount = true;
	})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<RoleManager<ApplicationRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();

//Amazon S3
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

// Add services to the container.
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "ro", "en" };
    options.AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures)
    .SetDefaultCulture("en");
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.UseAuthentication();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<CheckTeacherAssignmentExpirationMiddleware>();

app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
