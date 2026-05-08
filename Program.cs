using AppliedInvoice.Components;
using AppliedInvoice.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<FbrService>();

var dbPath = Path.Combine(
    builder.Environment.WebRootPath, // 👈 this points to wwwroot
    "DB",
    "Invoice.db"
);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddSingleton<SQLiteService>();

var config = builder.Configuration;
var baseUrl = config["FBR:BaseUrl"];
var fbrGetToken = config["FBR:TokenGet"];
var fbrPostToken = config["FBR:TokenPost"];

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbrGetToken);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
