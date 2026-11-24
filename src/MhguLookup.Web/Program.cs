using MhguLookup.Web;
using MhguLookup.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<LookupData>();
builder.Services.AddSingleton<Icons>();
builder.Services.AddSingleton<LocalStore>();
builder.Services.AddSingleton<UiSettings>();
builder.Services.AddSingleton<Bookmarks>();
builder.Services.AddSingleton<Notes>();

var host = builder.Build();
host.Services.GetRequiredService<UiSettings>().ApplySaved();
await host.Services.GetRequiredService<Icons>().LoadAsync();   // ~120 KB, fetched behind the loading screen
await host.RunAsync();
