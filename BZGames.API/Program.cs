using BZGames.API;
using BZGames.API.Hubs;
using BZGames.Application;
using BZGames.Infrastructure;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.WithOrigins("*");
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseHttpsRedirection();

var imagesStorageConnectionString = builder.Configuration.GetSection("ConnectionStrings:ImageStorage").Value;
var imagesStoragePath = Path.Combine(Directory.GetCurrentDirectory(), imagesStorageConnectionString);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesStoragePath),
    RequestPath = $"/{imagesStorageConnectionString}"
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<RPSHub>("/rpsGame");
app.MapHub<C4Hub>("/c4Game");
app.MapHub<TTTHub>("/tttGame");
app.MapHub<BTSHub>("/btsGame");
app.MapHub<GRCHub>("/grcHub");

app.Run();
