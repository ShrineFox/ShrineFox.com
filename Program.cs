using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using ShrineFoxCom.Components;

var builder = WebApplication.CreateBuilder(args);

// dotnet publish ShrineFoxCom.csproj -c Releas -o ./publish

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorDownloadFile();

//builder.WebHost.UseWebRoot("wwwroot").UseStaticWebAssets(); // fix css isolation?

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
