using EcommProject_1147.DataAccess.Data;
using EcommProject_1147.DataAccess.Repository;
using EcommProject_1147.DataAccess.Repository.IRepository;
using EcommProject_1147.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var cs = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(cs));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//          .AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
