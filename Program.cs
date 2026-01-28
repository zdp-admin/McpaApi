using McpaApi.Database.Mysql;
using McpaApi.Jobs;
using McpaApi.Models;
using McpaApi.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/McpaApi-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); 

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

//config db contexts 
builder.Services.AddDbContext<McpaCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("McpaCommercialConnection")));
builder.Services.AddDbContext<SuresteCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SuresteCommercialConnection")));
builder.Services.AddDbContext<PuntoSurCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PuntoSurCommercialConnection")));
builder.Services.AddDbContext<MexCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MexCommercialConnection")));
builder.Services.AddDbContext<GarageCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("GarageCommercialConnection")));
builder.Services.AddDbContext<AguaZulCommercialDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AguaZulCommercialConnection")));

//mysql db context 
builder.Services.AddDbContext<ShopGarageDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("ShopGarageConnection"), new MySqlServerVersion(new Version(8, 4, 5))));
builder.Services.AddDbContext<ShopAguaAzulDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("ShopAguaAzulConnection"), new MySqlServerVersion(new Version(8, 4, 5))));
builder.Services.AddDbContext<ShopPuntoSurDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("ShopPuntoSurConnection"), new MySqlServerVersion(new Version(8, 4, 5))));

//Config Inject Services
builder.Services.AddScoped<McpaService>();
builder.Services.AddScoped<ShopGarageService>();
builder.Services.AddScoped<ShopPuntoSurService>();
builder.Services.AddScoped<ShopAguaAzulService>();
builder.Services.AddScoped<ReportService>();
//Config Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILealtadService, LealtadService>();

//Config Jobs
/*builder.Services.AddQuartz(options =>
{
  // Los trabajos pueden usar inyección de dependencias directamente
  /*options.AddJob<EmailJob>(job => job
      .WithIdentity("EmailJob"));

  options.AddTrigger(trigger => trigger
      .ForJob("EmailJob")
      .WithIdentity("EmailJob-trigger")
      .WithCronSchedule("0 0/1 * * * ?"));*/
  //.WithCronSchedule("0 0 0 * * ?")); // Cada noche a las 00:00

  /*options.AddJob<ReportJob>(job => job.WithIdentity("ReportJob"));
  options.AddTrigger(trigger => trigger.ForJob("ReportJob")
  .WithIdentity("ReportJob-trigger")
  .WithCronSchedule("0 0 20 * * ?"));*/
  //.WithCronSchedule("0 0/1 * * * ?"));

  /*options.AddJob<LealtadJob>(job => job
        .WithIdentity("LealtadJob"));
  options.AddTrigger(trigger => trigger
      .ForJob("LealtadJob")
      .WithIdentity("LealtadJob-trigger")*/
      /*.WithCronSchedule("0 0/1 * * * ?"));*/
      //.WithCronSchedule("0 0 21 * * ?"));

  //options.UseDefaultThreadPool(tp => tp.MaxConcurrency = 2);
//});
// Ejecutar Quartz como servicio hospedado
/*builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});*/


//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();