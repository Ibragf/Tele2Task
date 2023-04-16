using Microsoft.EntityFrameworkCore;
using test_task.Db;
using test_task.Services;
using test_task.Services.DataProviderService;
using test_task.Services.ResidentsClient.ResidentsClient;
using test_task.Services.ResidentsClientService;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: true);

var connectionString = builder.Configuration.GetConnectionString("postgres");
var taskAddress = builder.Configuration["TaskAddress"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddHttpClient<IResidentsClient, ResidentsClient>(client => client.BaseAddress = new Uri(taskAddress!));

builder.Services.AddTransient<IDataProvider,DataProvider>();

var app = builder.Build();

var dataProvider = app.Services.GetRequiredService<IDataProvider>();
var task = dataProvider.GetResidentsAsync();
task.Wait();
var residents = task.Result;

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    var resident = context.Residents.FirstOrDefault();
    if (resident == null)
    {
        context.Residents.AddRange(residents);
        context.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
