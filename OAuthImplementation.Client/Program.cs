using Microsoft.AspNetCore.Authentication.Cookies;
using OAuthImplementation.Client;
using OAuthImplementation.Client.RequestHandlers;
using OAuthImplementation.Shared.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new FlexibleCaseModelBinderProvider());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();
builder.Services.AddHttpClient("ImageApi")
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);


builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("Client"));
builder.Services.Configure<AuthServerSettings>(builder.Configuration.GetSection("AuthServer"));
builder.Services.Configure<ImageApiSettings>(builder.Configuration.GetSection("ImageApi"));

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

app.MapControllers();

app.Run();
