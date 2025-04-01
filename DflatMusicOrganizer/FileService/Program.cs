var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var baseDirectory = builder.Configuration.GetValue<string>("BaseDirectory");

app.MapPost("/list-directory", (string? relativePath) =>
{
    string targetDirectory = baseDirectory;

    if (!string.IsNullOrEmpty(relativePath))
    {
        // Sanitize the relative path to prevent directory traversal attacks
        relativePath = relativePath.Replace("..", "").Replace("/", "\\").TrimStart('\\');
        targetDirectory = Path.Combine(baseDirectory, relativePath);
    }
    if (!Directory.Exists(targetDirectory))
    {
        return Results.NotFound("Directory not found.");
    }

    var directoryContents = Directory.EnumerateFileSystemEntries(targetDirectory)
                                     .Select(entry => new
                                     {
                                         Name = Path.GetFileName(entry),
                                         Type = Directory.Exists(entry) ? "Directory" : "File"
                                     });

    return Results.Ok(directoryContents);
})
.WithName("ListDirectory");



app.Run();