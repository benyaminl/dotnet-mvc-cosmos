using Microsoft.Azure.Cosmos;
using coba.Repositories.Domain;
using coba.Repositories.Infrastructure;
using Microsoft.Azure.Cosmos.Fluent;
using coba.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DB Cosmos Client
string url = builder.Configuration.GetValue<string>("COSMOS_ENDPOINT");
string authKey = builder.Configuration.GetValue<string>("COSMOS_KEY");
// prepare the singleton connection, once for all
builder.Services.AddSingleton<CosmosClient>(cc => new CosmosClientBuilder(url,authKey)
    .WithCustomSerializer(new CosmosSystemTextJsonSerializer(new System.Text.Json.JsonSerializerOptions(){
    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    }))
    .Build()
);

// Map Interface with Actual Infra Repo
builder.Services.AddScoped<IContactRepository, ContactRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
