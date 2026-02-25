var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Program.cs en tu API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE",
        policy => policy.WithOrigins("https://localhost:5003") // Puerto de tu front
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});



var app = builder.Build();

app.UseCors("AllowFE");

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
