using FileService;
using FileService.Common;
using FileService.Infrastructure;
using FileService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var baseDirectory = builder.Configuration.GetRequiredValue<string>("BaseDirectory");
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IDirectoryService>(
    provider => new DirectoryService(baseDirectory, provider.GetRequiredService<IFileSystem>()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapPost("/list-directory", (IDirectoryService directoryService, RelativePath? relativePath) =>
{
    var directoryContents = directoryService.ListDirectoryContents(relativePath);
    if (directoryContents.IsSuccess)
        return Results.Ok(directoryContents.Value);

    return Results.BadRequest(directoryContents.Error);
})
.WithName("ListDirectory");



app.Run();
