using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/upload", async (HttpContext context, IConfiguration config) =>
{
    var response = context.Response;
    var request = context.Request;
    IFormFileCollection files = request.Form.Files;
    var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
    Directory.CreateDirectory(uploadPath);

    foreach (var file in files)
    {
        string fullPath = $"{uploadPath}/{file.FileName}";

        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
    }
});

app.MapGet("/send/{name}", async (HttpContext context, string name) =>
{
    string filePath = $"uploads\\{name}.jpg";
    if (File.Exists(filePath))
    {
        await context.Response.SendFileAsync($"uploads\\{name}.jpg");

    }
    else
    {
        await context.Response.WriteAsync("Not found");
    }
    Results.Ok("Send");
}
);

app.MapGet("/random", async (HttpContext context) =>
{
    DirectoryInfo d = new DirectoryInfo(@"uploads"); //Assuming Test is your Folder

    FileInfo[] Files = d.GetFiles("*.jpg"); //Getting Text files
    List<string> files = new List<string>();
    foreach (FileInfo file in Files)
    {
        files.Add(file.Name);
    }
    Random rnd = new Random();
    int index = rnd.Next(0, files.Count);
    string filePath = $"uploads\\{files[index]}";
    await context.Response.SendFileAsync(filePath);
    Results.Ok("Send");
}
);

app.Run();
