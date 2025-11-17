using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vdoflix.Web.Services;
using Vdoflix.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

// Identity + EF Core Sqlite
builder.Services.AddDbContext<Vdoflix.Web.Data.ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");
});
builder.Services.AddIdentity<Vdoflix.Web.Data.ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<Vdoflix.Web.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Auth cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

// HttpClient for TMDB
builder.Services.AddHttpClient("tmdb", c =>
{
    c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

// App services
builder.Services.AddScoped<Vdoflix.Web.Services.TmdbService>();

// Session for profile switching
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
