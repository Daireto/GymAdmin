using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//DataContext
builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//User configuration
//TODO: Make strongest password
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    cfg.SignIn.RequireConfirmedEmail = true;
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();

//Not authorized action configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});

//Inyections
builder.Services.AddScoped<IUserHelper, UserHelper>(); //IUserHelper
builder.Services.AddScoped<ICombosHelper, CombosHelper>(); //ICombosHelper
builder.Services.AddScoped<IBlobHelper, BlobHelper>(); //IBlobHelper
builder.Services.AddScoped<IMailHelper, MailHelper>(); //IMailHelper
builder.Services.AddTransient<SeedDb>(); //Seeder

builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); //Real time changes on views

var app = builder.Build();

SeedData();
void SeedData()
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (IServiceScope scope = scopedFactory.CreateScope())
    {
        SeedDb service = scope.ServiceProvider.GetService<SeedDb>();
        service.SeedAsync().Wait();
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}"); //Route for error pages

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //Enable authentication

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
