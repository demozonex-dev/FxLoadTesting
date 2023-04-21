using Fx.WebApi.Call.Injector.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<IHttpInjector, HttpInjector>();
builder.Services.AddHttpClient<HttpInjector>("Injector", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["InjectorUrl"]);
    httpClient.DefaultRequestHeaders.Add("Accept", "Application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
