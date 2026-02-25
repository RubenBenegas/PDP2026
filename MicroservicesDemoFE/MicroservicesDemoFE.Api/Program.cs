using MicroservicesDemoFE.Api.Handler;
using MicroservicesDemoFE.Api.Services.Concrete;
using MicroservicesDemoFE.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor(); // Necesario para acceder al HttpContext

builder.Services.AddHttpClient<IMovieService, MovieService>()
    .AddHttpMessageHandler<TokenHandler>();
builder.Services.AddHttpClient<IBookService, BookService>()
    .AddHttpMessageHandler<TokenHandler>();
builder.Services.AddHttpClient<IReviewService, ReviewService>()
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient(); // necesario para HttpClientFactory

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
