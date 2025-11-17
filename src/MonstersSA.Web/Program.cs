using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Environment.EnvironmentName = Environments.Development;

// Adicionando o contexto da aplicação:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Registro de serviços (lógica do Excel e Relatórios)
builder.Services.AddScoped<IStatementProcessingService, StatementProcessingService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// CRIANDO O BD AUTOMATICAMENTE NA PRIMEIRA EXECUÇÃO DO PROJETO:
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
