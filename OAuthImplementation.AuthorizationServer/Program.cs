using OAuthImplementation.AuthorizationServer.Repositories;
using OAuthImplementation.Shared.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new FlexibleCaseModelBinderProvider());
});


builder.Services.AddSingleton<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();

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
