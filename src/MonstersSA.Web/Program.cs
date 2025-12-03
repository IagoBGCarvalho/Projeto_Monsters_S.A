using MonstersSA.Web.Components;
using MonstersSA.Web.Data;
using MonstersSA.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Pegando a connection string do appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=monsters.db";

// Configuração do BD com FACTORY
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Aumentando o limite do SignalR para uploads de arquivos grandes
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 20 * 1024 * 1024; // 20MB
});

// Registrando serviços de 
builder.Services.AddScoped<StatementProcessingService>();
builder.Services.AddScoped<ReportsService>();

// Serviços padrão do Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configuração do pipeline do HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
