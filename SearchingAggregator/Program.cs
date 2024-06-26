using Microsoft.EntityFrameworkCore;
using SearchingAggregator.Database;
using SearchingAggregator.Database.Repositories;
using SearchingAggregator.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISearchService, GoogleSearchService>();
builder.Services.AddScoped<ISearchResultsRepository, SearchResultsRepository>();
builder.Services.AddDbContext<SearchResultsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SearchResultsDbContext")));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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